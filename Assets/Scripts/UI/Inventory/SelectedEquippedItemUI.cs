using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SelectedEquippedItemUI : MonoBehaviour
{
    [Header("UI References")]
    public Image background; // <-- เพิ่ม Background
    public Image rarityFrame;
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public Image itemTypeImage;
    public TextMeshProUGUI gradeText;
    public TextMeshProUGUI enchantedLevelText;
    public TextMeshProUGUI mainStatText;
    public List<TextMeshProUGUI> subStatTexts;

    [Header("Action Buttons")]
    public Button unequipButton;
    public Button closeButton;

    private InventoryItemData currentItem;
    private ItemDataSO currentItemSO;

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
        this.currentItemSO = GameDatabase.Instance.GetItemByID(itemData.itemID);

        if (currentItemSO == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        itemIcon.sprite = currentItemSO.icon;
        itemNameText.text = currentItemSO.itemName;

        Color rarityColor = GameDatabase.Instance.GetRarityColor(currentItem.rarity);
        if (rarityFrame != null) rarityFrame.color = rarityColor;
        
        // --- เพิ่ม Logic การปรับสีพื้นหลัง ---
        if (background != null)
        {
            Color bgColor = rarityColor;
            bgColor.a = 0.1f; // ทำให้จางลง
            background.color = bgColor;
        }
        // ------------------------------------

        if (itemTypeImage != null)
        {
            itemTypeImage.sprite = GameDatabase.Instance.GetEquipmentTypeIcon(currentItemSO.equipmentSlot);
        }

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

        // --- แก้ไขการแสดงผล Main Stat และ Sub Stat ให้ใช้ชื่อย่อ ---
        float mainStatValue = currentItemSO.GetMainStatValueForQuality(itemData.qualityLevel);
        string mainStatName = GetStatAbbreviation(currentItemSO.mainStat.statToModify);
        mainStatText.text = $"{mainStatName}: {mainStatValue}";

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
        // ---------------------------------------------------------

        if (unequipButton != null)
        {
            unequipButton.onClick.RemoveAllListeners();
            unequipButton.onClick.AddListener(OnUnequipClicked);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnUnequipClicked()
    {
        if (currentItem == null || currentItemSO == null) return;
        EquipmentManager.Instance.UnequipItem(currentItemSO.equipmentSlot);
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
            default: return statName.ToUpper();
        }
    }
}
