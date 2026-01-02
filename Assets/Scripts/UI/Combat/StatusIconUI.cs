using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusIconUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI durationText;

    public void Setup(ActiveStatusEffect statusEffect)
    {
        if (statusEffect == null || statusEffect.effectSO == null)
        {
            gameObject.SetActive(false);
            return;
        }

        // ตั้งค่าไอคอน
        iconImage.sprite = statusEffect.effectSO.icon;

        // ตั้งค่าตัวเลข Duration
        durationText.text = statusEffect.remainingDuration.ToString();
    }
}
