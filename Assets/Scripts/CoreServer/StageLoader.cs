using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class StageLoader : MonoBehaviour
{
    public static StageLoader Instance { get; private set; }

    // ข้อมูลที่จะส่งไปยัง Combat Scene
    public string stageIDForCombat; // <-- เพิ่ม ID ของด่าน
    public UnitsDataSO playerUnitSOForCombat;
    public UnitStats playerFinalStats;
    public List<UnitsDataSO> enemiesForCombat;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadCombatScene(string stageID, UnitsDataSO playerUnitData, UnitStats finalStats, List<UnitsDataSO> enemyUnitData, string sceneName)
    {
        Debug.Log($"[StageLoader] Receiving data for Stage ID: {stageID}. Player: {playerUnitData.unitName}, Enemies: {(enemyUnitData != null ? enemyUnitData.Count : 0)}");

        this.stageIDForCombat = stageID;
        this.playerUnitSOForCombat = playerUnitData;
        this.playerFinalStats = finalStats;
        this.enemiesForCombat = enemyUnitData;

        SceneManager.LoadScene(sceneName);
    }
}
