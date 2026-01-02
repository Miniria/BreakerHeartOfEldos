using System.Collections.Generic;
using System.Linq;
using UnityEngine; // เพิ่ม UnityEngine เพื่อใช้ Debug.LogError

public static class StatCalculator
{
    /// <summary>
    /// คำนวณค่าพลังพื้นฐานจาก Attributes (STR, AGI, INT)
    /// </summary>
    public static UnitStats CalculateBaseStatsFromAttributes(int str, int agi, int intel)
    {
        UnitStats stats = new UnitStats();

        // Strength
        stats.health += str * 600;
        stats.defense += str * 30;
        stats.attack += str * 10;

        // Agility
        stats.speed += agi * 10;
        stats.evation += agi * 40;
        stats.accurate += agi * 10;
        stats.criticalDamage += agi * 10;

        // Intelligence
        stats.attack += intel * 60;
        stats.accurate += intel * 30;
        stats.health += intel * 100;

        return stats;
    }

    /// <summary>
    /// คำนวณค่าพลังทั้งหมดที่ได้จากของสวมใส่
    /// </summary>
    public static UnitStats CalculateStatsFromEquipment(EquipmentData equippedItems, List<InventoryItemData> inventory)
    {
        UnitStats equipmentStats = new UnitStats();
        if (equippedItems == null || inventory == null) return equipmentStats;

        List<ulong> equippedIds = new List<ulong>
        {
            equippedItems.weaponInstanceID,
            equippedItems.armorInstanceID,
            equippedItems.bootsInstanceID,
            equippedItems.accessoryInstanceID
        };

        foreach (ulong instanceID in equippedIds)
        {
            if (instanceID == 0) continue;

            InventoryItemData itemData = inventory.FirstOrDefault(item => item.uniqueInstanceID == instanceID);
            if (itemData == null) continue;

            ItemDataSO itemSO = GameDatabase.Instance.GetItemByID(itemData.itemID);
            if (itemSO == null) continue;

            // 1. เพิ่ม Main Stat
            float mainStatValue = itemSO.GetMainStatValueForQuality(itemData.qualityLevel);
            AddStat(equipmentStats, itemSO.mainStat.statToModify, mainStatValue);

            // 2. เพิ่ม Sub-stats ทั้งหมด
            if (itemData.rolledSubStats != null)
            {
                foreach (var subStat in itemData.rolledSubStats)
                {
                    AddStat(equipmentStats, subStat.statToModify, subStat.value);
                }
            }
        }

        return equipmentStats;
    }

    /// <summary>
    /// Helper method สำหรับบวกค่าพลังเข้าไปใน UnitStats
    /// </summary>
    private static void AddStat(UnitStats stats, string statName, float value)
    {
        switch (statName.ToLower())
        {
            case "health": stats.health += value; break;
            case "attack": stats.attack += value; break;
            case "defense": stats.defense += value; break;
            case "speed": stats.speed += value; break;
            case "critical": stats.critical += value; break;
            case "criticaldamage": stats.criticalDamage += value; break;
            case "accurate": stats.accurate += value; break;
            case "evation": stats.evation += value; break;
        }
    }
}
