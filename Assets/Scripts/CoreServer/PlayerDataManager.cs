using UnityEngine;
using System;
using System.Linq;
using static StatCalculator;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }
    public static event Action OnDataLoaded;
    public static event Action OnPlayerStatsChanged;

    public GameData gameData;
    public LevelDataSO levelTable;
    
    // --- เพิ่มตัวแปรสำหรับเก็บค่าพลังสุดท้ายของผู้เล่น ---
    public UnitStats currentPlayerStats; 

    [Header("New Player Defaults")]
    public string defaultPlayerUnitID = "player_001";

    private bool isLoggedIn = false;

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
            EquipmentManager.OnEquipmentChanged += RecalculatePlayerStatsForEvent;
            RecalculatePlayerStats(); // เรียกคำนวณครั้งแรกเมื่อ PlayerDataManager ถูกสร้าง
        }
    }
    
    private void OnDestroy()
    {
        EquipmentManager.OnEquipmentChanged -= RecalculatePlayerStatsForEvent;
    }

    public bool Login(string username, string password)
    {
        gameData = SaveManager.Instance.LoadGame(); 

        if (gameData.playerData.username == null || gameData.playerData.username == username)
        {
            if (gameData.playerData.username == null)
            {
                InitializeNewPlayerData(username, password);
            }
            
            isLoggedIn = true;
            CalculateOfflineProgress();
            OnDataLoaded?.Invoke();
            RecalculatePlayerStats(); // เรียกคำนวณหลังจากโหลดเกม
            return true;
        }
        
        return false;
    }

    public void LoginAsGuest()
    {
        gameData = new GameData();
        InitializeNewPlayerData("Guest-" + new System.Random().Next(1000, 9999), "");
        isLoggedIn = true;
        OnDataLoaded?.Invoke();
        RecalculatePlayerStats(); // เรียกคำนวณหลังจากสร้างผู้เล่นใหม่
    }

    public void Logout()
    {
        SaveData();
        isLoggedIn = false;
        gameData = null;
    }

    public bool IsLoggedIn()
    {
        return isLoggedIn;
    }

    private void InitializeNewPlayerData(string username, string password)
    {
        gameData.playerData.username = username;
        gameData.playerData.password = password;
        gameData.playerData.playerUnitID = defaultPlayerUnitID;
        gameData.playerData.accountLevel = 1;
        gameData.playerData.gold = 100;
    }

    public void SaveData()
    {
        if (gameData != null && isLoggedIn)
        {
            SaveManager.Instance.SaveGame(gameData);
        }
    }

    public void AddExperience(long amount)
    {
        if (gameData == null || levelTable == null) return;

        gameData.playerData.accountExperience += amount;
        long requiredExp = levelTable.GetExperienceForLevel(gameData.playerData.accountLevel);

        while (gameData.playerData.accountExperience >= requiredExp && requiredExp > 0)
        {
            gameData.playerData.accountExperience -= requiredExp;
            gameData.playerData.accountLevel++;
            Debug.Log($"ACCOUNT LEVEL UP! Reached Level {gameData.playerData.accountLevel}");
            
            RecalculatePlayerStats(); // เรียกเมธอดที่คืนค่า UnitStats
            requiredExp = levelTable.GetExperienceForLevel(gameData.playerData.accountLevel);
        }
    }

    /// <summary>
    /// เมธอดนี้มีไว้สำหรับ Event เท่านั้น (void, ไม่มีพารามิเตอร์)
    /// </summary>
    private void RecalculatePlayerStatsForEvent()
    {
        RecalculatePlayerStats(); // เรียกเมธอดหลักที่คืนค่า UnitStats
    }

    /// <summary>
    /// ศูนย์กลางการคำนวณค่าพลังของผู้เล่นใหม่ทั้งหมด (คืนค่า UnitStats)
    /// </summary>
    public UnitStats RecalculatePlayerStats()
    {
        if (gameData == null) return new UnitStats();

        UnitsDataSO playerSO = GameDatabase.Instance.GetUnitSOByID(gameData.playerData.playerUnitID);
        if (playerSO == null) 
        {
            Debug.LogError("Cannot find Player's UnitDataSO!");
            currentPlayerStats = new UnitStats(); // ตั้งค่าให้เป็นค่าว่าง
            return currentPlayerStats;
        }
        
        UnitStats finalStats = new UnitStats(playerSO.stats); 

        int baseAttribute = 9;
        int totalStr = baseAttribute + gameData.playerData.accountLevel;
        int totalAgi = baseAttribute + gameData.playerData.accountLevel;
        int totalInt = baseAttribute + gameData.playerData.accountLevel;
        UnitStats levelStats = CalculateBaseStatsFromAttributes(totalStr, totalAgi, totalInt);

        finalStats.health += levelStats.health;
        finalStats.attack += levelStats.attack;
        finalStats.defense += levelStats.defense;
        finalStats.speed += levelStats.speed;
        finalStats.critical += levelStats.critical;
        finalStats.criticalDamage += levelStats.criticalDamage;
        finalStats.accurate += levelStats.accurate;
        finalStats.evation += levelStats.evation;

        UnitStats equipmentStats = CalculateStatsFromEquipment(gameData.playerData.equippedItems, gameData.playerData.inventory);
        finalStats.health += equipmentStats.health;
        finalStats.attack += equipmentStats.attack;
        finalStats.defense += equipmentStats.defense;
        finalStats.speed += equipmentStats.speed;
        finalStats.critical += equipmentStats.critical;
        finalStats.criticalDamage += equipmentStats.criticalDamage;
        finalStats.accurate += equipmentStats.accurate;
        finalStats.evation += equipmentStats.evation;

        // --- ลบการเรียก playerUnit.ApplyBaseStats ออกไป ---
        // if (GameManager.Instance != null && GameManager.Instance.playerUnit != null)
        // {
        //     GameManager.Instance.playerUnit.ApplyBaseStats(finalStats);
        // }

        // --- เก็บค่าพลังสุดท้ายไว้ใน currentPlayerStats ---
        currentPlayerStats = finalStats; 

        Debug.Log($"Player stats recalculated: HP={finalStats.health}, ATK={finalStats.attack}");
        OnPlayerStatsChanged?.Invoke();

        return finalStats;
    }

    private void CalculateOfflineProgress()
    {
        if (gameData == null || gameData.playerData.lastLogoutTime == 0) return;

        DateTime lastLogout = DateTime.FromBinary(gameData.playerData.lastLogoutTime);
        TimeSpan offlineTime = DateTime.UtcNow - lastLogout;

        if (offlineTime.TotalSeconds > 0)
        {
            long offlineSeconds = (long)offlineTime.TotalSeconds;
            long goldEarned = offlineSeconds * 10;

            gameData.playerData.gold += goldEarned;
            Debug.Log($"Player was offline for {offlineTime.TotalMinutes:F0} minutes. Earned {goldEarned} gold.");
        }
    }

    public bool IsStageCleared(string stageID)
    {
        if (gameData == null) return false;
        return gameData.playerData.clearedStageIDs.Contains(stageID);
    }

    public void SetStageAsCleared(string stageID)
    {
        if (gameData == null || IsStageCleared(stageID)) return;
        gameData.playerData.clearedStageIDs.Add(stageID);
        Debug.Log($"Stage {stageID} marked as cleared.");
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveData();
        }
    }
}
