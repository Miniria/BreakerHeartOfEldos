using UnityEngine;
using System.Collections.Generic;

public enum RewardType { Gold, Gem, Item, Experience }

[System.Serializable]
public struct RewardItem
{
    public RewardType type;
    public ItemRarity rarity;
    public int amount; 
    public int maxAmount;
    public string itemID;
    public int itemQuantity;
}

[System.Serializable]
public struct RandomLootItem
{
    public string itemID;
    [Range(0, 100)] public float dropChance;
}

public static class RewardSystem
{
    public static List<RewardItem> GrantStageRewards(Stage completedStage)
    {
        if (completedStage == null) return new List<RewardItem>();

        List<RewardItem> grantedRewards = new List<RewardItem>();

        bool isFirstClear = !PlayerDataManager.Instance.IsStageCleared(completedStage.name);

        if (isFirstClear)
        {
            Debug.Log("--- Granting First-Time Clear Rewards ---");
            foreach (var reward in completedStage.firstTimeRewards)
            {
                GrantReward(reward, grantedRewards);
            }
            PlayerDataManager.Instance.SetStageAsCleared(completedStage.name);
        }

        Debug.Log("--- Granting Guaranteed Rewards ---");
        foreach (var reward in completedStage.guaranteedRewards)
        {
            GrantReward(reward, grantedRewards);
        }

        Debug.Log("--- Rolling for Random Drops ---");
        foreach (var loot in completedStage.randomDrops)
        {
            if (Random.Range(0, 100f) <= loot.dropChance)
            {
                ItemRarity rolledRarity = RollForRarity();
                int rolledQuality = Random.Range(completedStage.minItemQuality, completedStage.maxItemQuality + 1); 

                InventoryItemData newItem = ItemFactory.CreateItem(loot.itemID, rolledRarity, rolledQuality);
                
                if (newItem != null)
                {
                    EquipmentManager.Instance.AddItem(newItem);
                    grantedRewards.Add(new RewardItem { type = RewardType.Item, itemID = loot.itemID, itemQuantity = 1, rarity = rolledRarity });
                }
            }
        }
        
        return grantedRewards;
    }

    public static void GrantReward(RewardItem reward, List<RewardItem> grantedList) // <-- เปลี่ยนเป็น public
    {
        int finalAmount = (reward.maxAmount > reward.amount) ? Random.Range(reward.amount, reward.maxAmount + 1) : reward.amount;
        
        switch (reward.type)
        {
            case RewardType.Gold:
                PlayerDataManager.Instance.gameData.playerData.gold += finalAmount;
                grantedList.Add(new RewardItem { type = RewardType.Gold, amount = finalAmount });
                break;
            case RewardType.Gem:
                PlayerDataManager.Instance.gameData.playerData.gems += finalAmount;
                grantedList.Add(new RewardItem { type = RewardType.Gem, amount = finalAmount });
                break;
            case RewardType.Experience:
                // PlayerDataManager.Instance.AddExperience(finalAmount); 
                grantedList.Add(new RewardItem { type = RewardType.Experience, amount = finalAmount });
                break;
            case RewardType.Item:
                InventoryItemData newItem = ItemFactory.CreateItem(reward.itemID, ItemRarity.Common, 1);
                if (newItem != null)
                {
                    newItem.quantity = reward.itemQuantity > 0 ? reward.itemQuantity : 1;
                    EquipmentManager.Instance.AddItem(newItem);
                    grantedList.Add(reward);
                }
                break;
        }
    }

    private static ItemRarity RollForRarity()
    {
        float roll = Random.Range(0f, 100f);
        if (roll <= 1f) return ItemRarity.Legendary;
        if (roll <= 5f) return ItemRarity.Epic;
        if (roll <= 15f) return ItemRarity.Rare;
        if (roll <= 40f) return ItemRarity.Uncommon;
        return ItemRarity.Common;
    }
}
