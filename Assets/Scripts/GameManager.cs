using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public bool isAutoMode = false;
    public bool isActionInProgress { get; private set; }

    [Header("Unit Lists")]
    public List<Unit> allianceUnits;
    public List<Unit> enemyUnits;
    public Unit playerUnit { get; set; }
    
    [Header("Settings")]
    public float maxActionRate = 100f;
    [SerializeField] private float gaugeUpdateInterval = 0.1f;

    private readonly List<Unit> allUnits = new List<Unit>();
    private float gaugeUpdateTimer = 0f;
    private Queue<Unit> TurnOrder = new Queue<Unit>();

    [Header("UI")]
    public TMP_Text timerText;
    public BattleResultUI battleResultUI;
    
    private bool isGameEnded = false;
    private float elapsedTime = 0f;
    private UnitGenerator unitGenerator;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        unitGenerator = GetComponent<UnitGenerator>();
    }

    void Update()
    {
        if (isGameEnded) return;

        TimerCounter();

        if (!isActionInProgress)
        {
            UpdateActionGauge();
            TryStartTurn();
        }
        UpdateActionState();
        CheckGameEnd();
    }

    public void ToggleAutoMode()
    {
        isAutoMode = !isAutoMode;
        Debug.Log($"Auto mode is now {(isAutoMode ? "ON" : "OFF")}");
    }
    
    private void UpdateActionGauge()
    {
        gaugeUpdateTimer += Time.deltaTime;
        if (gaugeUpdateTimer < gaugeUpdateInterval) return;
        gaugeUpdateTimer = 0f;

        allUnits.Clear();
        CleanUnitLists();
        allUnits.AddRange(allianceUnits);
        allUnits.AddRange(enemyUnits);

        foreach (var unit in allUnits)
        {
            if (unit == null || unit.currentState != UnitState.Idle) continue;
            
            unit.ActionGauge += maxActionRate * unit.actionGaugeMultiplier * gaugeUpdateInterval;

            if (unit.ActionGauge >= BaseUnit.DefaultMaxActionGauge && !TurnOrder.Contains(unit))
            {
                TurnOrder.Enqueue(unit);
            }
        }
    }

    private void StartTurn()
    {
        if (TurnOrder.Count == 0) return;
        
        Unit currentUnit = TurnOrder.Dequeue();
        if (currentUnit == null || currentUnit.currentState == UnitState.Dead)
        {
            return;
        }
        
        currentUnit.StartTurn();
    }
    
    private void TryStartTurn()
    {
        if (isActionInProgress) return;
        if (TurnOrder.Count == 0) return;
        StartTurn();
    }
    
    public void RegisterUnit(Unit unit)
    {
        if (unit.CompareTag("Alliance"))
        {
            allianceUnits.Add(unit);
            // --- เมื่อ PlayerUnit ถูกสร้างใน Gameplay Scene ---
            if (unit.CompareTag("Player") && PlayerDataManager.Instance != null && PlayerDataManager.Instance.currentPlayerStats != null)
            {
                unit.ApplyBaseStats(PlayerDataManager.Instance.currentPlayerStats);
                playerUnit = unit; // กำหนด playerUnit ให้ GameManager
            }
            // -------------------------------------------------
        }
        else if (unit.CompareTag("Enemy")) enemyUnits.Add(unit);

        RecalculatePercentages();
        unit.ActionGauge = Random.Range(0.05f, 0.2f) * BaseUnit.DefaultMaxActionGauge;
        unit.StateChanged += (newState) => UpdateActionState();
        unit.OnUnitDead += () => UnregisterUnit(unit);
    }
    
    private void UnregisterUnit(Unit unit)
    {
        allianceUnits.Remove(unit);
        enemyUnits.Remove(unit);
        RecalculatePercentages();
    }

    private void RecalculatePercentages()
    {
        float totalSPD = 0f;
        allUnits.Clear();
        allUnits.AddRange(allianceUnits);
        allUnits.AddRange(enemyUnits);

        foreach (var u in allUnits)
        {
            if(u != null) totalSPD += u.currentStats.speed;
        }

        if (totalSPD <= 0) return;

        foreach (var u in allUnits)
        {
            if(u != null) u.actionGaugeMultiplier = u.currentStats.speed / totalSPD;
        }
    }
    
    private void CleanUnitLists()
    {
        allianceUnits.RemoveAll(u => u == null);
        enemyUnits.RemoveAll(u => u == null);
    }
    
    private void UpdateActionState()
    {
        CleanUnitLists();
        isActionInProgress = allUnits.Any(unit => unit != null && unit.currentState != UnitState.Idle && unit.currentState != UnitState.Dead);
    }

    private void TimerCounter()
    {
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"Time {minutes:00}:{seconds:00}";
    }

    private void CheckGameEnd()
    {
        if (isGameEnded) return;

        if (allianceUnits.Count == 0 && enemyUnits.Count > 0)
        {
            EndStage(false);
        }
        else if (enemyUnits.Count == 0 && allianceUnits.Count > 0)
        {
            EndStage(true);
        }
    }

    private void EndStage(bool playerWon)
    {
        if (isGameEnded) return;
        isGameEnded = true;
        Time.timeScale = 0f;

        List<RewardItem> grantedRewards = new List<RewardItem>();
        Stage currentStage = unitGenerator.currentStage;

        if (playerWon && currentStage != null)
        {
            grantedRewards = RewardSystem.GrantStageRewards(currentStage);
        }

        if (battleResultUI != null)
        {
            battleResultUI.ShowResult(playerWon, currentStage, grantedRewards);
        }
    }
}
