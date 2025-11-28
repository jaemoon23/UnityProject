using System;
using UnityEngine;

/// <summary>
/// 스킬 관련 Enum 정의
/// CSV의 SkillEnumTable과 매핑됩니다
/// </summary>

/// <summary>
/// 스킬 에셋 타입 (MainSkillTable의 skill_type)
/// </summary>
public enum SkillAssetType
{
    Projectile,     // 투사체 (1000)
    AOE,            // 범위 공격 (1001)
    DOT,            // 지속 데미지 (1002)
    Buff,           // 버프 (1003)
    Debuff,         // 디버프 (1004)
    Heal,           // 힐 (1005)
    Summon,         // 소환 (1006)
    Teleport,       // 순간이동 (1007)
    Dash,           // 돌진 (1008)
    Flicker,        // 플리커 스트라이크 (1009)
    Channeling,     // 채널링 (1010)
    Trap,           // 트랩 (1011)
    Mine,           // 지뢰 (1012)
    Aura,           // 오라 (1013)
    Shield,         // 보호막 (1014)
    Pull,           // 끌어당기기
    Push,           // 밀어내기
    Stun,           // 스턴
    Slow,           // 슬로우
    Root,           // 속박
}

/// <summary>
/// 속성 타입 (MainSkillTable의 element_type)
/// </summary>
public enum ElementType
{
    None,       // 무속성 (2000)
    Fire,       // 화염 (2001)
    Ice,        // 냉기 (2002)
    Lightning,  // 번개 (2003)
    Poison,     // 독 (2004)
    Holy,       // 신성 (2005)
    Dark,       // 암흑 (2006)
    Nature,     // 자연 (2007)
    Arcane,     // 비전 (2008)
}

/// <summary>
/// 데미지 타입 (MainSkillTable의 damage_type)
/// </summary>
public enum DamageType
{
    Physical,   // 물리 (3000)
    Magical,    // 마법 (3001)
    Pure,       // 순수 (3002)
    Hybrid,     // 혼합 (3003)
}

/// <summary>
/// 상태 이상 효과 타입 (SupportSkillTable의 status_effect)
/// </summary>
public enum StatusEffectType
{
    None,   // 효과 없음 (4000)
    CC,     // 군중 제어 (4001)
    DOT,    // 지속 데미지 (4002)
    Mark,   // 표식 (4003)
    Chain,  // 연쇄 공격 (4004)
}

/// <summary>
/// CC (군중 제어) 타입 (SupportSkillTable의 cc_type)
/// </summary>
public enum CCType
{
    None,       // 없음 (5000)
    Stun,       // 스턴 (5001)
    Slow,       // 슬로우 (5002)
    Root,       // 속박 (5003)
    Freeze,     // 빙결 (5004)
    Knockback,  // 넉백 (5005)
    Silence,    // 침묵 (5006)
}

/// <summary>
/// DOT (지속 데미지) 타입 (SupportSkillTable의 dot_type)
/// </summary>
public enum DOTType
{
    None,       // 없음 (6000)
    Burn,       // 화상 (6001)
    Poison,     // 중독 (6002)
    Bleed,      // 출혈 (6003)
    Corrosion,  // 부식 (6004)
    Curse,      // 저주 (6005)
}

/// <summary>
/// Mark (표식) 타입 (SupportSkillTable의 mark_type)
/// </summary>
public enum MarkType
{
    None,       // 없음 (7000)
    Flame,      // 화염 표식 (7001)
    Ice,        // 빙결 표식 (7002)
    Lightning,  // 번개 표식 (7003)
    Poison,     // 독 표식 (7004)
    Holy,       // 신성 표식 (7005)
    Curse,      // 저주 표식 (7006)
    Focus,      // 집중 표식 (7007)
}

/// <summary>
/// 스킬 카테고리
/// </summary>
public enum SkillCategory
{
    Main,       // 메인 스킬
    Support,    // 보조 스킬
}

/// <summary>
/// 스탯 타입 (Buff/Debuff용)
/// </summary>
public enum StatType
{
    AttackSpeed,
    MoveSpeed,
    Damage,
    Defense,
    MaxHealth,
    MaxMana,
    CriticalChance,
    CriticalDamage,
}

/// <summary>
/// 스탯 수정자 타입
/// </summary>
public enum ModifierType
{
    Flat,        // 고정값
    Percentage,  // %
}

/// <summary>
/// 스탯 수정자 (Buff/Debuff용)
/// </summary>
[Serializable]
public class StatModifier
{
    public StatType statType;
    public ModifierType modifierType;
    public float value;
}
