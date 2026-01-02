using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Stat Texts")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI spdText;
    public TextMeshProUGUI critRateText;
    public TextMeshProUGUI critDmgText;
    public TextMeshProUGUI accText;
    public TextMeshProUGUI evaText;

    // ไม่ต้องมี playerUnit อีกต่อไป
    // private Unit playerUnit;

    private void OnEnable()
    {
        UpdateStats();
    }

    private void LateUpdate()
    {
        UpdateStats();
    }

    /// <summary>
    /// ดึงค่าสถานะล่าสุดของผู้เล่นมาแสดงผล
    /// </summary>
    public void UpdateStats()
    {
        // ตรวจสอบว่า PlayerDataManager พร้อมใช้งานหรือไม่
        if (PlayerDataManager.Instance == null || PlayerDataManager.Instance.currentPlayerStats == null)
        {
            // อาจจะแสดงค่าเริ่มต้นหรือซ่อน UI ไปก่อน
            if (hpText != null) hpText.text = "0";
            // ... (ทำแบบเดียวกันกับ Text อื่นๆ) ...
            return;
        }

        // ดึงค่า stats มาจาก PlayerDataManager โดยตรง
        UnitStats stats = PlayerDataManager.Instance.currentPlayerStats;

        // นำค่าไปใส่ใน Text แต่ละช่อง
        if (hpText != null) hpText.text = stats.health.ToString("F0");
        if (atkText != null) atkText.text = stats.attack.ToString("F0");
        if (defText != null) defText.text = stats.defense.ToString("F0");
        if (spdText != null) spdText.text = stats.speed.ToString("F0");
        if (critRateText != null) critRateText.text = stats.critical.ToString("F1") + "%";
        if (critDmgText != null) critDmgText.text = stats.criticalDamage.ToString("F1") + "%";
        if (accText != null) accText.text = stats.accurate.ToString("F1") + "%";
        if (evaText != null) evaText.text = stats.evation.ToString("F1") + "%";
    }
}
