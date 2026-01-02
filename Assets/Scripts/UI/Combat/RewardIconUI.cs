using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardIconUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public Image frameImage; // สำหรับเปลี่ยนสีตาม Rarity
    public TextMeshProUGUI amountText;

    public void Setup(RewardItem reward)
    {
        switch (reward.type)
        {
            case RewardType.Gold:
                iconImage.sprite = GameDatabase.Instance.goldIcon;
                amountText.text = reward.amount.ToString("N0");
                if (frameImage != null) frameImage.enabled = false;
                break;

            case RewardType.Gem:
                iconImage.sprite = GameDatabase.Instance.gemIcon;
                amountText.text = reward.amount.ToString("N0");
                if (frameImage != null) frameImage.enabled = false;
                break;

            case RewardType.Experience:
                iconImage.sprite = GameDatabase.Instance.expIcon;
                amountText.text = $"+{reward.amount:N0} EXP";
                if (frameImage != null) frameImage.enabled = false;
                break;

            case RewardType.Item:
                ItemDataSO itemSO = GameDatabase.Instance.GetItemByID(reward.itemID);
                if (itemSO != null)
                {
                    iconImage.sprite = itemSO.icon;
                    amountText.text = (reward.itemQuantity > 1) ? reward.itemQuantity.ToString() : "";
                    
                    if (frameImage != null)
                    {
                        frameImage.enabled = true;
                        // --- แก้ไข: ดึง Rarity จาก reward แทน itemSO ---
                        frameImage.color = GameDatabase.Instance.GetRarityColor(reward.rarity);
                    }
                }
                break;
        }
    }
}
