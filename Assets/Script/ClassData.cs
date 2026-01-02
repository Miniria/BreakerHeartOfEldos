using UnityEngine;

[CreateAssetMenu(menuName = "Breaker/PlayerClass")]
public class PlayerClassData : ScriptableObject
{
    public string className;

    [Header("Visual")]
    public GameObject classPrefab;

    [Header("Starting Stats")]
    public int baseHP;
    //public int baseMana;
    public int baseAttack;
    public int baseDefense;
    public float baseAttackSpeed;

    [Header("Growth Per Level")]
    public int hpGrowth;
    //public int manaGrowth;
    public int attackGrowth;
    public int defenseGrowth;
    public float attackSpeedGrowth;

    [Header("Allowed Weapons")]
    public WeaponType[] allowedWeaponTypes;

    [Header("Class Skills")]
    public SkillData[] defaultSkills = new SkillData[3];
}

public enum WeaponType { Sword, Bow, Katar, Staff, Mace }
