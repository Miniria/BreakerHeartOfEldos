using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitStats // <-- เปลี่ยนจาก struct เป็น class
{
    public float health;
    public float attack;
    public float defense;
    public float speed;
    public float critical;
    public float criticalDamage;
    public float resistance;
    public float accurate;
    public float evation;

    // Constructor สำหรับสร้าง Instance ใหม่
    public UnitStats() { }

    // Constructor สำหรับการคัดลอก
    public UnitStats(UnitStats other)
    {
        this.health = other.health;
        this.attack = other.attack;
        this.defense = other.defense;
        this.speed = other.speed;
        this.critical = other.critical;
        this.criticalDamage = other.criticalDamage;
        this.resistance = other.resistance;
        this.accurate = other.accurate;
        this.evation = other.evation;
    }
}

[CreateAssetMenu(fileName = "UnitsDataSO", menuName = "Scriptable Objects/UnitsSO")]
public class UnitsDataSO : ScriptableObject
{
    public int unitID;
    public string unitName;
    public Sprite icon;
    
    [Header("Attributes")] 
    public int strength;
    public int agility;
    public int intelligence;
    
    [Header("Units status")]
    public UnitStats stats = new UnitStats(); // <-- Initialize เพื่อป้องกัน Null
    
    [Header("Skills")]
    public List<SkillDataSO> skills;
    
    [Header("Prefab & VFX")]
    public GameObject unitPrefab;
    public GameObject hitVfxPrefab; // <-- เพิ่ม Prefab สำหรับ Hit VFX
    public WeaponDataSO weapon;

    [Header("Animation")] 
    public bool hasAnimation;
    public AnimationClip idle;
    public AnimationClip attack;
    public AnimationClip gethit;
    public AnimationClip dead;
    
    [Header("Attack Movement")]
    public float dashSpeed = 20f;
    public float attackDistance = 2.5f;
}
