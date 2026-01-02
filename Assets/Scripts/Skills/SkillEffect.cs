using UnityEngine;

public enum AdditionalEffectType
{
    ApplyStatusEffect,
    // Future possibilities:
    // Dispel,
    // Cleanse,
    // ModifyActionGauge,
    // ExecuteIfTargetHasStatus
}

[System.Serializable]
public class SkillEffect
{
    public AdditionalEffectType effectType;

    [Header("General Settings")]
    [Tooltip("Power สำหรับเอฟเฟกต์นี้โดยเฉพาะ (เช่น % การสะท้อน, % การลดเกจ). ถ้าเป็น 0 จะใช้ Power จากสกิลหลัก")]
    public float power;

    [Header("Status Effect Settings")]
    [Tooltip("SO ของสถานะที่จะมอบให้")]
    public StatusEffectSO statusEffectToApply;
    [Tooltip("โอกาสที่จะติดสถานะ (1-100%)")]
    [Range(1, 100)] public float chanceToApply = 100f;
}
