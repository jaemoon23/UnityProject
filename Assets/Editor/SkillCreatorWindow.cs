using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Issue #273: 스킬 제작 에디터 툴
/// Window > Skill Creator로 열기
/// </summary>
public class SkillCreatorWindow : EditorWindow
{
    private SkillAssetData loadedSkill;
    private SkillAssetData currentSkill;
    private SerializedObject serializedSkill;
    private Vector2 scrollPosition;
    private string validationMessage = "";
    private MessageType validationMessageType = MessageType.Info;

    [MenuItem("Window/Skill Creator")]
    public static void ShowWindow()
    {
        SkillCreatorWindow window = GetWindow<SkillCreatorWindow>("Skill Creator");
        window.minSize = new Vector2(500, 600);
    }

    private void OnEnable()
    {
        if (currentSkill == null)
        {
            currentSkill = CreateInstance<SkillAssetData>();
            currentSkill.hideFlags = HideFlags.DontSave; // 에디터에서만 사용
        }
        serializedSkill = new SerializedObject(currentSkill);
    }

    private void OnGUI()
    {
        if (serializedSkill == null || serializedSkill.targetObject == null)
        {
            if (currentSkill == null)
            {
                currentSkill = CreateInstance<SkillAssetData>();
                currentSkill.hideFlags = HideFlags.DontSave;
            }
            serializedSkill = new SerializedObject(currentSkill);
        }

        serializedSkill.Update();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawLoadSection();
        EditorGUILayout.Space(10);
        DrawSeparator();
        EditorGUILayout.Space(10);

        DrawCategorySection();
        EditorGUILayout.Space(10);

        DrawBasicInfoSection();
        EditorGUILayout.Space(10);

        // 메인 스킬일 때만 기본 능력치/속성/이펙트/타입별 설정 표시
        if (currentSkill.skillCategory == SkillCategory.Main)
        {
            DrawStatsSection();
            EditorGUILayout.Space(10);

            DrawAttributesSection();
            EditorGUILayout.Space(10);

            DrawEffectsSection();
            EditorGUILayout.Space(10);

            DrawTypeSpecificSection();
            EditorGUILayout.Space(10);
        }
        else // 보조 스킬일 때
        {
            DrawSupportSkillSection();
            EditorGUILayout.Space(10);
        }

        DrawSeparator();
        EditorGUILayout.Space(10);

        DrawPreviewSection();
        EditorGUILayout.Space(10);

        DrawValidationSection();
        EditorGUILayout.Space(10);

        DrawCreateButton();

        EditorGUILayout.EndScrollView();

        serializedSkill.ApplyModifiedProperties();
    }

