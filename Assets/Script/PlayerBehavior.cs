using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    private PlayerUnitData data;

    public void Initialize(PlayerUnitData unitData)
    {
        data = unitData;
        Debug.Log($"{data.characterName} the {data.classData.className} spawned! HP: {data.maxHP}");
    }

    public void TakeDamage(int amount)
    {
        data.maxHP -= amount; // Replace with real HP system
        if (data.maxHP <= 0)
        {
            Debug.Log($"{data.characterName} has fallen!");
        }
    }

    public float GetAttackSpeed()
    {
        return data.attackSpeed;
    }

    public int GetAttackPower()
    {
        return data.attack + data.weaponData.attack;
    }

    public float GetWeaponCooldown()
    {
        return data.weaponData != null ? data.weaponData.baseCooldown : 1.0f;
    }

    public float GetWeaponMinCooldown()
    {
        return data.weaponData != null ? data.weaponData.minCooldown : 0.5f;
    }

    public string GetCharacterName()
    {
        return data.characterName;
    }
}
