using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 스킬 Prefab 데이터베이스
/// CSV 데이터와 Prefab을 연결하는 ScriptableObject
/// 경로: Assets/ScriptableObjects/Skills/SkillPrefabDatabase.asset
/// </summary>
[CreateAssetMenu(fileName = "SkillPrefabDatabase", menuName = "Skills/Skill Prefab Database", order = 0)]
public class SkillPrefabDatabase : ScriptableObject
{
    private const string ASSET_PATH = "Assets/ScriptableObjects/Skills/SkillPrefabDatabase.asset";

    private static SkillPrefabDatabase _instance;
    private static bool _isLoading = false;

    public static SkillPrefabDatabase Instance
    {
        get
        {
            if (_instance == null && !_isLoading)
            {
                LoadInstance();
            }
            return _instance;
        }
    }

    /// <summary>
    /// 인스턴스 로드 (에디터: AssetDatabase, 빌드: Addressables)
    /// </summary>
    private static void LoadInstance()
    {
        _isLoading = true;

#if UNITY_EDITOR
        // 에디터: AssetDatabase로 직접 로드
        _instance = AssetDatabase.LoadAssetAtPath<SkillPrefabDatabase>(ASSET_PATH);
        if (_instance == null)
        {
            // 프로젝트에서 검색
            string[] guids = AssetDatabase.FindAssets("t:SkillPrefabDatabase");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _instance = AssetDatabase.LoadAssetAtPath<SkillPrefabDatabase>(path);
            }
        }

        if (_instance == null)
        {
            Debug.LogWarning($"[SkillPrefabDatabase] Not found at {ASSET_PATH}. Create via Tools > Skills > Skill Manager");
        }
#else
        // 빌드: Addressables 동기 로드 시도
        try
        {
            var handle = Addressables.LoadAssetAsync<SkillPrefabDatabase>(AddressableKey.SkillPrefabDatabase);
            _instance = handle.WaitForCompletion();
        }
        catch (Exception e)
        {
            Debug.LogError($"[SkillPrefabDatabase] Failed to load from Addressables: {e.Message}");
        }
#endif

