using UnityEngine;
using UnityEngine.UI; // สำหรับ Button
using System.Collections.Generic; // สำหรับ List

public class TestRewardButton : MonoBehaviour
{
    [Header("Reward Settings")]
    public RewardType rewardType = RewardType.Gold;
    public int amount = 100;
    public ItemRarity itemRarity = ItemRarity.Common; // สำหรับ Item Type
    public string itemID = "item_1"; // สำหรับ Item Type
    public int itemQuantity = 1; // สำหรับ Item Type

    // เมธอดนี้จะถูกเรียกเมื่อกดปุ่ม
    public void OnClick_GrantReward()
    {
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogError("PlayerDataManager is not initialized. Cannot grant reward.");
            return;
        }

        // สร้าง RewardItem struct
        RewardItem testReward = new RewardItem
        {
            type = rewardType,
            amount = amount,
            rarity = itemRarity,
            itemID = itemID,
            itemQuantity = itemQuantity
        };

        // สร้าง List เปล่าๆ สำหรับเก็บรางวัลที่ได้รับ (ไม่จำเป็นต้องแสดงผลใน Test นี้)
        List<RewardItem> grantedList = new List<RewardItem>();

        // เรียก RewardSystem เพื่อมอบรางวัล
        RewardSystem.GrantReward(testReward, grantedList);

        Debug.Log($"Granted {rewardType} x {amount} (Rarity: {itemRarity})");

        // (Optional) อัปเดต UI ของ PlayerStatsUI ทันที
        // ถ้า PlayerStatsUI อยู่ใน Scene เดียวกันและเปิดอยู่
        PlayerStatsUI playerStatsUI = FindObjectOfType<PlayerStatsUI>();
        if (playerStatsUI != null && playerStatsUI.gameObject.activeInHierarchy)
        {
            playerStatsUI.UpdateStats();
        }
    }
}
