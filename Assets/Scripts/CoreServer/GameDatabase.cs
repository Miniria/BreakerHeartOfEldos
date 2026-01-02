using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "GameDatabase", menuName = "GameData/Game Database")]
public class GameDatabase : ScriptableObject
{
    [Header("Global Icons")]
    public Sprite goldIcon;
    public Sprite gemIcon;
    public Sprite expIcon;

    [Header("Equipment Type Icons")]
    public Sprite weaponTypeIcon;
    public Sprite armorTypeIcon;
    public Sprite accessoryTypeIcon;

    [Header("Rarity Colors")]
    public Color commonColor = Color.gray;
    public Color uncommonColor = Color.green;
    public Color rareColor = Color.blue;
    public Color epicColor = Color.magenta;
    public Color legendaryColor = Color.yellow;

    [Header("Databases")]
    public List<UnitsDataSO> allUnits;
    public List<Stage> allStages;
    public List<ItemDataSO> allItems;

    private static GameDatabase _instance;
    public static GameDatabase Instance
    {
        get
        {
            if (_instance == null) Initialize();
            return _instance;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        if (_instance == null)
        {
            _instance = Resources.Load<GameDatabase>("GameDatabase");
            if (_instance == null)
            {
                Debug.LogError("GameDatabase could not be loaded from Resources folder.");
            }
        }
    }

    public UnitsDataSO GetUnitSOByID(string unitID)
    {
        if (allUnits == null)
        {
            Debug.LogError("[GameDatabase] 'allUnits' list is not initialized.");
            return null;
        }
        return allUnits.FirstOrDefault(u => u.unitID.ToString() == unitID);
    }

    public Stage GetStageByID(string stageID)
    {
        if (allStages == null)
        {
            Debug.LogError("[GameDatabase] 'allStages' list is not initialized. Please assign stages in the GameDatabase asset.");
            return null;
        }
        if (string.IsNullOrEmpty(stageID))
        {
            Debug.LogWarning("[GameDatabase] GetStageByID was called with a null or empty stageID.");
            return null;
        }
        return allStages.FirstOrDefault(s => s.stageID == stageID);
    }

    public ItemDataSO GetItemByID(string itemID)
    {
        if (allItems == null)
        {
            Debug.LogError("[GameDatabase] 'allItems' list is not initialized.");
            return null;
        }
        return allItems.FirstOrDefault(i => i.itemID == itemID);
    }

    public Color GetRarityColor(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common: return commonColor;
            case ItemRarity.Uncommon: return uncommonColor;
            case ItemRarity.Rare: return rareColor;
            case ItemRarity.Epic: return epicColor;
            case ItemRarity.Legendary: return legendaryColor;
            default: return Color.white;
        }
    }

    public Sprite GetEquipmentTypeIcon(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.Weapon: return weaponTypeIcon;
            case EquipmentSlot.Armor: return armorTypeIcon;
            case EquipmentSlot.Accessory: return accessoryTypeIcon;
            default: return null;
        }
    }
}
