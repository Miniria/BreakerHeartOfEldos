using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitGenerator : MonoBehaviour
{
    [Header("Unit Prefab")]
    public GameObject baseUnitPrefab;

    [Header("Positions")]
    public GameObject[] enemyPosition;
    public GameObject[] alliancePosition;

    [Header("UI Settings - Pre-placed UI")]
    public UnitUI[] allianceUIs;
    public UnitUI[] enemyUIs;
    
    public SkillButton[] skillButtons;
    private GameManager gameManager;

    public Stage currentStage { get; private set; }

    void Awake()
    {
        Debug.Log("[UnitGenerator] Awake: Getting GameManager component.");
        gameManager = GetComponent<GameManager>();
    }
    
    void Start()
    {
        Debug.Log("--- [UnitGenerator] Start ---");

        if (StageLoader.Instance == null)
        {
            Debug.LogError("[UnitGenerator] CRITICAL: StageLoader.Instance is NULL. Cannot proceed.");
            return;
        }
        Debug.Log("[UnitGenerator] StageLoader found.");

        if (GameDatabase.Instance == null)
        {
            Debug.LogError("[UnitGenerator] CRITICAL: GameDatabase.Instance is NULL. Cannot proceed.");
            return;
        }
        Debug.Log("[UnitGenerator] GameDatabase found.");

        Debug.Log($"[UnitGenerator] Attempting to find stage with ID: '{StageLoader.Instance.stageIDForCombat}'");
        this.currentStage = GameDatabase.Instance.GetStageByID(StageLoader.Instance.stageIDForCombat);

        if (this.currentStage == null)
        {
            Debug.LogError($"[UnitGenerator] CRITICAL: Could not find Stage SO with ID: '{StageLoader.Instance.stageIDForCombat}' in GameDatabase.");
            return;
        }
        Debug.Log($"[UnitGenerator] Stage '{this.currentStage.stageName}' found successfully.");

        if (StageLoader.Instance.playerUnitSOForCombat == null)
        {
            Debug.LogError("[UnitGenerator] CRITICAL: playerUnitSOForCombat from StageLoader is NULL.");
            return;
        }
        if (StageLoader.Instance.enemiesForCombat == null)
        {
            Debug.LogError("[UnitGenerator] CRITICAL: enemiesForCombat from StageLoader is NULL.");
            return;
        }
        Debug.Log("[UnitGenerator] All data from StageLoader seems valid.");

        var allianceQueue = new Queue<UnitsDataSO>();
        var enemyQueue = new Queue<UnitsDataSO>();

        allianceQueue.Enqueue(StageLoader.Instance.playerUnitSOForCombat);
        foreach (var enemy in StageLoader.Instance.enemiesForCombat)
        {
            enemyQueue.Enqueue(enemy);
        }
        Debug.Log($"[UnitGenerator] Queued {allianceQueue.Count} alliance and {enemyQueue.Count} enemies.");

        Debug.Log("[UnitGenerator] Starting to spawn units...");
        SpawnInitialUnits(allianceQueue, alliancePosition, true);
        SpawnInitialUnits(enemyQueue, enemyPosition, false);
        Debug.Log("--- [UnitGenerator] Spawning process finished. ---");
    }

    private void SpawnInitialUnits(Queue<UnitsDataSO> queue, GameObject[] positions, bool isAlliance)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            if (queue.Count == 0)
            {
                Debug.LogWarning($"[UnitGenerator] No more units in {(isAlliance ? "alliance" : "enemy")} queue to spawn at position {i}.");
                break;
            }
            var data = queue.Dequeue();
            if (data == null)
            {
                Debug.LogError($"[UnitGenerator] Dequeued NULL unit data for {(isAlliance ? "alliance" : "enemy")} at position {i}. Skipping.");
                continue;
            }
            bool isPlayer = isAlliance && i == 0;
            SpawnUnitAtPosition(data, positions[i], isAlliance, isPlayer, i);
        }
    }

    private void SpawnUnitAtPosition(UnitsDataSO data, GameObject position, bool isAlliance, bool isPlayer, int uiIndex)
    {
        Debug.Log($"[UnitGenerator] Spawning '{data.unitName}' at '{position.name}'.");
        GameObject newUnitGO = Instantiate(baseUnitPrefab, position.transform.position, position.transform.rotation, position.transform);
        newUnitGO.tag = isAlliance ? "Alliance" : "Enemy";
        
        Unit unit = newUnitGO.GetComponent<Unit>();
        if (unit == null)
        {
            Debug.LogError($"[UnitGenerator] CRITICAL: Prefab 'baseUnitPrefab' is missing 'Unit' component!");
            return;
        }
        
        if (isPlayer)
        {
            unit.InitializeFromPlayerData(data, StageLoader.Instance.playerFinalStats);
        }
        else
        {
            unit.unitData = data;
            unit.InitializeFromSO();
        }
        
        UnitUI targetUnitUI = isAlliance ? allianceUIs.ElementAtOrDefault(uiIndex) : enemyUIs.ElementAtOrDefault(uiIndex);
        if (targetUnitUI != null)
        {
            targetUnitUI.targetUnit = unit;
            targetUnitUI.InitializeUI();
        }

        gameManager.RegisterUnit(unit);
        
        if (isPlayer)
        {
            PlayerController pc = newUnitGO.GetComponent<PlayerController>() ?? newUnitGO.AddComponent<PlayerController>();
            pc.playerUnit = unit;
            
            if (unit.skills != null && unit.skills.Count > 0)
            {
                foreach (var btn in skillButtons) btn.Init(pc);
            }
            else
            {
                foreach (var btn in skillButtons) btn.gameObject.SetActive(false);
            }
            gameManager.playerUnit = unit;
        }

        unit.OnUnitDead += () => OnUnitDeath(unit, isAlliance);
    }

    private void OnUnitDeath(Unit deadUnit, bool isAlliance)
    {
        // ... (โค้ดส่วนนี้เหมือนเดิม) ...
    }

    private UnitUI FindUnitUIForUnit(Unit unit)
    {
        // ... (โค้ดส่วนนี้เหมือนเดิม) ...
        return null; // Placeholder
    }

    private int GetUIIndexForPosition(GameObject positionGO, bool isAlliance)
    {
        // ... (โค้ดส่วนนี้เหมือนเดิม) ...
        return -1; // Placeholder
    }
}
