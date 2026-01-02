using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData
{
    public string username;
    public string password;
    public long lastLogoutTime;

    public string playerUnitID;
    public int accountLevel;
    public long accountExperience;
    public long gold;
    public long gems;
    public List<string> clearedStageIDs;

    public List<InventoryItemData> inventory;
    public EquipmentData equippedItems;

    public PlayerData()
    {
        inventory = new List<InventoryItemData>();
        equippedItems = new EquipmentData();
        clearedStageIDs = new List<string>();
        accountLevel = 1;
        gold = 0;
        gems = 0;
    }
}

[System.Serializable]
public class InventoryItemData
{
    public string itemID;
    public ulong uniqueInstanceID;
    public int quantity;

    public ItemRarity rarity;
    public int qualityLevel;
    public int enchantLevel;
    public List<StatModifier> rolledSubStats; 

    public InventoryItemData(string id, int qty = 1)
    {
        itemID = id;
        quantity = qty;
        uniqueInstanceID = GenerateUniqueId();
    }

    private ulong GenerateUniqueId()
    {
        return (ulong)DateTime.UtcNow.Ticks + (ulong)new System.Random().Next(0, 9999);
    }
}


[System.Serializable]
public class EquipmentData
{
    public ulong weaponInstanceID;
    public ulong armorInstanceID;
    public ulong bootsInstanceID; // <-- เพิ่ม Boots
    public ulong accessoryInstanceID;
}

// คลาสหลักที่รวมข้อมูลทุกอย่างไว้ด้วยกัน
[System.Serializable]
public class GameData
{
    public PlayerData playerData;

    public GameData()
    {
        playerData = new PlayerData();
    }
}
