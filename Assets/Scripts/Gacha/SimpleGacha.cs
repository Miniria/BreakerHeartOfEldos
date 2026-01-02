using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SimpleGacha : MonoBehaviour
{
    public static SimpleGacha Instance { get; private set; }

    [Header("Gacha Settings")]
    public List<ItemDataSO> gachaPool;
    public int costPerRoll = 100;

    [Header("Result Display (Optional)")]
    public GameObject resultPanel;
    public Transform resultItemContainer;
    public GameObject gachaResultSlotPrefab; // <-- เปลี่ยนชื่อ Prefab
    public Button closeResultButton;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        if (closeResultButton != null)
        {
            closeResultButton.onClick.AddListener(CloseResultPanel);
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    public void RollGachaSingle()
    {
        RollGachaMultiple(1);
    }

    public void RollGachaMultiple(int amount)
    {
        if (gachaPool == null || gachaPool.Count == 0)
        {
            Debug.LogError("Gacha pool is not set or is empty!");
            return;
        }

        long totalCost = (long)costPerRoll * amount;
        if (PlayerDataManager.Instance.gameData.playerData.gems < totalCost)
        {
            Debug.Log("Not enough gems for multiple rolls!");
            return;
        }

        PlayerDataManager.Instance.gameData.playerData.gems -= totalCost;
        Debug.Log($"Spent {totalCost} gems. Remaining: {PlayerDataManager.Instance.gameData.playerData.gems}");

        List<InventoryItemData> results = new List<InventoryItemData>();
        for (int i = 0; i < amount; i++)
        {
            ItemDataSO randomItemSO = gachaPool[Random.Range(0, gachaPool.Count)];
            int rolledQuality = Random.Range(5, 21);
            ItemRarity rolledRarity = GetRarityFromQuality(rolledQuality);
            InventoryItemData newItem = ItemFactory.CreateItem(randomItemSO.itemID, rolledRarity, rolledQuality);
            
            if (newItem != null)
            {
                EquipmentManager.Instance.AddItem(newItem);
                results.Add(newItem);
                Debug.Log($"<color=cyan>Gacha Result {i+1}:</color> {randomItemSO.itemName} (T{rolledQuality}, {rolledRarity})");
            }
        }

        ShowResult(results);
    }

    private ItemRarity GetRarityFromQuality(int quality)
    {
        if (quality >= 18) return ItemRarity.Legendary;
        if (quality >= 15) return ItemRarity.Epic;
        if (quality >= 10) return ItemRarity.Rare;
        if (quality >= 5) return ItemRarity.Uncommon;
        return ItemRarity.Common;
    }

    private void ShowResult(List<InventoryItemData> items)
    {
        if (resultPanel == null || resultItemContainer == null || gachaResultSlotPrefab == null) return;

        resultPanel.SetActive(true);

        foreach (Transform child in resultItemContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in items)
        {
            // --- แก้ไข Logic การสร้าง Slot ---
            GameObject slotGO = Instantiate(gachaResultSlotPrefab, resultItemContainer);
            GachaResultSlotUI slotUI = slotGO.GetComponent<GachaResultSlotUI>();
            if (slotUI != null)
            {
                slotUI.Setup(item); 
            }
        }
    }

    public void CloseResultPanel()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }
}