        _isLoading = false;
    }

    /// <summary>
    /// 인스턴스 수동 설정 (GameManager 등에서 Addressables 로드 후 호출)
    /// </summary>
    public static void SetInstance(SkillPrefabDatabase database)
    {
        _instance = database;
        _instance?.Initialize();
    }

    /// <summary>
    /// Addressables 비동기 로드 (권장)
    /// </summary>
    public static async Cysharp.Threading.Tasks.UniTask LoadInstanceAsync()
    {
        if (_instance != null) return;

        _isLoading = true;

        try
        {
            var handle = Addressables.LoadAssetAsync<SkillPrefabDatabase>(AddressableKey.SkillPrefabDatabase);
            await handle.Task;
            _instance = handle.Result;
            _instance?.Initialize();
            Debug.Log("[SkillPrefabDatabase] Loaded from Addressables");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SkillPrefabDatabase] Async load failed: {e.Message}");
        }

        _isLoading = false;
    }

    [Header("메인 스킬 Prefab 목록")]
    [Tooltip("CSV의 skill_id와 Prefab을 연결합니다")]
    public List<MainSkillPrefabEntry> mainSkillPrefabs = new List<MainSkillPrefabEntry>();

    [Header("서포트 스킬 Prefab 목록")]
    [Tooltip("CSV의 support_id와 Prefab을 연결합니다")]
    public List<SupportSkillPrefabEntry> supportSkillPrefabs = new List<SupportSkillPrefabEntry>();

    // 런타임 캐시
    private Dictionary<int, MainSkillPrefabEntry> _mainSkillCache;
    private Dictionary<int, SupportSkillPrefabEntry> _supportSkillCache;

    /// <summary>
    /// 캐시 초기화 (필요 시 호출)
    /// </summary>
    public void Initialize()
    {
        BuildCache();
    }

    private void OnEnable()
    {
        BuildCache();
    }

    private void BuildCache()
    {
        _mainSkillCache = new Dictionary<int, MainSkillPrefabEntry>();
        foreach (var entry in mainSkillPrefabs)
        {
            if (!_mainSkillCache.ContainsKey(entry.skillId))
            {
                _mainSkillCache[entry.skillId] = entry;
            }
            else
            {
                Debug.LogWarning($"[SkillPrefabDatabase] Duplicate skill_id: {entry.skillId}");
            }
        }

        _supportSkillCache = new Dictionary<int, SupportSkillPrefabEntry>();
        foreach (var entry in supportSkillPrefabs)
        {
            if (!_supportSkillCache.ContainsKey(entry.supportId))
            {
                _supportSkillCache[entry.supportId] = entry;
            }
            else
            {
                Debug.LogWarning($"[SkillPrefabDatabase] Duplicate support_id: {entry.supportId}");
            }
        }
    }

    #region Main Skill Methods

    /// <summary>
    /// 메인 스킬 Prefab 엔트리 조회
    /// </summary>
    public MainSkillPrefabEntry GetMainSkillEntry(int skillId)
    {
        if (_mainSkillCache == null) BuildCache();

        if (_mainSkillCache.TryGetValue(skillId, out var entry))
        {
            return entry;
        }
        return null;
    }

    /// <summary>
    /// 메인 스킬의 투사체 Prefab 조회
    /// </summary>
    public GameObject GetMainSkillProjectile(int skillId)
    {
        return GetMainSkillEntry(skillId)?.projectilePrefab;
    }

    /// <summary>
    /// 메인 스킬의 피격 이펙트 Prefab 조회
    /// </summary>
    public GameObject GetMainSkillHitEffect(int skillId)
    {
        return GetMainSkillEntry(skillId)?.hitEffectPrefab;
    }

    /// <summary>
    /// 메인 스킬의 시전 이펙트 Prefab 조회
    /// </summary>
    public GameObject GetMainSkillCastEffect(int skillId)
    {
        return GetMainSkillEntry(skillId)?.castEffectPrefab;
    }

    /// <summary>
    /// 메인 스킬의 범위 이펙트 Prefab 조회
    /// </summary>
    public GameObject GetMainSkillAreaEffect(int skillId)
    {
        return GetMainSkillEntry(skillId)?.areaEffectPrefab;
    }

    #endregion

    #region Support Skill Methods

    /// <summary>
    /// 서포트 스킬 Prefab 엔트리 조회
    /// </summary>
    public SupportSkillPrefabEntry GetSupportSkillEntry(int supportId)
    {
        if (_supportSkillCache == null) BuildCache();

        if (_supportSkillCache.TryGetValue(supportId, out var entry))
        {
            return entry;
        }
        return null;
    }

    /// <summary>
    /// 서포트 스킬의 이펙트 Prefab 조회
    /// </summary>
    public GameObject GetSupportSkillEffect(int supportId)
    {
        return GetSupportSkillEntry(supportId)?.effectPrefab;
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// 메인 스킬 데이터 + Prefab 함께 조회
    /// </summary>
    public (MainSkillData data, MainSkillPrefabEntry prefabs) GetMainSkillWithPrefabs(int skillId)
    {
        var data = CSVLoader.Instance?.GetData<MainSkillData>(skillId);
        var prefabs = GetMainSkillEntry(skillId);
        return (data, prefabs);
    }

    /// <summary>
    /// 서포트 스킬 데이터 + Prefab 함께 조회
    /// </summary>
    public (SupportSkillData data, SupportSkillPrefabEntry prefabs) GetSupportSkillWithPrefabs(int supportId)
    {
        var data = CSVLoader.Instance?.GetData<SupportSkillData>(supportId);
        var prefabs = GetSupportSkillEntry(supportId);
        return (data, prefabs);
    }

    /// <summary>
    /// Prefab이 연결되지 않은 메인 스킬 ID 목록 반환
    /// </summary>
    public List<int> GetUnlinkedMainSkillIds()
    {
        var unlinked = new List<int>();

        if (CSVLoader.Instance == null || !CSVLoader.Instance.IsInit)
            return unlinked;

        var table = CSVLoader.Instance.GetTable<MainSkillData>();
        if (table == null) return unlinked;

        foreach (var skill in table.GetAll())
        {
            var entry = GetMainSkillEntry(skill.skill_id);
            if (entry == null || !entry.HasAnyPrefab())
            {
                unlinked.Add(skill.skill_id);
            }
        }

        return unlinked;
    }

    /// <summary>
    /// Prefab이 연결되지 않은 서포트 스킬 ID 목록 반환
    /// </summary>
    public List<int> GetUnlinkedSupportSkillIds()
    {
        var unlinked = new List<int>();

        if (CSVLoader.Instance == null || !CSVLoader.Instance.IsInit)
            return unlinked;

        var table = CSVLoader.Instance.GetTable<SupportSkillData>();
        if (table == null) return unlinked;

        foreach (var skill in table.GetAll())
        {
            var entry = GetSupportSkillEntry(skill.support_id);
            if (entry == null || !entry.HasAnyPrefab())
            {
                unlinked.Add(skill.support_id);
            }
        }

        return unlinked;
    }

    #endregion
}

