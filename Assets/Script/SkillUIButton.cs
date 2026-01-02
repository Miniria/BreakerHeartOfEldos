using UnityEngine;
using UnityEngine.UI;

public class SkillUIButton : MonoBehaviour
{
    public int skillSlotIndex;
    public SkillController skillController;

    [Header("UI")]
    public Image iconImage;
    public Image cooldownOverlay;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {
            skillController?.UseSkill(skillSlotIndex);
        });

        if (skillController != null)
        {
            var skill = skillController.GetSkill(skillSlotIndex);
            if (iconImage != null && skill != null)
            {
                iconImage.sprite = skill.icon;
            }
        }
    }

    void Update()
    {
        if (skillController != null && cooldownOverlay != null)
        {
            float fill = skillController.GetCooldownPercent(skillSlotIndex);
            cooldownOverlay.fillAmount = fill;
        }
    }
}
