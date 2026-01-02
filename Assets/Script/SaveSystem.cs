using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string saveFile = Application.persistentDataPath + "/playerdata.json";

    public static void SavePlayerData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFile, json);
        Debug.Log("Saved to: " + saveFile);
    }

    public static PlayerData LoadPlayerData()
    {
        if (File.Exists(saveFile))
        {
            string json = File.ReadAllText(saveFile);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            Debug.LogWarning("No save file found.");
            return null;
        }
    }

    public static void DeleteSave()
    {
        if (File.Exists(saveFile))
            File.Delete(saveFile);
    }
}