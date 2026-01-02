using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

// สคริปต์นี้สำหรับ "ช่องของที่สวมใส่แล้ว" โดยเฉพาะ
public class EquippedSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    public Image rarityBG;
    public Image rarityFrame;
    public Image itemIcon;
    public TextMeshProUGUI gradeText;
    public TextMeshProUGUI enchantText;

    private InventoryItemData _itemData;
    private EquippedItemsPanelUI _ownerPanel; // Reference ไปยัง Panel แม่

    // Setup จะคล้ายกับของ ItemSlotUI แต่รับ Owner เป็น EquippedItemsPanelUI
    public void Setup(InventoryItemData itemData, EquippedItemsPanelUI owner)
    {
        this._itemData = itemData;
        this._ownerPanel = owner;

        ItemDataSO itemSO = GameDatabase.Instance.GetItemByID(itemData.itemID);
        if (itemSO == null)
        {
            gameObject.SetActive(false);
            return;
        }

        // --- ตั้งค่าการแสดงผล (เหมือนกับ ItemSlotUI) ---
        if (itemIcon != null)
        {
            itemIcon.sprite = itemSO.icon;
            itemIcon.enabled = true;
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

        if (itemSO.IsEquipment())
        {
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
        }
        else
        {
            if (gradeText != null) gradeText.gameObject.SetActive(false);
            if (enchantText != null) enchantText.gameObject.SetActive(false);
        }
    }

    // เมื่อถูกกด, จะไปเรียกเมธอดใน EquippedItemsPanelUI โดยตรง
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_ownerPanel != null && _itemData != null)
        {
            _ownerPanel.OnEquippedItemClicked(this._itemData);
        }
    }
}
