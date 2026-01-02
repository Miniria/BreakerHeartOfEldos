using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageNodeUI : MonoBehaviour
{
    [Tooltip("ลำดับของด่านนี้ในบท (ต้องกำหนดใน Inspector)")]
    public int stageIndexInChapter;

    [Header("UI References")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI stageNameText; // หรือ stageNumberText

    private Stage _stageData;
    private StageSelectionUI _ownerUI;

    public void Setup(Stage data, StageSelectionUI owner)
    {
        this._stageData = data;
        this._ownerUI = owner;

        if (stageNameText != null)
        {
            stageNameText.text = data.stageName; // หรือ $"Stage {data.stageIndexInChapter}"
        }

        // ไม่ต้องยุ่งกับ onClick ที่นี่แล้ว
        // button.onClick.RemoveAllListeners();
        // button.onClick.AddListener(OnNodeClicked);
    }

    // เมธอดนี้จะถูกเรียกจาก Event 'On Click ()' ใน Inspector ของปุ่ม
    public void OnNodeClicked()
    {
        if (_ownerUI != null && _stageData != null)
        {
            _ownerUI.OnStageNodeSelected(_stageData);
        }
        else
        {
            Debug.LogError($"StageNodeUI (Index: {stageIndexInChapter}) has not been setup correctly or is missing data!");
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
