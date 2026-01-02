using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BattleResultUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    [Header("Reward UI (For Victory Panel)")]
    public Transform rewardItemContainer;
    public GameObject rewardIconPrefab;

    [Header("Victory Buttons")]
    public Button victory_HomeButton;
    public Button victory_ContinueButton;
    public Button victory_RestartButton;

    [Header("Defeat Buttons")]
    public Button defeat_HomeButton;
    public Button defeat_RestartButton;

    private Stage completedStage;

    private void Awake()
    {
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);

        // --- ตั้งค่า Listener ให้กับปุ่มทุกปุ่ม ---
        // ปุ่มหน้า Victory
        if (victory_HomeButton != null) victory_HomeButton.onClick.AddListener(OnHomeClicked);
        if (victory_ContinueButton != null) victory_ContinueButton.onClick.AddListener(OnContinueClicked);
        if (victory_RestartButton != null) victory_RestartButton.onClick.AddListener(OnRestartClicked);

        // ปุ่มหน้า Defeat
        if (defeat_HomeButton != null) defeat_HomeButton.onClick.AddListener(OnHomeClicked);
        if (defeat_RestartButton != null) defeat_RestartButton.onClick.AddListener(OnRestartClicked);
    }

    public void ShowResult(bool won, Stage stage, List<RewardItem> rewards)
    {
        this.completedStage = stage;

        if (won)
        {
            victoryPanel.SetActive(true);
            defeatPanel.SetActive(false);

            // ตั้งค่าปุ่มในหน้า Victory
            if (stage == null)
            {
                Debug.LogError("[BattleResultUI] Received NULL stage data. Continue and Restart will be disabled.");
                victory_ContinueButton.interactable = false;
                victory_RestartButton.interactable = false;
            }
            else
            {
                victory_RestartButton.interactable = true;
                Stage nextStage = FindNextStage();
                victory_ContinueButton.interactable = (nextStage != null);
            }

            // แสดงผลรางวัล
            foreach (Transform child in rewardItemContainer)
            {
                Destroy(child.gameObject);
            }
            if (rewards != null)
            {
                foreach (var reward in rewards)
                {
                    GameObject iconGO = Instantiate(rewardIconPrefab, rewardItemContainer);
                    RewardIconUI iconUI = iconGO.GetComponent<RewardIconUI>();
                    if (iconUI != null) iconUI.Setup(reward);
                }
            }
        }
        else
        {
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(true);
        }
    }

    void OnHomeClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainGame"); //อย่าเปลี่ยนชื่อเพราะหน้าหลักคือ scene MainGame
    }

    void OnContinueClicked()
    {
        if (completedStage == null) return;
        Time.timeScale = 1f;
        Stage nextStage = FindNextStage();

        if (nextStage != null)
        {
            UnitStats finalStats = PlayerDataManager.Instance.RecalculatePlayerStats();
            UnitsDataSO playerUnitSO = GameDatabase.Instance.GetUnitSOByID(PlayerDataManager.Instance.gameData.playerData.playerUnitID);
            StageLoader.Instance.LoadCombatScene(nextStage.stageID, playerUnitSO, finalStats, nextStage.enemiesInThisStage, nextStage.combatSceneName);
        }
        else
        {
            OnHomeClicked();
        }
    }

    public void OnRestartClicked()
    {   
        if (completedStage == null) return;
        Time.timeScale = 1f;
        UnitStats finalStats = PlayerDataManager.Instance.RecalculatePlayerStats();
        UnitsDataSO playerUnitSO = GameDatabase.Instance.GetUnitSOByID(PlayerDataManager.Instance.gameData.playerData.playerUnitID);
        StageLoader.Instance.LoadCombatScene(completedStage.stageID, playerUnitSO, finalStats, completedStage.enemiesInThisStage, completedStage.combatSceneName);
    }

    public void OnClick_Restart()
    {
        // 1. คืนค่า Time.timeScale ให้เป็นปกติก่อนโหลด Scene ใหม่
        Time.timeScale = 1f;
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 3. โหลด Scene นั้นซ้ำอีกครั้ง
        Debug.Log($"Restarting scene: {currentSceneName}");
        SceneManager.LoadScene(currentSceneName);
    }

    private Stage FindNextStage()
    {
        if (completedStage == null) return null;
        
        return GameDatabase.Instance.allStages.Find(s => 
            s.chapterIndex == completedStage.chapterIndex && 
            s.stageIndexInChapter == completedStage.stageIndexInChapter + 1);
    }
}
