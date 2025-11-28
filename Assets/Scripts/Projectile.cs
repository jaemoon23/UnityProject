#if false
// LMJ: Old Projectile disabled for combat system refactor (Issue #265)
// Will be replaced with straight-line projectile
using NovelianMagicLibraryDefense.Managers;
using UnityEngine;

//JML: Projectile with Rigidbody-based movement and target tracking
public class Projectile_OLD : MonoBehaviour, IPoolable
{
    // Old code preserved for reference
}
#endif

//LMJ : Unified projectile system - supports both physics and effect modes
//      Migrated to new CSV-based skill system
namespace Novelian.Combat
{
    using NovelianMagicLibraryDefense.Managers;
    using UnityEngine;
    using Cysharp.Threading.Tasks;
    using System.Threading;

    public enum ProjectileMode
    {
        Physics,    // Rigidbody-based with collision detection
        Effect      // Visual-only with lerp movement
    }

    public class Projectile : MonoBehaviour, IPoolable
    {
        private const float OUT_OF_BOUNDS_DISTANCE = 100f;

        [Header("Components")]
        [SerializeField, Tooltip("Rigidbody for physics movement (Physics mode only)")]
        private Rigidbody rb;

        [Header("Damage")]
        [SerializeField, Tooltip("Projectile damage")]
        private float damage = 10f;

        // Movement mode
        private ProjectileMode mode = ProjectileMode.Physics;
        private System.Action<Vector3> onHitCallback;

        // Skill data (CSV-based)
        private int skillId;
        private MainSkillData skillData;
        private MainSkillPrefabEntry skillPrefabs;

        // Support skill data for status effects (CSV-based)
        private int supportSkillId;
        private SupportSkillData supportSkillData;
        private SupportSkillPrefabEntry supportPrefabs;

        // Chain state tracking
        private int currentChainCount = 0;
        private int maxChainCount = 0;
        private System.Collections.Generic.HashSet<ITargetable> chainHitTargets;
        private float currentChainDamage = 0f;

        // Movement state
        private Vector3 fixedDirection;
        private float speed;
        private float lifetime;
        private Vector3 startPosition;
        private Vector3 targetPosition;
        private float elapsedTime;
        private bool isInitialized = false;

        // Lifetime tracking
        private CancellationTokenSource lifetimeCts;

        //LMJ : Launch projectile in Physics mode - basic version (for backward compatibility)
        public void Launch(Vector3 spawnPos, Vector3 targetPos, float projectileSpeed, float projectileLifetime)
        {
            Launch(spawnPos, targetPos, projectileSpeed, projectileLifetime, this.damage, 0, 0);
        }

        //LMJ : Launch projectile in Physics mode - with skill IDs (new CSV-based system)
        public void Launch(Vector3 spawnPos, Vector3 targetPos, float projectileSpeed, float projectileLifetime, float damageAmount, int mainSkillId, int supportId)
        {
            mode = ProjectileMode.Physics;
            transform.position = spawnPos;
            startPosition = spawnPos;
            targetPosition = targetPos;

            // Calculate fixed direction (NO HOMING)
            fixedDirection = (targetPos - spawnPos).normalized;
            speed = projectileSpeed;
            lifetime = projectileLifetime;
            damage = damageAmount;
            elapsedTime = 0f;
            isInitialized = true;

            // Load skill data from CSV and PrefabDatabase
            LoadSkillData(mainSkillId, supportId);

            // Spawn effect prefab as child if skillData is provided
            if (skillPrefabs != null && skillPrefabs.projectilePrefab != null)
            {
                // Clear any existing child effects
                foreach (Transform child in transform)
                {
                    if (child.gameObject != null)
                    {
                        Object.Destroy(child.gameObject);
                    }
                }

                // Spawn new effect as child
                GameObject effectInstance = Object.Instantiate(skillPrefabs.projectilePrefab, transform);
                effectInstance.transform.localPosition = Vector3.zero;
                effectInstance.transform.localRotation = Quaternion.LookRotation(fixedDirection);
                Debug.Log($"[Projectile] Effect prefab spawned as child: {skillPrefabs.projectilePrefab.name}");
            }

            // Initialize Chain state (only on first launch, not re-launch)
            if (currentChainCount == 0 && supportSkillData != null && supportSkillData.GetStatusEffectType() == StatusEffectType.Chain)
            {
                maxChainCount = supportSkillData.chain_count;
                chainHitTargets = new System.Collections.Generic.HashSet<ITargetable>();
                currentChainDamage = damageAmount;
                Debug.Log($"[Projectile] Chain initialized: maxChainCount={maxChainCount}, initialDamage={currentChainDamage:F1}");
            }

            // Cancel previous lifetime token
            lifetimeCts?.Cancel();
            lifetimeCts = new CancellationTokenSource();

            // Start lifetime countdown
            TrackLifetimeAsync(lifetimeCts.Token).Forget();

            Debug.Log($"[Projectile] Physics mode launched from {spawnPos} to {targetPos}, damage={damage:F1}, chainCount={currentChainCount}/{maxChainCount}");
        }

