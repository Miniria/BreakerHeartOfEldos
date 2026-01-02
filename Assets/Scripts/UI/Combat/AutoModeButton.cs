using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AutoModeButton : MonoBehaviour
{
    [Header("UI References")]
    private Button autoButton;
    private Image buttonImage;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color autoModeColor = Color.yellow;

    private void Awake()
    {
        autoButton = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        // ตั้งค่าให้ปุ่มนี้เรียกใช้เมธอด ToggleAutoMode ของ GameManager
        autoButton.onClick.AddListener(() => {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ToggleAutoMode();
            }
        });
    }

    private void Start()
    {
        // ตั้งค่าสีเริ่มต้น
        UpdateButtonColor();
    }

    // ใช้ LateUpdate เพื่อให้แน่ใจว่าสีจะอัปเดตหลังจาก Logic ของ GameManager ทำงานแล้ว
    private void LateUpdate()
    {
        UpdateButtonColor();
    }

    private void UpdateButtonColor()
    {
        if (GameManager.Instance == null || buttonImage == null) return;

        // เปลี่ยนสีของ Image ตามสถานะ isAutoMode
        buttonImage.color = GameManager.Instance.isAutoMode ? autoModeColor : normalColor;
    }
}
