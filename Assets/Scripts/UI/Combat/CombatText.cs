using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
public class CombatText : MonoBehaviour
{
    [Tooltip("ลาก TextMeshPro Component ที่จะแสดงผลมาใส่")]
    public TextMeshProUGUI textMesh;

    [Tooltip("เวลาที่ GameObject นี้จะถูกทำลาย (ควรนานกว่า Animation เล็กน้อย)")]
    public float destroyTime = 1.5f;

    private void Awake()
    {
        // --- DEBUG: ปิดการทำลายตัวเองชั่วคราว ---
        // Destroy(gameObject, destroyTime); 
        Debug.Log($"[CombatText] Awake on {gameObject.name}. Auto-destroy is DISABLED for debugging.");
    }

    /// <summary>
    /// ตั้งค่าข้อความและสีของ Combat Text
    /// </summary>
    public void Setup(string text, Color color)
    {
        if (textMesh == null)
        {
            Debug.LogError("TextMeshProUGUI is not assigned in CombatText script!", this);
            return;
        }
        textMesh.text = text;
        textMesh.color = color;
    }
}
