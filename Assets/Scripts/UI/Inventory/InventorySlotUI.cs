using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image rarityFrame;
    public Image rarityBG;
    public Image itemIcon;
    public TextMeshProUGUI gradeText;
    public TextMeshProUGUI enchantText;
    public Button slotButton;

    private InventoryItemData currentItem;
    private Action<InventoryItemData> onSlotClicked;

    public void Setup(InventoryItemData itemData, Action<InventoryItemData> clickAction)
    {
        // --- DEBUGGING ---
        Debug.Log($"--- Setting up Slot for item: {itemData.itemID} ---");
        Debug.Log($"Is itemIcon null? : {(itemIcon == null)}");
        Debug.Log($"Is rarityFrame null? : {(rarityFrame == null)}");
        Debug.Log($"Is gradeText null? : {(gradeText == null)}");
        // -----------------

        currentItem = itemData;
        onSlotClicked = clickAction;

        ItemDataSO itemSO = GameDatabase.Instance.GetItemByID(itemData.itemID);
        if (itemSO == null)
        {
            Debug.LogError($"Cannot find ItemDataSO for itemID: {itemData.itemID}");
            gameObject.SetActive(false);
            return;
        }

        if (itemIcon != null)
        {
            itemIcon.sprite = itemSO.icon;
        }
        else
        {
            Debug.LogError("Setup failed: itemIcon reference is missing!");
        }

        Color rarityColor = GameDatabase.Instance.GetRarityColor(itemData.rarity);
        if (rarityFrame != null)
        {
            rarityFrame.color = rarityColor;
        }
        if (rarityBG != null)
        {
            rarityColor.a = 0.3f; 
            rarityBG.color = rarityColor;
        }

        if (gradeText != null)
        {
            gradeText.text = $"T{itemData.qualityLevel}";
            gradeText.gameObject.SetActive(itemData.qualityLevel > 0);
        }

        if (enchantText != null)
        {
            if (itemData.enchantLevel > 0)
            {
                enchantText.text = $"+{itemData.enchantLevel}";
                enchantText.gameObject.SetActive(true);
            }
            else
            {
                enchantText.gameObject.SetActive(false);
            }
        }

        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(OnSlotClicked);
        }
    }

    private void OnSlotClicked()
    {
        onSlotClicked?.Invoke(currentItem);
    }
}
