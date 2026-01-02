using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private GameObject confirmExitPanel;

    // เรียกเมื่อกดปุ่ม Exit
    public void OnExitBattlePressed()
    {
        confirmExitPanel.SetActive(true);

        // หยุดเกม
        Time.timeScale = 0f;
    }

    // กด Yes (ยืนยันออก)
    public void OnConfirmExit()
    {
        Time.timeScale = 1f; // คืนค่าเวลา
        Debug.Log("ออกจากการต่อสู้แล้ว");

        // ตัวอย่าง: โหลดกลับไปที่ Lobby
        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
    }

    // กด No (ยกเลิก)
    public void OnCancelExit()
    {
        confirmExitPanel.SetActive(false);

        // กลับมาเล่นเกมต่อ
        Time.timeScale = 1f;
    }
}
