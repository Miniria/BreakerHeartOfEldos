using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EquippedItemUI : MonoBehaviour
{
    [Header("UI References")]
    public Image background;
    public Image rarityFrame;
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public Image itemTypeImage;
    public TextMeshProUGUI gradeText;
    public TextMeshProUGUI enchantedLevelText;
    public TextMeshProUGUI mainStatText;
    public List<TextMeshProUGUI> subStatTexts;

    public void Show(InventoryItemData itemData)
    {
        ItemDataSO itemSO = GameDatabase.Instance.GetItemByID(itemData.itemID);
        if (itemSO == null) 
        {
            gameObject.SetActive(false);
            return;
        }

        itemIcon.sprite = itemSO.icon;
        // rarityFrame.color = GameDatabase.Instance.GetRarityColor(itemSO.rarity);
        itemNameText.text = itemSO.itemName;
        
        if (itemTypeImage != null)
        {
            itemTypeImage.sprite = GameDatabase.Instance.GetEquipmentTypeIcon(itemSO.equipmentSlot);
        }

        if (itemSO.IsEquipment())
        {
            gradeText.text = itemData.qualityLevel.ToString();
            
            if (itemData.enchantLevel > 0)
            {
                enchantedLevelText.text = $"+{itemData.enchantLevel}";
            }
            else
            {
                enchantedLevelText.text = "";
            }

            float mainStatValue = itemSO.GetMainStatValueForQuality(itemData.qualityLevel);
            mainStatText.text = $"{itemSO.mainStat.statToModify}: {mainStatValue}";

            for (int i = 0; i < subStatTexts.Count; i++)
            {
                if (itemData.rolledSubStats != null && i < itemData.rolledSubStats.Count)
                {
                    var subStat = itemData.rolledSubStats[i];
                    subStatTexts[i].text = $"- {subStat.statToModify}: {subStat.value}";
                    subStatTexts[i].gameObject.SetActive(true);
                }
                else
                {
                    subStatTexts[i].gameObject.SetActive(false);
                }
            }
        }
        else 
        {
            gradeText.text = "";
            enchantedLevelText.text = "";
            mainStatText.text = "";
            foreach(var txt in subStatTexts) { txt.gameObject.SetActive(false); }
        }
    }
}
