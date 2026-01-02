using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardPreviewIconUI : MonoBehaviour
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI amountText;
    public GameObject firstTimeMark; // <-- เพิ่มตัวแปรสำหรับ "ป้าย"

    private void Awake()
    {
        // ปิดป้ายไว้เป็นค่าเริ่มต้น
        if (firstTimeMark != null)
        {
            firstTimeMark.SetActive(false);
        }
    }

    /// <summary>
    /// ตั้งค่าไอคอนและจำนวนของรางวัล
    /// </summary>
    /// <param name="isFirstTime">เป็นรางวัลครั้งแรกหรือไม่?</param>
    public void Setup(RewardItem reward, bool isFirstTime)
    {
        Sprite iconToShow = null;
        int amountToShow = 0;

        switch (reward.type)
        {
            case RewardType.Item:
                ItemDataSO itemData = GameDatabase.Instance.GetItemByID(reward.itemID);
                if (itemData != null)
                {
                    iconToShow = itemData.icon;
                    amountToShow = reward.itemQuantity;
                }
                else
                {
                    Debug.LogWarning($"Could not find item with ID '{reward.itemID}' in GameDatabase.");
                    gameObject.SetActive(false);
                    return;
                }
                break;

            case RewardType.Gold:
                iconToShow = GameDatabase.Instance.goldIcon;
                amountToShow = reward.amount;
                break;

            case RewardType.Experience:
                iconToShow = GameDatabase.Instance.expIcon;
                amountToShow = reward.amount;
                break;
            
            case RewardType.Gem:
                iconToShow = GameDatabase.Instance.gemIcon;
                amountToShow = reward.amount;
                break;

            default:
                gameObject.SetActive(false);
                return;
        }

        // ตั้งค่ารูปภาพ
        if (itemIcon != null)
        {
            itemIcon.sprite = iconToShow;
        }

        // ตั้งค่าจำนวน
        if (amountText != null)
        {
            amountText.text = amountToShow.ToString();
            bool showAmount = !(reward.type == RewardType.Item && amountToShow <= 1);
            amountText.gameObject.SetActive(showAmount);
        }

        // เปิด/ปิด "ป้าย" รางวัลครั้งแรก
        if (firstTimeMark != null)
        {
            firstTimeMark.SetActive(isFirstTime);
        }
    }
}
