[System.Serializable]
public class PlayerUnitData
{
    public string characterName;
    public PlayerClassData classData;
    public WeaponData weaponData;
    public int level;

    // These are calculated from class + level
    public int maxHP;
    public int mana;
    public int attack;
    public int defense;
    public float attackSpeed;

    public float weaponCooldown;
    public float weaponMinCooldown;

    public void InitializeStats()
    {
        maxHP = classData.baseHP + classData.hpGrowth * (level - 1);
        //mana = classData.baseMana + classData.manaGrowth * (level - 1);
        attack = classData.baseAttack + classData.attackGrowth * (level - 1);
        defense = classData.baseDefense + classData.defenseGrowth * (level - 1);
        attackSpeed = classData.baseAttackSpeed + classData.attackSpeedGrowth * (level - 1);

        weaponCooldown = weaponData.baseCooldown;
        weaponMinCooldown = weaponData.minCooldown;
    }
}