        //LMJ : Load skill data from CSV and PrefabDatabase
        private void LoadSkillData(int mainSkillId, int supportId)
        {
            skillId = mainSkillId;
            supportSkillId = supportId;
            skillData = null;
            skillPrefabs = null;
            supportSkillData = null;
            supportPrefabs = null;

            if (CSVLoader.Instance == null || !CSVLoader.Instance.IsInit)
            {
                Debug.LogWarning("[Projectile] CSVLoader not initialized");
                return;
            }

            var prefabDb = SkillPrefabDatabase.Instance;

            // Load main skill data
            if (mainSkillId > 0)
            {
                skillData = CSVLoader.Instance.GetData<MainSkillData>(mainSkillId);
                if (skillData != null)
                {
                    skillPrefabs = prefabDb?.GetMainSkillEntry(mainSkillId);
                }
            }

            // Load support skill data
            if (supportId > 0)
            {
                supportSkillData = CSVLoader.Instance.GetData<SupportSkillData>(supportId);
                if (supportSkillData != null)
                {
                    supportPrefabs = prefabDb?.GetSupportSkillEntry(supportId);
                }
            }
        }

        //LMJ : Launch projectile in Effect mode (for visual-only projectiles without physics)
        public void LaunchEffect(Vector3 spawnPos, Vector3 targetPos, float projectileSpeed, float projectileLifetime, float damageAmount, System.Action<Vector3> onHit = null, int supportId = 0)
        {
            mode = ProjectileMode.Effect;
            transform.position = spawnPos;
            startPosition = spawnPos;
            targetPosition = targetPos;

            fixedDirection = (targetPos - spawnPos).normalized;
            speed = projectileSpeed;
            lifetime = projectileLifetime;
            damage = damageAmount;
            onHitCallback = onHit;
            elapsedTime = 0f;
            isInitialized = true;

            // Load support skill data
            LoadSkillData(0, supportId);

            transform.rotation = Quaternion.LookRotation(fixedDirection);

            // Set layer to Projectile for proper collision detection
            gameObject.layer = LayerMask.NameToLayer("Projectile");

            // Add Kinematic Rigidbody for collision detection (required for Trigger detection)
            Rigidbody effectRb = gameObject.GetComponent<Rigidbody>();
            if (effectRb == null)
            {
                effectRb = gameObject.AddComponent<Rigidbody>();
            }
            effectRb.isKinematic = true;
            effectRb.useGravity = false;
            effectRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            // Add SphereCollider for collision detection
            SphereCollider collider = gameObject.GetComponent<SphereCollider>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<SphereCollider>();
            }
            collider.isTrigger = true;
            collider.radius = 1.0f;

            // Cancel previous lifetime token
            lifetimeCts?.Cancel();
            lifetimeCts = new CancellationTokenSource();

            // Start effect movement
            EffectMovementAsync(lifetimeCts.Token).Forget();

            Debug.Log($"[Projectile] Effect mode launched from {spawnPos} to {targetPos}");
        }

        //LMJ : Physics-based movement in fixed direction (Physics mode only)
        private void FixedUpdate()
        {
            if (mode != ProjectileMode.Physics) return;
            if (!isInitialized) return;

            if (Time.timeScale == 0f)
            {
                if (rb != null) rb.linearVelocity = Vector3.zero;
                return;
            }

            if (rb != null) rb.linearVelocity = fixedDirection * speed;

            if (fixedDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(fixedDirection);
            }

            if (Vector3.Distance(startPosition, transform.position) > OUT_OF_BOUNDS_DISTANCE)
            {
                ReturnToPool();
            }
        }

