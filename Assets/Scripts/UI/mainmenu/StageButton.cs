using UnityEngine;
using System.Collections.Generic;

public class StageButton : MonoBehaviour
{
    private Stage _stageData;

    public Stage StageData
    {
        set 
        { 
            _stageData = value; 
            Debug.Log($"[StageButton] Received StageData: {(_stageData != null ? _stageData.stageName : "NULL")}");
        }
    }

    public void OnClick_StartStage()
    {
        Debug.Log("[StageButton] OnClick_StartStage called.");

        if (_stageData == null)
        {
            Debug.LogError("[StageButton] Error: _stageData is NULL. Cannot start stage.");
            return;
        }

        string playerUnitID = PlayerDataManager.Instance.gameData.playerData.playerUnitID;
        if (string.IsNullOrEmpty(playerUnitID))
        {
            Debug.LogError("[StageButton] Player Unit ID is not set in PlayerData!");
            return;
        }

        UnitsDataSO playerUnitSO = GameDatabase.Instance.GetUnitSOByID(playerUnitID);
        if (playerUnitSO == null)
        {
            Debug.LogError($"[StageButton] Could not find Unit SO with ID: {playerUnitID} in GameDatabase!");
            return;
        }

        UnitStats finalStats = PlayerDataManager.Instance.RecalculatePlayerStats();

        // 4. เรียกใช้ StageLoader เพื่อส่งข้อมูลทั้งหมดและเปลี่ยน Scene
        Debug.Log($"[StageButton] Loading Scene: {_stageData.combatSceneName} with Stage ID: {_stageData.stageID}");
        StageLoader.Instance.LoadCombatScene(
            _stageData.stageID, // <-- ส่ง ID ของด่านไปด้วย
            playerUnitSO, 
            finalStats, 
            _stageData.enemiesInThisStage, 
            _stageData.combatSceneName
        );
    }
}
