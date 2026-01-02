using UnityEngine;
using System.IO;
using System;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private string saveFilePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ทำให้ SaveManager อยู่ตลอดแม้เปลี่ยน Scene
            saveFilePath = Path.Combine(Application.persistentDataPath, "gamedata.json");
        }
    }

    public void SaveGame(GameData data)
    {
        // **อัปเดตเวลาก่อน Save เสมอ**
        data.playerData.lastLogoutTime = DateTime.UtcNow.ToBinary();

        string json = JsonUtility.ToJson(data, true); // true เพื่อให้จัดรูปแบบสวยงาม อ่านง่าย
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved to: " + saveFilePath);
    }

    public GameData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Game Loaded from: " + saveFilePath);
            return data;
        }
        else
        {
            Debug.LogWarning("No save file found. Creating a new game.");
            return new GameData(); // คืนค่าข้อมูลเกมใหม่ถ้าไม่มีไฟล์เซฟ
        }
    }
}