        //LMJ : Effect-based movement with lerp (Effect mode only)
        private async UniTaskVoid EffectMovementAsync(CancellationToken ct)
        {
            try
            {
                while (isInitialized && !ct.IsCancellationRequested)
                {
                    if (Time.timeScale == 0f)
                    {
                        await UniTask.Yield(ct);
                        continue;
                    }

                    elapsedTime += Time.deltaTime;

                    float distance = Vector3.Distance(startPosition, targetPosition);
                    float t = Mathf.Clamp01(elapsedTime * speed / distance);
                    transform.position = Vector3.Lerp(startPosition, targetPosition, t);

                    if (t >= 1f || elapsedTime >= lifetime)
                    {
                        OnReachTarget();
                        break;
                    }

                    await UniTask.Yield(ct);
                }
            }
            catch (System.OperationCanceledException)
            {
                // Expected
            }
        }

        //LMJ : Handle reaching target in Effect mode
        private void OnReachTarget()
        {
            isInitialized = false;
            onHitCallback?.Invoke(targetPosition);
            Destroy(gameObject);
        }

        //LMJ : Track lifetime and auto-despawn
        private async UniTaskVoid TrackLifetimeAsync(CancellationToken ct)
        {
            try
            {
                await UniTask.Delay((int)(lifetime * 1000), cancellationToken: ct);

                if (!ct.IsCancellationRequested)
                {
                    ReturnToPool();
                }
            }
            catch (System.OperationCanceledException)
            {
                // Expected when projectile hits target before lifetime ends
            }
        }

        //LMJ : Handle collision with monsters and obstacles (both Physics and Effect modes)
        private void OnTriggerEnter(Collider other)
        {
            if (!isInitialized) return;

            // Obstacle collision
            if (other.CompareTag(Tag.Obstacle))
            {
                if (mode == ProjectileMode.Physics)
                {
                    ReturnToPool();
                }
                else if (mode == ProjectileMode.Effect)
                {
                    lifetimeCts?.Cancel();
                    Destroy(gameObject);
                }
                return;
            }

            // Ground collision
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Debug.Log("[Projectile] Hit ground, destroying");
                if (mode == ProjectileMode.Physics)
                {
                    ReturnToPool();
                }
                else if (mode == ProjectileMode.Effect)
                {
                    lifetimeCts?.Cancel();
                    Destroy(gameObject);
                }
                return;
            }

