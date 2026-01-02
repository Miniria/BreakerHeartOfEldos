using UnityEngine;
using System.Collections.Generic;

public static class ItemFactory
{
    /// <summary>
    /// สร้างไอเทมชิ้นใหม่พร้อมกำหนด Rarity, Quality และสุ่มสถานะ
    /// </summary>
    public static InventoryItemData CreateItem(string itemID, ItemRarity rarity, int quality = 1)
    {
        ItemDataSO itemSO = GameDatabase.Instance.GetItemByID(itemID);
        if (itemSO == null)
        {
            Debug.LogError($"ItemFactory: Cannot find ItemDataSO with ID: {itemID}");
            return null;
        }

        InventoryItemData newItem = new InventoryItemData(itemID, 1);
        newItem.rarity = rarity; // <-- กำหนด Rarity ให้กับไอเทมชิ้นนี้

        if (itemSO.IsEquipment())
        {
            newItem.qualityLevel = quality;
            newItem.enchantLevel = 0;
            newItem.rolledSubStats = new List<StatModifier>();

            // --- สุ่มสถานะรอง โดยใช้ Rarity ที่ได้รับมา ---
            int subStatCount = itemSO.GetNumberOfSubStatsToRoll(rarity); // <-- ส่ง rarity เข้าไป
            List<StatRandomizer> availableStats = new List<StatRandomizer>(itemSO.possibleSubStats);

            for (int i = 0; i < subStatCount && availableStats.Count > 0; i++)
            {
                int randomIndex = Random.Range(0, availableStats.Count);
                StatRandomizer selectedPool = availableStats[randomIndex];
                
                float qualityBonus = selectedPool.growthPerQuality * (quality - 1);
                float finalMinValue = selectedPool.minBaseValue + qualityBonus;
                float finalMaxValue = selectedPool.maxBaseValue + qualityBonus;

                float randomValue = Random.Range(finalMinValue, finalMaxValue);

                StatModifier subStat = new StatModifier
                {
                    statToModify = selectedPool.statName,
                    value = randomValue,
                };
                newItem.rolledSubStats.Add(subStat);
                
                availableStats.RemoveAt(randomIndex);
            }
        }
        
        Debug.Log($"Created new item: {itemSO.itemName} (Rarity: {rarity}, Quality: T{quality})");
        return newItem;
    }
}
