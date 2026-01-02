using UnityEngine;
using System;

public class CombatTextSpawner : MonoBehaviour
{
    // เอา Singleton ออกไป
    // public static CombatTextSpawner Instance { get; private set; }

    [Tooltip("ลาก Prefab ของคุณ (popUpDmg) มาใส่")]
    public GameObject combatTextPrefab;

    private Camera mainCamera;

    private void Awake()
    {
        // ไม่ต้องมี Logic ของ Singleton อีกต่อไป
        mainCamera = Camera.main;
    }

    public void Spawn(Vector3 worldPosition, string text, Color color)
    {
        try
        {
            if (combatTextPrefab == null)
            {
                Debug.LogError("[CombatTextSpawner] PREFAB IS NULL. Please assign 'popUpDmg' in the Inspector.");
                return;
            }
            if (mainCamera == null)
            {
                Debug.LogError("[CombatTextSpawner] MAIN CAMERA IS NULL. Please tag your camera 'MainCamera'.");
                return;
            }

            GameObject textGO = Instantiate(combatTextPrefab, transform);
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition + new Vector3(0, 1.5f, 0));
            textGO.transform.position = screenPosition;

            CombatText textComponent = textGO.GetComponent<CombatText>();
            if (textComponent != null)
            {
                textComponent.Setup(text, color);
            }
            else
            {
                Debug.LogError($"Prefab '{combatTextPrefab.name}' is missing the 'CombatText.cs' script.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[CombatTextSpawner] An exception occurred: {e.Message}\n{e.StackTrace}");
        }
    }
}
