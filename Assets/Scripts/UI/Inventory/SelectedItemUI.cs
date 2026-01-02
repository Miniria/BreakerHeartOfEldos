using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SelectedItemUI : MonoBehaviour
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

    [Header("Action Buttons")]
    public Button equipButton;
    public Button enchantButton;
    public Button closeButton;

    private InventoryItemData currentItem;

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }
    }

        public void Show(InventoryItemData itemData)
    {
        this.currentItem = itemData;
        
        ItemDataSO itemSO = GameDatabase.Instance.GetItemByID(itemData.itemID);
        if (itemSO == null) 
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        itemIcon.sprite = itemSO.icon;
        itemNameText.text = itemSO.itemName;
        
        Color rarityColor = GameDatabase.Instance.GetRarityColor(itemData.rarity);
        if (rarityFrame != null) rarityFrame.color = rarityColor;

        if (itemTypeImage != null)
        {
            itemTypeImage.sprite = GameDatabase.Instance.GetEquipmentTypeIcon(itemSO.equipmentSlot);
        }

        if (itemSO.IsEquipment())
        {
            gradeText.text = $"T{itemData.qualityLevel}";
            
            if (itemData.enchantLevel > 0)
            {
                enchantedLevelText.text = $"+{itemData.enchantLevel}";
                enchantedLevelText.gameObject.SetActive(true);
            }
            else
            {
                enchantedLevelText.gameObject.SetActive(false);
            }

            // --- แก้ไขการแสดงผล Main Stat ---
            float mainStatValue = itemSO.GetMainStatValueForQuality(itemData.qualityLevel);
            string mainStatName = GetStatAbbreviation(itemSO.mainStat.statToModify);
            mainStatText.text = $"{mainStatName}: {mainStatValue}";
            // --------------------------------

            // --- แก้ไขการแสดงผล Sub Stat ---
            for (int i = 0; i < subStatTexts.Count; i++)
            {
                if (itemData.rolledSubStats != null && i < itemData.rolledSubStats.Count)
                {
                    var subStat = itemData.rolledSubStats[i];
                    string subStatName = GetStatAbbreviation(subStat.statToModify);
                    subStatTexts[i].text = $"- {subStatName}: {subStat.value:F1}";
                    subStatTexts[i].gameObject.SetActive(true);
                }
                else
                {
                    subStatTexts[i].gameObject.SetActive(false);
                }
            }
            equipButton.gameObject.SetActive(true);
        }
        else 
        {
            gradeText.text = "";
            enchantedLevelText.gameObject.SetActive(false);
            mainStatText.text = "";
            foreach(var txt in subStatTexts) { txt.gameObject.SetActive(false); }
            equipButton.gameObject.SetActive(false);
        }

        if (equipButton != null)
        {
            equipButton.onClick.RemoveAllListeners();
            equipButton.onClick.AddListener(OnEquipClicked);
        }
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnEquipClicked()
    {
        if (currentItem == null) return;

        EquipmentManager.Instance.EquipItem(this.currentItem);
        Hide();
    }
    
    /// <summary>
    /// แปลงชื่อสถานะเต็มเป็นชื่อย่อสำหรับแสดงผล
    /// </summary>
    private string GetStatAbbreviation(string statName)
    {
        if (string.IsNullOrEmpty(statName)) return "";

        switch (statName.ToLower())
        {
            case "health": return "HP";
            case "attack": return "ATK";
            case "defense": return "DEF";
            case "speed": return "SPD";
            case "critical": return "C.Rate";
            case "criticaldamage": return "C.DMG";
            case "accurate": return "ACC";
            case "evation": return "EVA";
            default: return statName.ToUpper(); // ถ้าไม่รู้จัก ให้แสดงเป็นตัวพิมพ์ใหญ่
        }
    }
}
