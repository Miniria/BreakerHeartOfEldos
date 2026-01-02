using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    [Header("Testing")]
    [Tooltip("ถ้าเป็น true, เมื่อเริ่มเกมใน Editor จะทำการ Login เป็น Guest โดยอัตโนมัติ")]
    public bool autoLoginAsGuestForTesting = true;

    [Header("UI Elements")]
    public GameObject tapToStartLabel;
    public GameObject loginPanel;

    [Header("Scene To Load")]
    public string nextSceneName = "MainMenu";

    private void Start()
    {
#if UNITY_EDITOR
        if (autoLoginAsGuestForTesting)
        {
            Debug.LogWarning("--- AUTO-LOGIN AS GUEST (FOR TESTING) ---");
            PlayerDataManager.Instance.LoginAsGuest();
        }
#endif

        // ตรวจสอบสถานะการ Login จาก PlayerDataManager
        if (PlayerDataManager.Instance.IsLoggedIn())
        {
            ShowTapToStart();
        }
        else
        {
            ShowLoginPanel();
        }
    }

    private void Update()
    {
        // ถ้าผู้เล่น Login อยู่ และกดหน้าจอ, ให้เข้าเกม
        if (PlayerDataManager.Instance.IsLoggedIn() && Input.GetMouseButtonDown(0) && tapToStartLabel.activeInHierarchy)
        {
            SceneManager.LoadScene(nextSceneName); 
        }
    }

    public void ShowTapToStart()
    {
        if (tapToStartLabel != null) tapToStartLabel.SetActive(true);
        if (loginPanel != null) loginPanel.SetActive(false);
    }

    public void ShowLoginPanel()
    {
        if (tapToStartLabel != null) tapToStartLabel.SetActive(false);
        if (loginPanel != null) loginPanel.SetActive(true);
    }

    // --- เมธอดสำหรับปุ่มใน Login Panel ---
    public void OnLoginClicked()
    {
        // TODO: ดึงค่าจาก InputField
        string username = "Player"; // ตัวอย่าง
        string password = "password"; // ตัวอย่าง

        bool success = PlayerDataManager.Instance.Login(username, password);
        if (success)
        {
            ShowTapToStart();
        }
        else
        {
            // TODO: แสดงข้อความว่า "Username หรือ Password ผิด"
        }
    }

    public void OnGuestClicked()
    {
        PlayerDataManager.Instance.LoginAsGuest();
        ShowTapToStart();
    }
}
