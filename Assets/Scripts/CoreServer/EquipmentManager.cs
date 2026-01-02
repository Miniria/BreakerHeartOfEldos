using UnityEngine;
using System.Linq;
using System;

public class EquipmentManager : MonoBehaviour
{
    public static event Action OnEquipmentChanged;
    public static EquipmentManager Instance { get; private set; }

    private GameData gameData
    {
        get
        {
            if (_gameData == null && PlayerDataManager.Instance != null)
            {
                _gameData = PlayerDataManager.Instance.gameData;
            }
            return _gameData;
        }
    }
    private GameData _gameData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            PlayerDataManager.OnDataLoaded += Initialize;
        }
    }

    private void OnDestroy()
    {
        PlayerDataManager.OnDataLoaded -= Initialize;
    }

    private void Initialize()
    {
        _gameData = PlayerDataManager.Instance.gameData;
    }

    public void EquipItem(InventoryItemData itemToEquip)
    {
        if (gameData == null || itemToEquip == null) return;

        ItemDataSO itemSO = GameDatabase.Instance.GetItemByID(itemToEquip.itemID);
        if (itemSO == null || !itemSO.IsEquipment()) return;

        // --- แก้ไข Logic การถอดของเก่า ---
        // 1. หาของเก่าที่ใส่อยู่ในช่องเดียวกัน
        InventoryItemData oldItem = GetEquippedItem(itemSO.equipmentSlot);
        
        // 2. ถอดของเก่า (เคลียร์ ID)
        UnequipItem(itemSO.equipmentSlot, false); 

        // 3. สวมใส่ของใหม่
        switch (itemSO.equipmentSlot)
        {
            case EquipmentSlot.Weapon:
                gameData.playerData.equippedItems.weaponInstanceID = itemToEquip.uniqueInstanceID;
                break;
            case EquipmentSlot.Armor:
                gameData.playerData.equippedItems.armorInstanceID = itemToEquip.uniqueInstanceID;
                break;
            case EquipmentSlot.Boots:
                gameData.playerData.equippedItems.bootsInstanceID = itemToEquip.uniqueInstanceID;
                break;
            case EquipmentSlot.Accessory:
                gameData.playerData.equippedItems.accessoryInstanceID = itemToEquip.uniqueInstanceID;
                break;
        }

        Debug.Log($"Equipped: {itemSO.itemName}");
        OnEquipmentChanged?.Invoke();
    }

    public void UnequipItem(EquipmentSlot slot, bool broadCastEvent = true)
    {
        if (gameData == null || slot == EquipmentSlot.None) return;
        
        ulong itemInstanceIDToUnequip = 0;
        switch (slot)
        {
            case EquipmentSlot.Weapon:
                itemInstanceIDToUnequip = gameData.playerData.equippedItems.weaponInstanceID;
                gameData.playerData.equippedItems.weaponInstanceID = 0;
                break;
            case EquipmentSlot.Armor:
                itemInstanceIDToUnequip = gameData.playerData.equippedItems.armorInstanceID;
                gameData.playerData.equippedItems.armorInstanceID = 0;
                break;
            case EquipmentSlot.Boots:
                itemInstanceIDToUnequip = gameData.playerData.equippedItems.bootsInstanceID;
                gameData.playerData.equippedItems.bootsInstanceID = 0;
                break;
            case EquipmentSlot.Accessory:
                itemInstanceIDToUnequip = gameData.playerData.equippedItems.accessoryInstanceID;
                gameData.playerData.equippedItems.accessoryInstanceID = 0;
                break;
        }

        if (itemInstanceIDToUnequip != 0 && broadCastEvent)
        {
            Debug.Log($"Unequipped item from slot: {slot}");
            OnEquipmentChanged?.Invoke();
        }
    }

    /// <summary>
    /// หาข้อมูลไอเทมที่สวมใส่อยู่จาก Slot ที่ระบุ
    /// </summary>
    public InventoryItemData GetEquippedItem(EquipmentSlot slot)
    {
        if (gameData == null) return null;

        var equippedIDs = gameData.playerData.equippedItems;
        ulong targetInstanceID = 0;

        switch (slot)
        {
            case EquipmentSlot.Weapon: targetInstanceID = equippedIDs.weaponInstanceID; break;
            case EquipmentSlot.Armor: targetInstanceID = equippedIDs.armorInstanceID; break;
            case EquipmentSlot.Boots: targetInstanceID = equippedIDs.bootsInstanceID; break;
            case EquipmentSlot.Accessory: targetInstanceID = equippedIDs.accessoryInstanceID; break;
        }

        if (targetInstanceID == 0) return null;

        return gameData.playerData.inventory.FirstOrDefault(item => item.uniqueInstanceID == targetInstanceID);
    }

    public void AddItem(InventoryItemData item)
    {
        if (gameData == null || item == null) return;
        gameData.playerData.inventory.Add(item);
    }

    public void RemoveItem(InventoryItemData item)
    {
        if (gameData == null || item == null) return;
        gameData.playerData.inventory.Remove(item);
    }
}
