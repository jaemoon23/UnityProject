using System;
using CsvHelper.Configuration.Attributes;

/// <summary>
/// SupportSkillTable.csv 데이터 클래스
/// CSV에서 서포트 스킬 데이터를 파싱합니다.
/// </summary>
[Serializable]
public class SupportSkillData
{
    [Name("support_id")]
    public int support_id { get; set; }

    [Name("support_category")]
    public int support_category { get; set; }

    [Name("compatible_types")]
    public int compatible_types { get; set; }

    [Name("add_projectiles")]
    public int add_projectiles { get; set; }

    [Name("add_pierce")]
    public int add_pierce { get; set; }

    [Name("damage_mult")]
    public float damage_mult { get; set; }

    [Name("speed_mult")]
    public float speed_mult { get; set; }

    [Name("aoe_mult")]
    public float aoe_mult { get; set; }

    [Name("cooldown_mult")]
    public float cooldown_mult { get; set; }

    [Name("mana_mult")]
    public float mana_mult { get; set; }

    [Name("attack_speed_mult")]
    public float attack_speed_mult { get; set; }

    [Name("cast_time_mult")]
    public float cast_time_mult { get; set; }

    [Name("status_effect")]
    public int status_effect { get; set; }

    [Name("cc_type")]
    public int cc_type { get; set; }

    [Name("cc_duration")]
    public float cc_duration { get; set; }

    [Name("cc_slow_amount")]
    public float cc_slow_amount { get; set; }

    [Name("dot_type")]
    public int dot_type { get; set; }

    [Name("dot_duration")]
    public float dot_duration { get; set; }

    [Name("dot_tick_interval")]
    public float dot_tick_interval { get; set; }

    [Name("dot_damage_per_tick")]
    public float dot_damage_per_tick { get; set; }

    [Name("mark_type")]
    public int mark_type { get; set; }

    [Name("mark_duration")]
    public float mark_duration { get; set; }

    [Name("mark_damage_mult")]
    public float mark_damage_mult { get; set; }

    [Name("chain_count")]
    public int chain_count { get; set; }

    [Name("chain_range")]
    public float chain_range { get; set; }

    [Name("chain_damage_reduction")]
    public float chain_damage_reduction { get; set; }

    // CSV의 //로 시작하는 주석 컬럼 (선택적)
    [Name("//support_name")]
    [Optional]
    public string support_name { get; set; }

    [Name("//description")]
    [Optional]
    public string description { get; set; }

    #region Helper Properties (Enum 변환)

    /// <summary>
    /// support_category를 SupportCategory로 변환
    /// 8001=Projectile, 8002=AOE, 8003=StatusEffect, 8004=Chain
    /// </summary>
    public SupportCategory GetSupportCategory()
    {
        return support_category switch
        {
            8001 => SupportCategory.Projectile,
            8002 => SupportCategory.AOE,
            8003 => SupportCategory.StatusEffect,
            8004 => SupportCategory.Chain,
            _ => SupportCategory.Projectile
        };
    }

    /// <summary>
    /// status_effect를 StatusEffectType으로 변환
    /// 4000=None, 4001=CC, 4002=DOT, 4003=Mark, 4004=Chain
    /// </summary>
    public StatusEffectType GetStatusEffectType()
    {
        return status_effect switch
        {
            4000 => StatusEffectType.None,
            4001 => StatusEffectType.CC,
            4002 => StatusEffectType.DOT,
            4003 => StatusEffectType.Mark,
            4004 => StatusEffectType.Chain,
            _ => StatusEffectType.None
        };
    }

    /// <summary>
    /// cc_type을 CCType으로 변환
    /// 5000=None, 5001=Stun, 5002=Slow, etc.
    /// </summary>
    public CCType GetCCType()
    {
        return cc_type switch
        {
            5000 => CCType.None,
            5001 => CCType.Stun,
            5002 => CCType.Slow,
            5003 => CCType.Root,
            5004 => CCType.Freeze,
            5005 => CCType.Knockback,
            5006 => CCType.Silence,
            _ => CCType.None
        };
    }

    /// <summary>
    /// dot_type을 DOTType으로 변환
    /// 6000=None, 6001=Burn, 6002=Poison, etc.
    /// </summary>
    public DOTType GetDOTType()
    {
        return dot_type switch
        {
            6000 => DOTType.None,
            6001 => DOTType.Burn,
            6002 => DOTType.Poison,
            6003 => DOTType.Bleed,
            6004 => DOTType.Corrosion,
            6005 => DOTType.Curse,
            _ => DOTType.None
        };
    }

    /// <summary>
    /// mark_type을 MarkType으로 변환
    /// 7000=None, 7001=Flame, 7002=Ice, etc.
    /// </summary>
    public MarkType GetMarkType()
    {
        return mark_type switch
        {
            7000 => MarkType.None,
            7001 => MarkType.Flame,
            7002 => MarkType.Ice,
            7003 => MarkType.Lightning,
            7004 => MarkType.Poison,
            7005 => MarkType.Holy,
            7006 => MarkType.Curse,
            7007 => MarkType.Focus,
            _ => MarkType.None
        };
    }

    #endregion
}

/// <summary>
/// 서포트 스킬 카테고리 (SkillEnumTable 기준)
/// </summary>
public enum SupportCategory
{
    Projectile = 1,   // 발사체 변형
    AOE = 2,          // 범위 변형
    StatusEffect = 3, // 상태이상 부여
    Chain = 4         // 연쇄 효과
}
