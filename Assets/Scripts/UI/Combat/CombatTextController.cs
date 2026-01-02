using UnityEngine;

[RequireComponent(typeof(BaseUnit))]
public class CombatTextController : MonoBehaviour
{
    // ตัวแปรสำหรับเก็บ Spawner ที่ถูกต้อง
    public CombatTextSpawner spawner;

    // ไม่ต้องมี Logic การดักฟัง Event อีกต่อไป
    // private BaseUnit targetUnit;
    // void Awake() { ... }
    // void OnEnable() { ... }
    // void OnDisable() { ... }
    // void OnHealthChanged(...) { ... }
    // void OnStateChanged(...) { ... }

    /// <summary>
    /// เมธอดนี้จะถูกเรียกโดยตรงจาก BaseUnit.TakeDamage
    /// </summary>
    public void ShowDamageText(DamageResult result)
    {
        if (spawner == null)
        {
            Debug.LogError("CombatTextSpawner is not assigned to CombatTextController!", this);
            return;
        }
        if (result.finalDamage <= 0 && !result.wasEvaded) return;

        string text;
        Color color;

        if (result.wasEvaded)
        {
            text = "Glancing";
            color = Color.gray;
        }
        else
        {
            text = Mathf.RoundToInt(result.finalDamage).ToString();
            color = result.wasCritical ? Color.yellow : Color.white;
            if(result.wasCritical) text += "!";
        }

        spawner.Spawn(transform.position, text, color);
    }
}
