using UnityEngine;

// Enum ใหม่สำหรับระบุพฤติกรรมพิเศษของสถานะ
public enum EffectBehaviorType
{
    None,
    StatModifier,
    DamageOverTime,
    Reflection,
    Stun,
    Silence
}

public enum StatModifierType
{
    Additive,
    Percentage
}

[System.Serializable]
public struct StatModifier
{
    public string statToModify;
    public StatModifierType type;
    public float value;
}

[CreateAssetMenu(fileName = "NewStatusEffect", menuName = "Scriptable Objects/StatusEffectSO")]
public class StatusEffectSO : ScriptableObject
{
    [Header("Basic Info")]
    public string effectName;
    public bool isBuff; // true = สถานะด้านบวก, false = สถานะด้านลบ
    public Sprite icon;
    public int durationInTurns;

    [Header("Visuals")]
    public GameObject vfxPrefab; // <-- เพิ่ม Prefab สำหรับ VFX ของสถานะ

    [Header("Behavior")]
    public EffectBehaviorType behaviorType;

    [Header("Reflection Settings")]
    [Tooltip("เปอร์เซ็นต์การสะท้อนดาเมจ (จะถูกกำหนดโดย Power ของสกิลที่ใช้)")]
    public float reflectionPercentage;

    [Header("Stat Modifier Settings")]
    public StatModifier[] statModifiers;

    [Header("Damage Over Time Settings")]
    public bool dealsDamageOverTime;
    public float damagePerTurn;
}
