using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    public Image rarityBG;
    public Image rarityFrame;
    public Image itemIcon;
    public TextMeshProUGUI gradeText;
    public TextMeshProUGUI enchantText;

    private InventoryItemData _itemData;
    private Action<InventoryItemData> _onClickAction;

    public void Setup(InventoryItemData itemData, Action<InventoryItemData> onClickAction)
    {
        this._itemData = itemData;
        this._onClickAction = onClickAction;

        ItemDataSO itemSO = GameDatabase.Instance.GetItemByID(itemData.itemID);
        if (itemSO == null)
        {
            Debug.LogError($"Could not find ItemDataSO with ID: {itemData.itemID}");
            gameObject.SetActive(false);
            return;
        }

        if (itemIcon != null)
        {
            itemIcon.sprite = itemSO.icon;
            itemIcon.enabled = true;
        }

        // ดึง Rarity จาก itemData (ข้อมูลของไอเทมชิ้นนี้จริงๆ)
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_onClickAction != null && _itemData != null)
        {
            _onClickAction.Invoke(this._itemData);
        }
    }
}