    private void DrawLoadSection()
    {
        EditorGUILayout.LabelField("📋 기존 스킬 불러오기 (선택사항)", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        loadedSkill = (SkillAssetData)EditorGUILayout.ObjectField(loadedSkill, typeof(SkillAssetData), false);
        if (GUILayout.Button("Load", GUILayout.Width(60)))
        {
            if (loadedSkill != null)
            {
                CopySkillData(loadedSkill, currentSkill);
                serializedSkill = new SerializedObject(currentSkill);
                validationMessage = "스킬 로드 완료";
                validationMessageType = MessageType.Info;
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawCategorySection()
    {
        EditorGUILayout.LabelField("📋 스킬 카테고리 선택", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.skillCategory = (SkillCategory)EditorGUILayout.EnumPopup("카테고리", currentSkill.skillCategory);
        EditorGUILayout.EndVertical();
    }

    private void DrawBasicInfoSection()
    {
        EditorGUILayout.LabelField("📋 기본 정보", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.skillName = EditorGUILayout.TextField("스킬 이름", currentSkill.skillName);

        // 메인 스킬일 때만 스킬 타입 표시
        if (currentSkill.skillCategory == SkillCategory.Main)
        {
            currentSkill.skillType = (SkillAssetType)EditorGUILayout.EnumPopup("스킬 타입", currentSkill.skillType);
        }

        currentSkill.description = EditorGUILayout.TextArea(currentSkill.description, GUILayout.Height(60));
        EditorGUILayout.EndVertical();
    }

    private void DrawStatsSection()
    {
        EditorGUILayout.LabelField("⚔️ 기본 능력치", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.baseDamage = EditorGUILayout.FloatField("기본 데미지", currentSkill.baseDamage);
        currentSkill.cooldown = EditorGUILayout.FloatField("쿨다운 (초)", currentSkill.cooldown);
        currentSkill.manaCost = EditorGUILayout.FloatField("마나 소모", currentSkill.manaCost);
        currentSkill.castTime = EditorGUILayout.FloatField("시전 시간 (초)", currentSkill.castTime);
        currentSkill.range = EditorGUILayout.FloatField("사거리 (m)", currentSkill.range);
        EditorGUILayout.EndVertical();
    }

    private void DrawAttributesSection()
    {
        EditorGUILayout.LabelField("🎨 속성", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.elementType = (ElementType)EditorGUILayout.EnumPopup("속성 타입", currentSkill.elementType);
        currentSkill.damageType = (DamageType)EditorGUILayout.EnumPopup("데미지 타입", currentSkill.damageType);
        EditorGUILayout.EndVertical();
    }

    private void DrawEffectsSection()
    {
        EditorGUILayout.LabelField("✨ 이펙트 프리팹", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        // 스킬 타입에 따라 다른 이펙트 UI 표시
        switch (currentSkill.skillType)
        {
            case SkillAssetType.Projectile:
                DrawProjectileEffects();
                break;

            case SkillAssetType.AOE:
                DrawAOEEffects();
                break;

            case SkillAssetType.Channeling:
                DrawChannelingEffects();
                break;

            case SkillAssetType.Flicker:
                DrawFlickerEffects();
                break;

            case SkillAssetType.DOT:
                DrawDOTEffects();
                break;

            case SkillAssetType.Summon:
                DrawSummonEffects();
                break;

            default:
                DrawGenericEffects();
                break;
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawProjectileEffects()
    {
        EditorGUILayout.LabelField("🎯 발사체 스킬 이펙트", EditorStyles.boldLabel);

        currentSkill.castEffectPrefab = (GameObject)EditorGUILayout.ObjectField("시전 이펙트 (Cast Effect)", currentSkill.castEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField("  └ 스킬 발동 순간, 캐스터 위치에서 재생", EditorStyles.miniLabel);
        EditorGUILayout.Space(3);

        currentSkill.projectileEffectPrefab = (GameObject)EditorGUILayout.ObjectField("투사체 비주얼 (Projectile Visual)", currentSkill.projectileEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField("  └ 투사체를 따라다니는 파티클", EditorStyles.miniLabel);
        EditorGUILayout.Space(3);

        currentSkill.hitEffectPrefab = (GameObject)EditorGUILayout.ObjectField("피격 이펙트 (Hit Effect)", currentSkill.hitEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField("  └ 타겟 충돌 시 폭발/충격 효과", EditorStyles.miniLabel);

        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox("💡 Retro Arsenal 추천:\n• Cast Effect: Combat/Muzzleflash\n• Projectile Visual: Combat/Missiles\n• Hit Effect: Combat/Explosions", MessageType.None);
    }

    private void DrawAOEEffects()
    {
        EditorGUILayout.LabelField("💥 AOE 스킬 이펙트", EditorStyles.boldLabel);

        currentSkill.castEffectPrefab = (GameObject)EditorGUILayout.ObjectField("시전 이펙트 (Cast Effect)", currentSkill.castEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField("  └ 시전 시간 동안 재생, 캐스터 위치", EditorStyles.miniLabel);
        EditorGUILayout.Space(3);

        currentSkill.projectileEffectPrefab = (GameObject)EditorGUILayout.ObjectField("스킬 비주얼 (Skill Visual)", currentSkill.projectileEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField("  └ 스킬의 메인 비주얼 이펙트", EditorStyles.miniLabel);
        EditorGUILayout.Space(3);

        currentSkill.hitEffectPrefab = (GameObject)EditorGUILayout.ObjectField("피격 이펙트 (Hit Effect)", currentSkill.hitEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField("  └ 타겟 충돌 판정 시 발생하는 이펙트", EditorStyles.miniLabel);
        EditorGUILayout.Space(3);

        currentSkill.areaEffectPrefab = (GameObject)EditorGUILayout.ObjectField("범위 이펙트 (Area Effect)", currentSkill.areaEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField("  └ AOE 스킬의 지속 범위 표시 이펙트", EditorStyles.miniLabel);

        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox("💡 Retro Arsenal 추천:\n• Cast Effect: Combat/Muzzleflash\n• Skill Visual: Combat/Explosions (지속형)\n• Hit Effect: Combat/Impact\n• Area Effect: Combat/AOE Indicators", MessageType.None);
    }

    private void DrawChannelingEffects()
    {
        EditorGUILayout.LabelField("🌊 채널링 레이저/빔 스킬 이펙트", EditorStyles.boldLabel);

        currentSkill.castEffectPrefab = (GameObject)EditorGUILayout.ObjectField("시전 이펙트 (Cast Effect)", currentSkill.castEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField($"  └ 시전 시간 ({currentSkill.castTime:F1}초) 동안 캐릭터 위치에서 재생", EditorStyles.miniLabel);
        EditorGUILayout.Space(3);

        currentSkill.projectileEffectPrefab = (GameObject)EditorGUILayout.ObjectField("시작점 이펙트 (Start Effect) [선택]", currentSkill.projectileEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField("  └ 채널링 시작 시 캐릭터 위치에서 재생 (빔 발사 지점)", EditorStyles.miniLabel);
        EditorGUILayout.Space(3);

        currentSkill.areaEffectPrefab = (GameObject)EditorGUILayout.ObjectField("빔 이펙트 (Beam Effect) [필수]", currentSkill.areaEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField($"  └ 캐릭터→타겟 연결되는 레이저/빔 ({currentSkill.channelDuration:F1}초 동안 유지)", EditorStyles.miniLabel);
        EditorGUILayout.Space(3);

        currentSkill.hitEffectPrefab = (GameObject)EditorGUILayout.ObjectField("피격 이펙트 (Hit Effect) [선택]", currentSkill.hitEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField($"  └ 타겟(몬스터) 위치에서 틱({currentSkill.channelTickInterval:F1}초)마다 재생", EditorStyles.miniLabel);

        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox(
            "💡 채널링 레이저/빔 동작 흐름:\n" +
            $"1. Cast Effect ({currentSkill.castTime:F1}초) - 캐스팅 준비\n" +
            $"2. Start Effect (시작 시) - 캐릭터 위치, 빔 발사 지점 [선택]\n" +
            $"3. Beam Effect ({currentSkill.channelDuration:F1}초) - 캐릭터→타겟 연결 빔 [필수]\n" +
            $"4. Hit Effect (틱마다) - 타겟 피격 지점 [선택]\n\n" +
            "구조: [캐릭터]→(Start)→(Beam)→(Hit)→[몬스터]\n\n" +
            "📝 Start Effect와 Hit Effect는 선택사항입니다.\n" +
            "통짜 이펙트를 사용하는 경우 Beam Effect만 설정하세요.",
            MessageType.Info
        );
    }

    private void DrawFlickerEffects()
    {
        EditorGUILayout.LabelField("👻 플리커 스킬 이펙트", EditorStyles.boldLabel);

        currentSkill.castEffectPrefab = (GameObject)EditorGUILayout.ObjectField("시전 이펙트 (Cast Effect)", currentSkill.castEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField("  └ 스킬 발동 순간, 캐스터 위치", EditorStyles.miniLabel);
        EditorGUILayout.Space(3);

        currentSkill.projectileEffectPrefab = (GameObject)EditorGUILayout.ObjectField("순간이동 이펙트 (Dash Effect)", currentSkill.projectileEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField("  └ 각 순간이동마다 재생되는 이펙트", EditorStyles.miniLabel);
        EditorGUILayout.Space(3);

        currentSkill.hitEffectPrefab = (GameObject)EditorGUILayout.ObjectField("피격 이펙트 (Hit Effect)", currentSkill.hitEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField("  └ 타겟 타격 시 재생", EditorStyles.miniLabel);

        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox("💡 플리커 스킬:\n각 순간이동마다 Dash Effect가 재생되며,\n타겟 타격 시 Hit Effect 발생", MessageType.None);
    }

    private void DrawDOTEffects()
    {
        EditorGUILayout.LabelField("🔥 DOT 스킬 이펙트", EditorStyles.boldLabel);

        currentSkill.castEffectPrefab = (GameObject)EditorGUILayout.ObjectField("시전 이펙트 (Cast Effect)", currentSkill.castEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField("  └ 스킬 발동 순간, 캐스터 위치", EditorStyles.miniLabel);
        EditorGUILayout.Space(3);

        currentSkill.projectileEffectPrefab = (GameObject)EditorGUILayout.ObjectField("DOT 지속 이펙트 (DOT Effect)", currentSkill.projectileEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField($"  └ DOT 지속 시간({currentSkill.dotDuration:F1}초) 동안 타겟을 따라다니며 재생", EditorStyles.miniLabel);
        EditorGUILayout.Space(3);

        currentSkill.hitEffectPrefab = (GameObject)EditorGUILayout.ObjectField("피격 이펙트 (Hit Effect)", currentSkill.hitEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField($"  └ 각 틱({currentSkill.dotTickInterval:F1}초)마다 재생", EditorStyles.miniLabel);

        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox("💡 DOT 스킬:\nDOT Effect는 타겟의 자식으로 붙어 따라다니며,\n각 틱마다 Hit Effect 재생", MessageType.None);
    }

    private void DrawSummonEffects()
    {
        EditorGUILayout.LabelField("💎 소환 스킬 이펙트", EditorStyles.boldLabel);

        currentSkill.castEffectPrefab = (GameObject)EditorGUILayout.ObjectField("시전 이펙트 (Cast Effect)", currentSkill.castEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField("  └ 스킬 발동 순간, 캐스터 위치", EditorStyles.miniLabel);
        EditorGUILayout.Space(3);

        currentSkill.projectileEffectPrefab = (GameObject)EditorGUILayout.ObjectField("소환 이펙트 (Summon Effect)", currentSkill.projectileEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.LabelField("  └ 소환 대상이 등장할 때 재생", EditorStyles.miniLabel);

        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox("💡 소환 스킬:\nCast Effect 후 Summon Effect와 함께\n소환 프리팹이 생성됨", MessageType.None);
    }

    private void DrawGenericEffects()
    {
        EditorGUILayout.LabelField("✨ 기본 이펙트", EditorStyles.boldLabel);

        currentSkill.castEffectPrefab = (GameObject)EditorGUILayout.ObjectField("시전 이펙트", currentSkill.castEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.Space(3);

        currentSkill.projectileEffectPrefab = (GameObject)EditorGUILayout.ObjectField("스킬 비주얼", currentSkill.projectileEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.Space(3);

        currentSkill.hitEffectPrefab = (GameObject)EditorGUILayout.ObjectField("피격 이펙트", currentSkill.hitEffectPrefab, typeof(GameObject), false);
        EditorGUILayout.Space(3);

        currentSkill.areaEffectPrefab = (GameObject)EditorGUILayout.ObjectField("범위 이펙트", currentSkill.areaEffectPrefab, typeof(GameObject), false);
    }

    private void DrawTypeSpecificSection()
    {
        switch (currentSkill.skillType)
        {
            case SkillAssetType.Projectile:
                DrawProjectileSection();
                break;
            case SkillAssetType.AOE:
                DrawAOESection();
                break;
            case SkillAssetType.DOT:
                DrawDOTSection();
                break;
            case SkillAssetType.Buff:
            case SkillAssetType.Debuff:
                DrawBuffDebuffSection();
                break;
            case SkillAssetType.Flicker:
                DrawFlickerSection();
                break;
            case SkillAssetType.Channeling:
                DrawChannelingSection();
                break;
            case SkillAssetType.Summon:
                DrawSummonSection();
                break;
            case SkillAssetType.Shield:
                DrawShieldSection();
                break;
            case SkillAssetType.Trap:
            case SkillAssetType.Mine:
                DrawTrapMineSection();
                break;
        }
    }

    private void DrawProjectileSection()
    {
        EditorGUILayout.LabelField("🎯 Projectile 전용 설정", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.projectileSpeed = EditorGUILayout.FloatField("투사체 속도 (m/s)", currentSkill.projectileSpeed);
        currentSkill.projectileLifetime = EditorGUILayout.FloatField("생존 시간 (초)", currentSkill.projectileLifetime);
        currentSkill.projectileCount = EditorGUILayout.IntField("발사 개수", currentSkill.projectileCount);
        currentSkill.pierceCount = EditorGUILayout.IntField("관통 횟수", currentSkill.pierceCount);
        currentSkill.isHoming = EditorGUILayout.Toggle("유도탄 (타겟 추적)", currentSkill.isHoming);
        EditorGUILayout.EndVertical();
    }

    private void DrawAOESection()
    {
        EditorGUILayout.LabelField("💥 AOE 전용 설정", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.aoeRadius = EditorGUILayout.FloatField("범위 반경 (m)", currentSkill.aoeRadius);
        currentSkill.aoeAngle = EditorGUILayout.Slider("각도 (°)", currentSkill.aoeAngle, 0f, 360f);
        currentSkill.aoeCenterOnCaster = EditorGUILayout.Toggle("시전자 중심", currentSkill.aoeCenterOnCaster);
        EditorGUILayout.EndVertical();
    }

    private void DrawDOTSection()
    {
        EditorGUILayout.LabelField("🔥 DOT 전용 설정", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.dotDuration = EditorGUILayout.FloatField("지속 시간 (초)", currentSkill.dotDuration);
        currentSkill.dotTickInterval = EditorGUILayout.FloatField("틱 간격 (초)", currentSkill.dotTickInterval);
        currentSkill.dotDamagePerTick = EditorGUILayout.FloatField("틱당 데미지", currentSkill.dotDamagePerTick);
        EditorGUILayout.EndVertical();
    }

    private void DrawBuffDebuffSection()
    {
        EditorGUILayout.LabelField("⚡ Buff/Debuff 전용 설정", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.buffDuration = EditorGUILayout.FloatField("지속 시간 (초)", currentSkill.buffDuration);
        currentSkill.isStackable = EditorGUILayout.Toggle("중첩 가능", currentSkill.isStackable);
        if (currentSkill.isStackable)
        {
            currentSkill.maxStacks = EditorGUILayout.IntField("최대 중첩", currentSkill.maxStacks);
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawFlickerSection()
    {
        EditorGUILayout.LabelField("👻 Flicker 전용 설정", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.flickerDashCount = EditorGUILayout.IntField("돌진 횟수", currentSkill.flickerDashCount);
        currentSkill.flickerDashRange = EditorGUILayout.FloatField("돌진 거리 (m)", currentSkill.flickerDashRange);
        currentSkill.flickerDashInterval = EditorGUILayout.FloatField("돌진 간격 (초)", currentSkill.flickerDashInterval);
        currentSkill.flickerReturnToOrigin = EditorGUILayout.Toggle("원래 위치로 복귀", currentSkill.flickerReturnToOrigin);
        EditorGUILayout.EndVertical();
    }

    private void DrawChannelingSection()
    {
        EditorGUILayout.LabelField("🌊 Channeling 전용 설정", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("⏱️ 타이밍 설정", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"  • 시전 시간: {currentSkill.castTime:F1}초 (⚔️ 기본 능력치 섹션에서 설정)", EditorStyles.miniLabel);
        currentSkill.channelDuration = EditorGUILayout.FloatField("채널링 시간 (초)", currentSkill.channelDuration);
        currentSkill.channelTickInterval = EditorGUILayout.FloatField("틱 간격 (초)", currentSkill.channelTickInterval);

        int tickCount = currentSkill.channelTickInterval > 0 ? Mathf.FloorToInt(currentSkill.channelDuration / currentSkill.channelTickInterval) : 0;
        EditorGUILayout.LabelField($"  총 틱 횟수: {tickCount}회", EditorStyles.miniLabel);

        EditorGUILayout.Space(5);
        currentSkill.interruptible = EditorGUILayout.Toggle("중단 가능", currentSkill.interruptible);

        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox(
            "💡 채널링 스킬 동작 흐름:\n" +
            $"1. 시전 시간 ({currentSkill.castTime:F1}초): Cast Effect 재생 (캐스팅 준비)\n" +
            $"2. 채널링 시간 ({currentSkill.channelDuration:F1}초): Skill Visual 유지 (채널링 이펙트)\n" +
            $"3. {currentSkill.channelTickInterval:F1}초마다 데미지/발사체 발사 (총 {tickCount}회)\n\n" +
            "⚡ Support 스킬 효과:\n" +
            "• 다중 발사 (Additional Projectiles)\n" +
            "• 체이닝 (Chain)\n" +
            "• 기타 상태 이상 효과 모두 적용 가능\n\n" +
            "📝 시전 시간 변경:\n" +
            "위쪽 '⚔️ 기본 능력치' 섹션에서 '시전 시간 (초)' 값을 변경하세요",
            MessageType.Info
        );

        EditorGUILayout.EndVertical();
    }

    private void DrawSummonSection()
    {
        EditorGUILayout.LabelField("💎 Summon 전용 설정", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.summonPrefab = (GameObject)EditorGUILayout.ObjectField("소환 프리팹", currentSkill.summonPrefab, typeof(GameObject), false);
        currentSkill.summonCount = EditorGUILayout.IntField("소환 개수", currentSkill.summonCount);
        currentSkill.summonDuration = EditorGUILayout.FloatField("지속 시간 (초)", currentSkill.summonDuration);
        EditorGUILayout.EndVertical();
    }

    private void DrawShieldSection()
    {
        EditorGUILayout.LabelField("🛡️ Shield 전용 설정", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.shieldAmount = EditorGUILayout.FloatField("보호막 수치", currentSkill.shieldAmount);
        currentSkill.shieldDuration = EditorGUILayout.FloatField("지속 시간 (초)", currentSkill.shieldDuration);
        currentSkill.absorbsDamage = EditorGUILayout.Toggle("데미지 흡수", currentSkill.absorbsDamage);
        EditorGUILayout.EndVertical();
    }

    private void DrawTrapMineSection()
    {
        EditorGUILayout.LabelField("🎭 Trap/Mine 전용 설정", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.trapPrefab = (GameObject)EditorGUILayout.ObjectField("Trap/Mine 프리팹", currentSkill.trapPrefab, typeof(GameObject), false);
        currentSkill.trapArmTime = EditorGUILayout.FloatField("설치 시간 (초)", currentSkill.trapArmTime);
        currentSkill.trapTriggerRadius = EditorGUILayout.FloatField("발동 반경 (m)", currentSkill.trapTriggerRadius);
        currentSkill.trapDuration = EditorGUILayout.FloatField("지속 시간 (초)", currentSkill.trapDuration);
        EditorGUILayout.EndVertical();
    }

    private void DrawSupportSkillSection()
    {
        EditorGUILayout.LabelField("🔧 메인 스킬 변형 효과", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.additionalProjectiles = EditorGUILayout.IntField("발사체 개수 추가", currentSkill.additionalProjectiles);
        currentSkill.additionalPierceCount = EditorGUILayout.IntField("관통 횟수 추가", currentSkill.additionalPierceCount);
        currentSkill.aoeRadiusMultiplier = EditorGUILayout.FloatField("AOE 반경 증가 (%)", currentSkill.aoeRadiusMultiplier);
        currentSkill.projectileSpeedMultiplier = EditorGUILayout.FloatField("투사체 속도 증가 (%)", currentSkill.projectileSpeedMultiplier);
        currentSkill.durationMultiplier = EditorGUILayout.FloatField("지속 시간 증가 (%)", currentSkill.durationMultiplier);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("⚡ 캐릭터 스텟 변형 (%)", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.damageModifier = EditorGUILayout.FloatField("데미지 변형 (%)", currentSkill.damageModifier);
        currentSkill.attackSpeedModifier = EditorGUILayout.FloatField("공격 속도 변형 (%)", currentSkill.attackSpeedModifier);
        currentSkill.manaCostModifier = EditorGUILayout.FloatField("마나 소모 변형 (%)", currentSkill.manaCostModifier);
        currentSkill.castTimeModifier = EditorGUILayout.FloatField("시전 시간 변형 (%)", currentSkill.castTimeModifier);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);

        DrawStatusEffectSection();
    }

    private void DrawStatusEffectSection()
    {
        EditorGUILayout.LabelField("💫 상태 이상 효과", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.statusEffectType = (StatusEffectType)EditorGUILayout.EnumPopup("상태 이상 타입", currentSkill.statusEffectType);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(5);

        // 선택된 상태 이상 타입에 따라 설정 UI 표시
        switch (currentSkill.statusEffectType)
        {
            case StatusEffectType.CC:
                DrawCCSection();
                break;
            case StatusEffectType.DOT:
                DrawDOTStatusSection();
                break;
            case StatusEffectType.Mark:
                DrawMarkSection();
                break;
            case StatusEffectType.Chain:
                DrawChainSection();
                break;
        }
    }

    private void DrawCCSection()
    {
        EditorGUILayout.LabelField("🎯 CC (군중 제어) 설정", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.ccType = (CCType)EditorGUILayout.EnumPopup("CC 타입", currentSkill.ccType);
        currentSkill.ccDuration = EditorGUILayout.FloatField("CC 지속시간 (초)", currentSkill.ccDuration);

        if (currentSkill.ccType == CCType.Slow)
        {
            currentSkill.ccSlowAmount = EditorGUILayout.Slider("둔화 정도 (%)", currentSkill.ccSlowAmount, 0f, 100f);
        }

        EditorGUILayout.Space(5);
        currentSkill.ccEffectPrefab = (GameObject)EditorGUILayout.ObjectField("CC 이펙트 (몬스터를 따라다니며 재생)", currentSkill.ccEffectPrefab, typeof(GameObject), false);

        EditorGUILayout.HelpBox("💡 Tip:\n• Stun/Freeze: 몬스터 dizzy 애니메이션 + 이동/공격 불가\n• Slow: 이동 속도 감소 (미구현)\n• Root: 이동 불가, 공격 가능 (미구현)\n• 이펙트는 몬스터 transform의 자식으로 붙어 따라다님", MessageType.Info);
        EditorGUILayout.EndVertical();
    }

    private void DrawDOTStatusSection()
    {
        EditorGUILayout.LabelField("🔥 DOT (지속 데미지) 설정", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.dotType = (DOTType)EditorGUILayout.EnumPopup("DOT 타입", currentSkill.dotType);
        currentSkill.dotDamagePerTick = EditorGUILayout.FloatField("틱당 데미지", currentSkill.dotDamagePerTick);
        currentSkill.dotTickInterval = EditorGUILayout.FloatField("틱 간격 (초)", currentSkill.dotTickInterval);
        currentSkill.dotDuration = EditorGUILayout.FloatField("DOT 지속시간 (초)", currentSkill.dotDuration);

        int tickCount = currentSkill.dotTickInterval > 0 ? Mathf.FloorToInt(currentSkill.dotDuration / currentSkill.dotTickInterval) : 0;
        float totalDamage = tickCount * currentSkill.dotDamagePerTick;
        EditorGUILayout.LabelField($"총 틱 횟수: {tickCount}회");
        EditorGUILayout.LabelField($"총 DOT 데미지: {totalDamage:F1}");

        EditorGUILayout.Space(5);
        currentSkill.dotEffectPrefab = (GameObject)EditorGUILayout.ObjectField("DOT 이펙트 (몬스터를 따라다니며 재생)", currentSkill.dotEffectPrefab, typeof(GameObject), false);

        EditorGUILayout.HelpBox("💡 Tip: 화상, 중독, 출혈 등 지속 데미지 효과\n이펙트는 몬스터 transform의 자식으로 붙어 따라다님", MessageType.Info);
        EditorGUILayout.EndVertical();
    }

    private void DrawMarkSection()
    {
        EditorGUILayout.LabelField("⭐ Mark (표식) 설정", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.markType = (MarkType)EditorGUILayout.EnumPopup("Mark 타입", currentSkill.markType);
        currentSkill.markDuration = EditorGUILayout.FloatField("Mark 지속시간 (초)", currentSkill.markDuration);
        currentSkill.markDamageMultiplier = EditorGUILayout.FloatField("추가 데미지 배율 (%)", currentSkill.markDamageMultiplier);
        currentSkill.markEffectPrefab = (GameObject)EditorGUILayout.ObjectField("Mark 이펙트", currentSkill.markEffectPrefab, typeof(GameObject), false);

        EditorGUILayout.HelpBox("💡 Tip: 표식이 있는 몬스터에게 추가 데미지", MessageType.Info);
        EditorGUILayout.EndVertical();
    }

    private void DrawChainSection()
    {
        EditorGUILayout.LabelField("⚡ Chain (연쇄 공격) 설정", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        currentSkill.chainCount = EditorGUILayout.IntField("Chain 횟수", currentSkill.chainCount);
        currentSkill.chainRange = EditorGUILayout.FloatField("Chain 범위 (m)", currentSkill.chainRange);
        currentSkill.chainDamageReduction = EditorGUILayout.Slider("Chain 데미지 감소율 (%)", currentSkill.chainDamageReduction, 0f, 100f);

        EditorGUILayout.Space(5);
        currentSkill.chainEffectPrefab = (GameObject)EditorGUILayout.ObjectField("Chain 이펙트 (번개가 튕기는 비주얼)", currentSkill.chainEffectPrefab, typeof(GameObject), false);

        if (currentSkill.chainCount > 0)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("📊 Chain 정보", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"최대 타격 적 수: {currentSkill.chainCount + 1}명 (첫 타격 + Chain {currentSkill.chainCount}회)");

            // 각 Chain별 데미지 계산
            float currentDamage = 100f; // 기준 데미지
            EditorGUILayout.LabelField("데미지 변화:");
            EditorGUILayout.LabelField($"  1번째 타격: {currentDamage:F1}%");
            for (int i = 1; i <= currentSkill.chainCount; i++)
            {
                currentDamage *= (1f - currentSkill.chainDamageReduction / 100f);
                EditorGUILayout.LabelField($"  {i + 1}번째 타격 (Chain {i}): {currentDamage:F1}%");
            }
        }

        EditorGUILayout.HelpBox("💡 Tip:\n• 투사체가 첫 타격 후 가까운 적에게 연쇄 공격\n• Chain 범위 내의 가장 가까운 적을 찾아 튕김\n• 데미지 감소율을 설정하여 Chain될수록 약한 데미지\n• Chain 이펙트는 적에서 적으로 튕기는 번개 비주얼", MessageType.Info);
        EditorGUILayout.EndVertical();
    }

    private void DrawPreviewSection()
    {
        EditorGUILayout.LabelField("📊 프리뷰 정보 (자동 계산)", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        switch (currentSkill.skillType)
        {
            case SkillAssetType.Projectile:
                float maxRange = currentSkill.projectileSpeed * currentSkill.projectileLifetime;
                float dps = currentSkill.cooldown > 0 ? currentSkill.baseDamage / currentSkill.cooldown : 0;
                float dpm = currentSkill.manaCost > 0 ? currentSkill.baseDamage / currentSkill.manaCost : 0;
                EditorGUILayout.LabelField($"• 최대 사거리: {maxRange:F1}m ({currentSkill.projectileSpeed:F1}m/s × {currentSkill.projectileLifetime:F1}초)");
                EditorGUILayout.LabelField($"• DPS: {dps:F1} ({currentSkill.baseDamage:F0} 데미지 / {currentSkill.cooldown:F1}초)");
                EditorGUILayout.LabelField($"• 마나 효율: {dpm:F2} DPM (데미지 per 마나)");
                break;

            case SkillAssetType.Flicker:
                float totalDistance = currentSkill.flickerDashCount * currentSkill.flickerDashRange;
                float totalTime = currentSkill.flickerDashCount * currentSkill.flickerDashInterval;
                float burstDPS = totalTime > 0 ? (currentSkill.baseDamage * currentSkill.flickerDashCount) / totalTime : 0;
                EditorGUILayout.LabelField($"• 총 이동거리: {totalDistance:F1}m ({currentSkill.flickerDashCount}회 × {currentSkill.flickerDashRange:F1}m)");
                EditorGUILayout.LabelField($"• 총 소요시간: {totalTime:F2}초 ({currentSkill.flickerDashCount}회 × {currentSkill.flickerDashInterval:F2}초)");
                EditorGUILayout.LabelField($"• 버스트 DPS: {burstDPS:F1}");
                break;

            case SkillAssetType.DOT:
                int tickCount = currentSkill.dotTickInterval > 0 ? Mathf.FloorToInt(currentSkill.dotDuration / currentSkill.dotTickInterval) : 0;
                float totalDotDamage = tickCount * currentSkill.dotDamagePerTick;
                EditorGUILayout.LabelField($"• 총 틱 횟수: {tickCount}회 ({currentSkill.dotDuration:F1}초 / {currentSkill.dotTickInterval:F1}초)");
                EditorGUILayout.LabelField($"• 총 DOT 데미지: {totalDotDamage:F1} ({tickCount}회 × {currentSkill.dotDamagePerTick:F1})");
                EditorGUILayout.LabelField($"• 즉발 + DOT: {currentSkill.baseDamage + totalDotDamage:F1}");
                break;

            case SkillAssetType.Channeling:
                int channelTickCount = currentSkill.channelTickInterval > 0 ? Mathf.FloorToInt(currentSkill.channelDuration / currentSkill.channelTickInterval) : 0;
                float totalChannelTime = currentSkill.castTime + currentSkill.channelDuration;
                float totalChannelDamage = currentSkill.baseDamage * channelTickCount;
                float channelDPS = totalChannelTime > 0 ? totalChannelDamage / totalChannelTime : 0;
                EditorGUILayout.LabelField($"• 총 소요 시간: {totalChannelTime:F1}초 (시전 {currentSkill.castTime:F1}초 + 채널링 {currentSkill.channelDuration:F1}초)");
                EditorGUILayout.LabelField($"• 총 틱 횟수: {channelTickCount}회 ({currentSkill.channelDuration:F1}초 / {currentSkill.channelTickInterval:F1}초)");
                EditorGUILayout.LabelField($"• 총 데미지: {totalChannelDamage:F1} ({currentSkill.baseDamage:F1} × {channelTickCount}회)");
                EditorGUILayout.LabelField($"• 평균 DPS: {channelDPS:F1} ({totalChannelDamage:F1} / {totalChannelTime:F1}초)");
                break;
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawValidationSection()
    {
        if (!string.IsNullOrEmpty(validationMessage))
        {
            EditorGUILayout.HelpBox(validationMessage, validationMessageType);
        }
    }

    private void DrawCreateButton()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField($"저장 경로: Assets/ScriptableObjects/Skills/", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.Space(5);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button($"🔨 스킬 생성 ({currentSkill.skillName}.asset)", GUILayout.Height(40)))
        {
            CreateSkillAsset();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndVertical();
    }

    private void CreateSkillAsset()
    {
        // 검증
        if (!currentSkill.Validate(out string errorMessage))
        {
            validationMessage = $"❌ {errorMessage}";
            validationMessageType = MessageType.Error;
            return;
        }

        // 경로 생성
        string folderPath = "Assets/ScriptableObjects/Skills";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 파일 이름 생성
        string assetPath = $"{folderPath}/{currentSkill.skillName}.asset";
        assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

        // SO 생성
        SkillAssetData newSkill = CreateInstance<SkillAssetData>();
        CopySkillData(currentSkill, newSkill);

        AssetDatabase.CreateAsset(newSkill, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorGUIUtility.PingObject(newSkill);
        Selection.activeObject = newSkill;

        validationMessage = $"✅ 스킬 생성 완료: {assetPath}";
        validationMessageType = MessageType.Info;
    }

    private void CopySkillData(SkillAssetData source, SkillAssetData dest)
    {
        dest.skillName = source.skillName;
        dest.skillType = source.skillType;
        dest.description = source.description;

        dest.baseDamage = source.baseDamage;
        dest.cooldown = source.cooldown;
        dest.manaCost = source.manaCost;
        dest.castTime = source.castTime;
        dest.range = source.range;

        dest.elementType = source.elementType;
        dest.damageType = source.damageType;

        dest.castEffectPrefab = source.castEffectPrefab;
        dest.projectileEffectPrefab = source.projectileEffectPrefab;
        dest.hitEffectPrefab = source.hitEffectPrefab;
        dest.areaEffectPrefab = source.areaEffectPrefab;

        dest.projectileSpeed = source.projectileSpeed;
        dest.projectileLifetime = source.projectileLifetime;
        dest.projectileCount = source.projectileCount;
        dest.pierceCount = source.pierceCount;
        dest.isHoming = source.isHoming;

        dest.aoeRadius = source.aoeRadius;
        dest.aoeAngle = source.aoeAngle;
        dest.aoeCenterOnCaster = source.aoeCenterOnCaster;

        dest.dotDuration = source.dotDuration;
        dest.dotTickInterval = source.dotTickInterval;
        dest.dotDamagePerTick = source.dotDamagePerTick;

        dest.buffDuration = source.buffDuration;
        dest.isStackable = source.isStackable;
        dest.maxStacks = source.maxStacks;

        dest.flickerDashCount = source.flickerDashCount;
        dest.flickerDashRange = source.flickerDashRange;
        dest.flickerDashInterval = source.flickerDashInterval;
        dest.flickerReturnToOrigin = source.flickerReturnToOrigin;

        dest.channelDuration = source.channelDuration;
        dest.channelTickInterval = source.channelTickInterval;
        dest.interruptible = source.interruptible;

        dest.summonPrefab = source.summonPrefab;
        dest.summonCount = source.summonCount;
        dest.summonDuration = source.summonDuration;

        dest.shieldAmount = source.shieldAmount;
        dest.shieldDuration = source.shieldDuration;
        dest.absorbsDamage = source.absorbsDamage;

        dest.trapPrefab = source.trapPrefab;
        dest.trapArmTime = source.trapArmTime;
        dest.trapTriggerRadius = source.trapTriggerRadius;
        dest.trapDuration = source.trapDuration;

        dest.skillCategory = source.skillCategory;
        dest.additionalProjectiles = source.additionalProjectiles;
        dest.additionalPierceCount = source.additionalPierceCount;
        dest.aoeRadiusMultiplier = source.aoeRadiusMultiplier;
        dest.projectileSpeedMultiplier = source.projectileSpeedMultiplier;
        dest.durationMultiplier = source.durationMultiplier;
        dest.damageModifier = source.damageModifier;
        dest.attackSpeedModifier = source.attackSpeedModifier;
        dest.manaCostModifier = source.manaCostModifier;
        dest.castTimeModifier = source.castTimeModifier;

        // Status effect fields
        dest.statusEffectType = source.statusEffectType;
        dest.ccType = source.ccType;
        dest.ccDuration = source.ccDuration;
        dest.ccSlowAmount = source.ccSlowAmount;
        dest.dotType = source.dotType;
        dest.markType = source.markType;
        dest.markDuration = source.markDuration;
        dest.markDamageMultiplier = source.markDamageMultiplier;
        dest.markEffectPrefab = source.markEffectPrefab;
        dest.chainCount = source.chainCount;
        dest.chainRange = source.chainRange;
        dest.chainDamageReduction = source.chainDamageReduction;
        dest.chainEffectPrefab = source.chainEffectPrefab;
        dest.ccEffectPrefab = source.ccEffectPrefab;
        dest.dotEffectPrefab = source.dotEffectPrefab;
    }

    private void DrawSeparator()
    {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
}
