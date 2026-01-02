using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class PlayerSetupUI : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_Dropdown classDropdown;
    public GameObject setupPanel;

    private void Start()
    {
        // ถ้ามีไฟล์เซฟแล้ว → ไปต่อ
        //PlayeraData loadedData = SaveSystem.LoadPlayerData();
        /*
        if (loadedData != null)
        {
            Debug.Log("พบข้อมูลเซฟของ: " + loadedData.playerName);
            LoadMainGame(); // ข้ามหน้า setup ไปเลย
            return;
        }
        */

        // ถ้าไม่มีเซฟ → แสดง UI ให้ผู้เล่นกรอก
        classDropdown.ClearOptions();
        classDropdown.AddOptions(Enum.GetNames(typeof(PlayerClass)).ToList());
        setupPanel.SetActive(true);
    }

    public void OnStartGameButtonPressed()
    {
        string playerName = nameInput.text;
        PlayerClass selectedClass = (PlayerClass)classDropdown.value;

        PlayeraData newData = new PlayeraData
        {
            playerName = playerName,
            playerClass = selectedClass,
            level = 1,
            coin = 0
        };

        //SaveSystem.SavePlayerData(newData);
        Debug.Log("สร้างผู้เล่นใหม่: " + playerName + " (" + selectedClass + ")");
        LoadMainGame();
    }

    private void LoadMainGame()
    {
        // โหลดฉาก MainGame
        SceneManager.LoadScene("MainGame");
    }
}