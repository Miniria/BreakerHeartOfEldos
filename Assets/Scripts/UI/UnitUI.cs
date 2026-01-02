using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UnitUI : MonoBehaviour
{
    public Unit targetUnit;

    [Header("UI Elements")]
    public Image unitIconImage; // <-- เพิ่มตัวแปรสำหรับไอคอน
    public Slider hpSlider;
    public Slider actionSlider;

    [Header("Status Effect Display")]
    public GameObject statusIconPrefab;
    public Transform statusIconContainer;

    private List<GameObject> activeStatusIcons = new List<GameObject>();

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void InitializeUI()
    {
        if (targetUnit == null)
        {
            Debug.LogError("UnitUI has no target unit assigned! UI will remain hidden.", this.gameObject);
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        // --- ตั้งค่าไอคอนยูนิต ---
        if (unitIconImage != null && targetUnit.unitData != null && targetUnit.unitData.icon != null)
        {
            unitIconImage.sprite = targetUnit.unitData.icon;
        }
        // -------------------------

        // Subscribe to all relevant events
        targetUnit.OnHealthChanged += UpdateHealthUI;
        targetUnit.OnActionGaugeChanged += UpdateActionGaugeUI;
        targetUnit.OnStatusEffectsChanged += UpdateStatusIcons;

        // Initial UI setup
        UpdateHealthUI(targetUnit.currentStats.health, targetUnit.currentStats.health);
        UpdateActionGaugeUI(targetUnit.ActionGauge, BaseUnit.DefaultMaxActionGauge);
        UpdateStatusIcons();
    }

    private void OnDisable()
    {
        if (targetUnit != null)
        {
            targetUnit.OnHealthChanged -= UpdateHealthUI;
            targetUnit.OnActionGaugeChanged -= UpdateActionGaugeUI;
            targetUnit.OnStatusEffectsChanged -= UpdateStatusIcons;
        }
    }

    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHealth;
            hpSlider.value = currentHealth;
        }
    }

    private void UpdateActionGaugeUI(float currentGauge, float maxGauge)
    {
        if (actionSlider != null)
        {
            actionSlider.maxValue = maxGauge;
            actionSlider.value = currentGauge;
        }
    }

    public void UpdateStatusIcons()
    {
        if (statusIconContainer == null || statusIconPrefab == null) return;
        if (targetUnit == null) 
        {
            foreach (GameObject icon in activeStatusIcons)
            {
                Destroy(icon);
            }
            activeStatusIcons.Clear();
            return;
        }

        foreach (GameObject icon in activeStatusIcons)
        {
            Destroy(icon);
        }
        activeStatusIcons.Clear();

        foreach (ActiveStatusEffect effect in targetUnit.activeStatusEffects)
        {
            GameObject iconGO = Instantiate(statusIconPrefab, statusIconContainer);
            StatusIconUI iconUI = iconGO.GetComponent<StatusIconUI>();
            if (iconUI != null)
            {
                iconUI.Setup(effect);
            }
            activeStatusIcons.Add(iconGO);
        }
    }
}
