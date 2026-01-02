using UnityEngine;
using System.Collections.Generic;

// ... (Enums and other structs) ...

[System.Serializable]
public struct StatRandomizer
{
    public string statName;
    [Tooltip("ค่า Min เริ่มต้นที่ Quality 1")]
    public float minBaseValue;
    [Tooltip("ค่า Max เริ่มต้นที่ Quality 1")]
    public float maxBaseValue;
    [Tooltip("ค่า Min/Max ที่จะบวกเพิ่มเข้าไปในทุกๆ 1 Quality Level")]
    public float growthPerQuality;
}

[CreateAssetMenu(fileName = "NewItemData", menuName = "Scriptable Objects/Item Data")]
public class ItemDataSO : ScriptableObject
{
    [Header("Core Info")]
    public string itemID;
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public ItemType itemType;

    [Header("Stacking")]
    public int maxStack = 1;

    [Header("Equipment Stats & Rules")]
    public EquipmentSlot equipmentSlot;
    
    public StatModifier mainStat; 
    public float mainStatBaseValue;
    public float mainStatGrowthPerQuality;
    public List<StatRandomizer> possibleSubStats;

    /// <summary>
    /// ตรวจสอบว่าไอเทมชิ้นนี้เป็นของสวมใส่ในช่องปกติหรือไม่ (ไม่รวม Pet)
    /// </summary>
    public bool IsEquipment()
    {
        return itemType == ItemType.Weapon || 
               itemType == ItemType.Armor || 
               itemType == ItemType.Accessory ||
               itemType == ItemType.Boots;
    }

    /// <summary>
    /// ตรวจสอบว่าไอเทมชิ้นนี้เป็นสัตว์เลี้ยงหรือไม่
    /// </summary>
    public bool IsPet()
    {
        return itemType == ItemType.Pet;
    }

    public float GetMainStatValueForQuality(int qualityLevel)
    {
        return mainStatBaseValue + (mainStatGrowthPerQuality * (qualityLevel - 1));
    }

    public int GetNumberOfSubStatsToRoll(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common: return 1;
            case ItemRarity.Uncommon: return 2;
            case ItemRarity.Rare: return 3;
            case ItemRarity.Epic: return 4;
            case ItemRarity.Legendary: return 4;
            default: return 0;
        }
    }

    public int GetMaxEnchantLevel(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common: return 3;
            case ItemRarity.Uncommon: return 6;
            case ItemRarity.Rare: return 9;
            case ItemRarity.Epic: return 12;
            case ItemRarity.Legendary: return 15;
            default: return 0;
        }
    }
}
