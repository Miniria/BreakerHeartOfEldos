using UnityEngine;
using TMPro;
using UnityEngine.UI; // เพิ่มเพื่อใช้ Slider

public class TopBarUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI gemText;
    public TextMeshProUGUI levelText; // <-- เพิ่ม Text สำหรับ Level
    public Slider expSlider;          // <-- เพิ่ม Slider สำหรับ EXP

    void LateUpdate()
    {
        if (PlayerDataManager.Instance == null || PlayerDataManager.Instance.gameData == null)
        {
            if (goldText != null) goldText.text = "0";
            if (gemText != null) gemText.text = "0";
            if (levelText != null) levelText.text = "Level 1";
            if (expSlider != null) expSlider.value = 0;
            return;
        }

        var playerData = PlayerDataManager.Instance.gameData.playerData;
        var levelTable = PlayerDataManager.Instance.levelTable;

        // --- แสดง Gold และ Gem (เหมือนเดิม) ---
        if (goldText != null)
        {
            goldText.text = playerData.gold.ToString("N0");
        }
        if (gemText != null)
        {
            gemText.text = playerData.gems.ToString("N0");
        }

        // --- เพิ่มการแสดงผล Level ---
        if (levelText != null)
        {
            levelText.text = "Level " + playerData.accountLevel;
        }

        // --- เพิ่มการแสดงผล EXP Slider ---
        if (expSlider != null && levelTable != null)
        {
            long currentExp = playerData.accountExperience;
            long requiredExp = levelTable.GetExperienceForLevel(playerData.accountLevel);

            if (requiredExp > 0)
            {
                // แปลงค่าเป็น float แล้วคำนวณสัดส่วนสำหรับ Slider
                expSlider.value = (float)currentExp / requiredExp;
            }
            else
            {
                // ถ้าเลเวลเต็ม ให้ Slider เต็ม
                expSlider.value = 1f;
            }
        }
    }
}
