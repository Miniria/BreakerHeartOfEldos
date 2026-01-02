using UnityEngine;
using System.Collections.Generic;

public enum SkillTargetType
{
    Single_Enemy,
    All_Enemies,
    Single_Ally,
    All_Allies,
    Self,
    Random_Enemy,
    Random_Ally
}

public enum MainEffectType
{
    Damage,
    Heal
}

[CreateAssetMenu(fileName = "NewSkillData", menuName = "Scriptable Objects/SkillDataSO")]
public class SkillDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string skillName;
    [TextArea] public string description;
    public Sprite icon;
    public float cooldown;
    [HideInInspector] public float currentCooldown;

    [Header("Visuals")]
    public AnimationClip skillAnimation;
    public GameObject vfxPrefab; // <-- เพิ่ม Prefab สำหรับ VFX
    public bool spawnVfxOnCaster = true; // true = เกิดที่ผู้ร่าย, false = เกิดที่เป้าหมาย

    [Header("Targeting")]
    public SkillTargetType targetType;

    [Header("Main Effect")]
    public MainEffectType mainEffectType;
    [Tooltip("ค่าพลังหลักของสกิล (สำหรับ Damage หรือ Heal)")]
    public float power;

    [Header("Additional Effects")]
    [Tooltip("เอฟเฟกต์เสริมที่จะทำงานหลังจากเอฟเฟกต์หลัก")]
    public List<SkillEffect> additionalEffects;
}
