using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StageDetailsUI : MonoBehaviour
{
    [Header("Info UI")]
    public TextMeshProUGUI stageNameText;
    public TextMeshProUGUI stageDescriptionText;

    [Header("Enemies UI")]
    public Transform monsterIconContainer;
    public GameObject monsterIconPrefab;

    [Header("Reward UI")]
    public Transform firstTimeRewardContainer;
    public Transform guaranteedRewardContainer;
    public Transform randomDropContainer;
    public GameObject rewardIconPrefab;

    [Header("Action Buttons")]
    public StageButton startCombatButton;
    public Button closeButton;

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }
    }

    public void Show(Stage stageData)
    {
        if (stageData == null) 
        {
            Debug.LogError("StageDetailsUI.Show() received null stageData!");
            return;
        }

        stageNameText.text = stageData.stageName;
        stageDescriptionText.text = stageData.stageDescription;

        UpdateMonsterUI(monsterIconContainer, stageData.enemiesInThisStage);

        // --- แก้ไขการเรียกใช้ ---
        // ส่ง true สำหรับรางวัลครั้งแรก
        UpdateRewardUI(firstTimeRewardContainer, stageData.firstTimeRewards, true); 
        // ส่ง false (หรือใช้ Overload) สำหรับรางวัลปกติ
        UpdateRewardUI(guaranteedRewardContainer, stageData.guaranteedRewards, false);
        // -----------------------
        
        if (startCombatButton != null)
        {
            startCombatButton.StageData = stageData;
        }
        else
        {
            Debug.LogError("[StageDetailsUI] StartCombatButton is not assigned in the inspector!");
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UpdateMonsterUI(Transform container, List<UnitsDataSO> enemies)
    {
        if (container == null) return;
        
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        if (enemies == null) return;

        if (monsterIconPrefab == null)
        {
            Debug.LogError("MonsterIconPrefab is not assigned in StageDetailsUI!");
            return;
        }

        foreach (var enemyData in enemies)
        {
            if (enemyData == null) continue;
            
            GameObject iconGO = Instantiate(monsterIconPrefab, container);
            Image iconImage = iconGO.GetComponent<Image>();
            
            if (iconImage != null && enemyData.icon != null)
            {
                iconImage.sprite = enemyData.icon;
            }
        }
    }

    // Overload สำหรับเรียกใช้แบบเดิม (isFirstTime = false)
    private void UpdateRewardUI(Transform container, List<RewardItem> rewards)
    {
        UpdateRewardUI(container, rewards, false);
    }

    // เมธอดหลักที่เพิ่มพารามิเตอร์ isFirstTime
    private void UpdateRewardUI(Transform container, List<RewardItem> rewards, bool isFirstTime)
    {
        if (container == null) return;
        
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        if (rewards == null) return;

        if (rewardIconPrefab == null)
        {
            Debug.LogError("RewardIconPrefab is not assigned in StageDetailsUI!");
            return;
        }

        foreach (var reward in rewards)
        {
            GameObject iconGO = Instantiate(rewardIconPrefab, container);
            RewardPreviewIconUI iconUI = iconGO.GetComponent<RewardPreviewIconUI>();
            if (iconUI != null)
            {
                // ส่ง isFirstTime เข้าไปในเมธอด Setup
                iconUI.Setup(reward, isFirstTime);
            }
            else
            {
                Debug.LogWarning($"Prefab '{rewardIconPrefab.name}' is missing RewardPreviewIconUI.cs script.");
            }
        }
    }
}
