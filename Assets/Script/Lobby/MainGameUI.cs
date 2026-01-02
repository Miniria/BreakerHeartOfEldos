using UnityEngine;
using TMPro;

public class MainGameUI : MonoBehaviour
{
    public TMP_Text playerNameText;
    public TMP_Text classText;
    public TMP_Text levelText;

    void Start()
    {
        PlayerData data = SaveSystem.LoadPlayerData();

        if (data != null)
        {
            //playerNameText.text = $"{data.playerName}";
            //classText.text = $"Class {data.playerClass}";
            //levelText.text = $"Lv. {data.level}";
        }
        else
        {
            playerNameText.text = "Name: Unknown";
            Debug.LogWarning("ไม่พบข้อมูลผู้เล่น");
        }
    }
}