            if (other.CompareTag(Tag.Monster))
            {
                Monster monster = other.GetComponent<Monster>();
                if (monster != null)
                {
                    // Apply status effects BEFORE damage
                    if (supportSkillData != null && supportSkillData.GetStatusEffectType() != StatusEffectType.Chain)
                    {
                        ApplyStatusEffect(monster);
                    }

                    // Apply damage
                    float damageToApply = (maxChainCount > 0) ? currentChainDamage : damage;
                    monster.TakeDamage(damageToApply);

                    // Spawn hit effect
                    if (skillPrefabs != null && skillPrefabs.hitEffectPrefab != null)
                    {
                        GameObject hitEffect = Object.Instantiate(skillPrefabs.hitEffectPrefab, other.transform.position, Quaternion.identity);
                        Object.Destroy(hitEffect, 2f);
                    }

                    // Add to hit targets for chain tracking
                    if (maxChainCount > 0)
                    {
                        chainHitTargets.Add(monster);
                    }

                    // Process Chain
                    if (supportSkillData != null && supportSkillData.GetStatusEffectType() == StatusEffectType.Chain && currentChainCount < maxChainCount)
                    {
                        // Spawn chain effect
                        if (supportPrefabs?.chainEffectPrefab != null && currentChainCount > 0)
                        {
                            SpawnChainEffect(startPosition, other.transform.position);
                        }

                        // Find next target
                        ITargetable nextTarget = FindNextChainTarget(other.transform.position, chainHitTargets, monster);

                        if (nextTarget != null)
                        {
                            currentChainDamage *= (1f - supportSkillData.chain_damage_reduction / 100f);
                            currentChainCount++;

                            Debug.Log($"[Projectile] Chain {currentChainCount}/{maxChainCount}: Bouncing to {nextTarget.GetTransform().name}");

                            Vector3 directionToNext = (nextTarget.GetPosition() - other.transform.position).normalized;
                            float spawnOffset = 1.0f;
                            Vector3 spawnPos = other.transform.position + directionToNext * spawnOffset;

                            Launch(spawnPos, nextTarget.GetPosition(), speed, lifetime, currentChainDamage, skillId, supportSkillId);
                            return;
                        }
                    }
                }

                // Cleanup
                if (mode == ProjectileMode.Physics)
                {
                    ReturnToPool();
                }
                else if (mode == ProjectileMode.Effect)
                {
                    lifetimeCts?.Cancel();
                    onHitCallback?.Invoke(other.transform.position);
                    Destroy(gameObject);
                }
            }
            else if (other.CompareTag(Tag.BossMonster))
            {
                BossMonster boss = other.GetComponent<BossMonster>();
                if (boss != null)
                {
                    // Apply status effects BEFORE damage
                    if (supportSkillData != null && supportSkillData.GetStatusEffectType() != StatusEffectType.Chain)
                    {
                        ApplyStatusEffectToBoss(boss);
                    }

                    // Apply damage
                    float damageToApply = (maxChainCount > 0) ? currentChainDamage : damage;
                    boss.TakeDamage(damageToApply);

                    // Spawn hit effect
                    if (skillPrefabs != null && skillPrefabs.hitEffectPrefab != null)
                    {
                        GameObject hitEffect = Object.Instantiate(skillPrefabs.hitEffectPrefab, other.transform.position, Quaternion.identity);
                        Object.Destroy(hitEffect, 2f);
                    }

                    // Add to hit targets for chain tracking
                    if (maxChainCount > 0)
                    {
                        chainHitTargets.Add(boss);
                    }

                    // Process Chain
                    if (supportSkillData != null && supportSkillData.GetStatusEffectType() == StatusEffectType.Chain && currentChainCount < maxChainCount)
                    {
                        if (supportPrefabs?.chainEffectPrefab != null && currentChainCount > 0)
                        {
                            SpawnChainEffect(startPosition, other.transform.position);
                        }

                        ITargetable nextTarget = FindNextChainTarget(other.transform.position, chainHitTargets, boss);

                        if (nextTarget != null)
                        {
                            currentChainDamage *= (1f - supportSkillData.chain_damage_reduction / 100f);
                            currentChainCount++;

                            Vector3 directionToNext = (nextTarget.GetPosition() - other.transform.position).normalized;
                            float spawnOffset = 1.0f;
                            Vector3 spawnPos = other.transform.position + directionToNext * spawnOffset;

                            Launch(spawnPos, nextTarget.GetPosition(), speed, lifetime, currentChainDamage, skillId, supportSkillId);
                            return;
                        }
                    }
                }

                // Cleanup
                if (mode == ProjectileMode.Physics)
                {
                    ReturnToPool();
                }
                else if (mode == ProjectileMode.Effect)
                {
                    lifetimeCts?.Cancel();
                    onHitCallback?.Invoke(other.transform.position);
                    Destroy(gameObject);
                }
            }
        }

        //LMJ : Apply status effect to monster
        private void ApplyStatusEffect(Monster monster)
        {
            if (supportSkillData == null || monster == null) return;

            // Get effect prefabs from database
            GameObject ccEffectPrefab = supportPrefabs?.ccEffectPrefab;
            GameObject dotEffectPrefab = supportPrefabs?.dotEffectPrefab;
            GameObject markEffectPrefab = supportPrefabs?.markEffectPrefab;

            switch (supportSkillData.GetStatusEffectType())
            {
                case StatusEffectType.CC:
                    monster.ApplyCC(supportSkillData.GetCCType(), supportSkillData.cc_duration, supportSkillData.cc_slow_amount, ccEffectPrefab);
                    break;

                case StatusEffectType.DOT:
                    monster.ApplyDOT(supportSkillData.GetDOTType(), supportSkillData.dot_damage_per_tick, supportSkillData.dot_tick_interval, supportSkillData.dot_duration, dotEffectPrefab);
                    break;

                case StatusEffectType.Mark:
                    monster.ApplyMark(supportSkillData.GetMarkType(), supportSkillData.mark_duration, supportSkillData.mark_damage_mult, markEffectPrefab);
                    break;

                case StatusEffectType.Chain:
                    // Chain is handled separately
                    break;
            }
        }

        //LMJ : Apply status effect to boss monster
        private void ApplyStatusEffectToBoss(BossMonster boss)
        {
            if (supportSkillData == null || boss == null) return;

            GameObject ccEffectPrefab = supportPrefabs?.ccEffectPrefab;
            GameObject dotEffectPrefab = supportPrefabs?.dotEffectPrefab;
            GameObject markEffectPrefab = supportPrefabs?.markEffectPrefab;

            switch (supportSkillData.GetStatusEffectType())
            {
                case StatusEffectType.CC:
                    boss.ApplyCC(supportSkillData.GetCCType(), supportSkillData.cc_duration, supportSkillData.cc_slow_amount, ccEffectPrefab);
                    break;

                case StatusEffectType.DOT:
                    boss.ApplyDOT(supportSkillData.GetDOTType(), supportSkillData.dot_damage_per_tick, supportSkillData.dot_tick_interval, supportSkillData.dot_duration, dotEffectPrefab);
                    break;

                case StatusEffectType.Mark:
                    boss.ApplyMark(supportSkillData.GetMarkType(), supportSkillData.mark_duration, supportSkillData.mark_damage_mult, markEffectPrefab);
                    break;

                case StatusEffectType.Chain:
                    break;
            }
        }

        //LMJ : Spawn chain effect visual between two positions
        private void SpawnChainEffect(Vector3 startPos, Vector3 endPos)
        {
            if (supportPrefabs == null || supportPrefabs.chainEffectPrefab == null) return;

            Vector3 midPos = (startPos + endPos) / 2f;
            GameObject chainEffect = Instantiate(supportPrefabs.chainEffectPrefab, midPos, Quaternion.identity);

            Vector3 direction = (endPos - startPos).normalized;
            if (direction != Vector3.zero)
            {
                chainEffect.transform.rotation = Quaternion.LookRotation(direction);
            }

            float distance = Vector3.Distance(startPos, endPos);
            chainEffect.transform.localScale = new Vector3(1f, 1f, distance);

            Destroy(chainEffect, 1f);
        }

        //LMJ : Find next target for chain effect
        private ITargetable FindNextChainTarget(Vector3 currentPosition, System.Collections.Generic.HashSet<ITargetable> hitTargets, ITargetable excludeTarget = null)
        {
            if (supportSkillData == null) return null;

            Collider[] hits = Physics.OverlapSphere(currentPosition, supportSkillData.chain_range);

            ITargetable closestTarget = null;
            float closestDistance = float.MaxValue;
            const float MIN_CHAIN_DISTANCE = 0.5f;

            foreach (var hit in hits)
            {
                if (!hit.CompareTag(Tag.Monster) && !hit.CompareTag(Tag.BossMonster))
                    continue;

                ITargetable target = hit.GetComponent<ITargetable>();
                if (target == null || !target.IsAlive())
                    continue;

                if (excludeTarget != null && target == excludeTarget)
                    continue;

                if (hitTargets.Contains(target))
                    continue;

                float distance = Vector3.Distance(currentPosition, target.GetPosition());

                if (distance < MIN_CHAIN_DISTANCE)
                    continue;

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = target;
                }
            }

            return closestTarget;
        }

        //LMJ : Return projectile to pool
        private void ReturnToPool()
        {
            lifetimeCts?.Cancel();
            GameManager.Instance.Pool.Despawn(this);
        }

        // IPoolable implementation
        public void OnSpawn()
        {
            mode = ProjectileMode.Physics;
            isInitialized = false;
            fixedDirection = Vector3.zero;
            elapsedTime = 0f;
            onHitCallback = null;

            if (rb != null) rb.linearVelocity = Vector3.zero;

            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in particleSystems)
            {
                var main = ps.main;
                main.simulationSpace = ParticleSystemSimulationSpace.Local;
            }
        }

        public void OnDespawn()
        {
            isInitialized = false;
            fixedDirection = Vector3.zero;
            elapsedTime = 0f;
            onHitCallback = null;

            // Clear skill references
            skillId = 0;
            skillData = null;
            skillPrefabs = null;
            supportSkillId = 0;
            supportSkillData = null;
            supportPrefabs = null;

            // Reset chain state
            currentChainCount = 0;
            maxChainCount = 0;
            chainHitTargets = null;
            currentChainDamage = 0f;

            if (rb != null) rb.linearVelocity = Vector3.zero;
            lifetimeCts?.Cancel();
        }

        private void OnDestroy()
        {
            lifetimeCts?.Cancel();
            lifetimeCts?.Dispose();
        }
    }
}
