using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GachaResultSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image rarityBG;
    public Image rarityFrame;
    public Image itemIcon;
    public TextMeshProUGUI gradeText;
    public TextMeshProUGUI enchantText;

    public void Setup(InventoryItemData itemData)
    {
        ItemDataSO itemSO = GameDatabase.Instance.GetItemByID(itemData.itemID);
        if (itemSO == null)
        {
            gameObject.SetActive(false);
            return;
        }

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

        if (itemSO.IsEquipment() || itemSO.IsPet())
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
}
