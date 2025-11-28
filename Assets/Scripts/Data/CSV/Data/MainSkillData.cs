using System;
using CsvHelper.Configuration.Attributes;

/// <summary>
/// MainSkillTable.csv 데이터 클래스
/// CSV에서 메인 스킬 데이터를 파싱합니다.
/// </summary>
[Serializable]
public class MainSkillData
{
    [Name("skill_id")]
    public int skill_id { get; set; }

    [Name("skill_type")]
    public int skill_type { get; set; }

    [Name("element_type")]
    public int element_type { get; set; }

    [Name("damage_type")]
    public int damage_type { get; set; }

    [Name("base_damage")]
    public float base_damage { get; set; }

    [Name("cooldown")]
    public float cooldown { get; set; }

    [Name("mana_cost")]
    public float mana_cost { get; set; }

    [Name("cast_time")]
    public float cast_time { get; set; }

    [Name("range")]
    public float range { get; set; }

    [Name("projectile_speed")]
    public float projectile_speed { get; set; }

    [Name("projectile_count")]
    public int projectile_count { get; set; }

    [Name("projectile_lifetime")]
    public float projectile_lifetime { get; set; }

    [Name("pierce_count")]
    public int pierce_count { get; set; }

    [Name("is_homing")]
    public bool is_homing { get; set; }

    [Name("aoe_radius")]
    public float aoe_radius { get; set; }

    [Name("aoe_angle")]
    public float aoe_angle { get; set; }

    [Name("channel_duration")]
    public float channel_duration { get; set; }

    [Name("channel_tick_interval")]
    public float channel_tick_interval { get; set; }

    [Name("interruptible")]
    public bool interruptible { get; set; }

    // CSV의 //로 시작하는 주석 컬럼 (선택적)
    [Name("//skill_name")]
    [Optional]
    public string skill_name { get; set; }

    [Name("//description")]
    [Optional]
    public string description { get; set; }

    #region Helper Properties (Enum 변환)

    /// <summary>
    /// skill_type을 SkillEnumTable 기준 SkillAssetType으로 변환
    /// 1000=Projectile, 1001=AOE, 1002=DOT, etc.
    /// </summary>
    public SkillAssetType GetSkillType()
    {
        return skill_type switch
        {
            1000 => SkillAssetType.Projectile,
            1001 => SkillAssetType.AOE,
            1002 => SkillAssetType.DOT,
            1003 => SkillAssetType.Buff,
            1004 => SkillAssetType.Debuff,
            1005 => SkillAssetType.Heal,
            1006 => SkillAssetType.Summon,
            1007 => SkillAssetType.Teleport,
            1008 => SkillAssetType.Dash,
            1009 => SkillAssetType.Flicker,
            1010 => SkillAssetType.Channeling,
            1011 => SkillAssetType.Trap,
            1012 => SkillAssetType.Mine,
            1013 => SkillAssetType.Aura,
            1014 => SkillAssetType.Shield,
            _ => SkillAssetType.Projectile
        };
    }

    /// <summary>
    /// element_type을 ElementType으로 변환
    /// 2000=None, 2001=Fire, 2002=Ice, etc.
    /// </summary>
    public ElementType GetElementType()
    {
        return element_type switch
        {
            2000 => ElementType.None,
            2001 => ElementType.Fire,
            2002 => ElementType.Ice,
            2003 => ElementType.Lightning,
            2004 => ElementType.Poison,
            2005 => ElementType.Holy,
            2006 => ElementType.Dark,
            2007 => ElementType.Nature,
            2008 => ElementType.Arcane,
            _ => ElementType.None
        };
    }

    /// <summary>
    /// damage_type을 DamageType으로 변환
    /// 3000=Physical, 3001=Magical, 3002=Pure, 3003=Hybrid
    /// </summary>
    public DamageType GetDamageType()
    {
        return damage_type switch
        {
            3000 => DamageType.Physical,
            3001 => DamageType.Magical,
            3002 => DamageType.Pure,
            3003 => DamageType.Hybrid,
            _ => DamageType.Physical
        };
    }

    #endregion
}