/// <summary>
/// 메인 스킬 Prefab 엔트리
/// </summary>
[Serializable]
public class MainSkillPrefabEntry
{
    [Tooltip("CSV의 skill_id")]
    public int skillId;

    [Tooltip("스킬 이름 (표시용, CSV에서 자동 설정)")]
    public string skillName;

    [Header("Prefabs")]
    [Tooltip("투사체 Prefab")]
    public GameObject projectilePrefab;

    [Tooltip("피격 이펙트 Prefab")]
    public GameObject hitEffectPrefab;

    [Tooltip("시전 이펙트 Prefab")]
    public GameObject castEffectPrefab;

    [Tooltip("범위 이펙트 Prefab (AOE용)")]
    public GameObject areaEffectPrefab;

    [Tooltip("트레일 이펙트 Prefab")]
    public GameObject trailEffectPrefab;

    /// <summary>
    /// 최소 하나의 Prefab이 연결되어 있는지 확인
    /// </summary>
    public bool HasAnyPrefab()
    {
        return projectilePrefab != null ||
               hitEffectPrefab != null ||
               castEffectPrefab != null ||
               areaEffectPrefab != null ||
               trailEffectPrefab != null;
    }
}

/// <summary>
/// 서포트 스킬 Prefab 엔트리
/// </summary>
[Serializable]
public class SupportSkillPrefabEntry
{
    [Tooltip("CSV의 support_id")]
    public int supportId;

    [Tooltip("서포트 스킬 이름 (표시용, CSV에서 자동 설정)")]
    public string supportName;

    [Header("Prefabs")]
    [Tooltip("서포트 스킬 이펙트 Prefab")]
    public GameObject effectPrefab;

    [Tooltip("CC 이펙트 Prefab (스턴, 슬로우 등)")]
    public GameObject ccEffectPrefab;

    [Tooltip("DOT 이펙트 Prefab (화상, 중독 등)")]
    public GameObject dotEffectPrefab;

    [Tooltip("마크 이펙트 Prefab")]
    public GameObject markEffectPrefab;

    [Tooltip("체인 이펙트 Prefab")]
    public GameObject chainEffectPrefab;

    /// <summary>
    /// 최소 하나의 Prefab이 연결되어 있는지 확인
    /// </summary>
    public bool HasAnyPrefab()
    {
        return effectPrefab != null ||
               ccEffectPrefab != null ||
               dotEffectPrefab != null ||
               markEffectPrefab != null ||
               chainEffectPrefab != null;
    }
}
