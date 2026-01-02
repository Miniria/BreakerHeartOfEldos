using UnityEngine;

public class SkillController : MonoBehaviour
{
    public SkillData[] equippedSkills = new SkillData[3]; // Drag in via inspector or set at runtime
    private SkillSlot[] skillSlots = new SkillSlot[3];

    private PlayerBehavior player;
    public bool autoUseSkills = false;

    void Start()
    {
        player = GetComponent<PlayerBehavior>();

        for (int i = 0; i < equippedSkills.Length; i++)
        {
            if (equippedSkills[i] != null)
                skillSlots[i] = new SkillSlot(equippedSkills[i], player);
        }
    }

    void Update()
    {
        for (int i = 0; i < skillSlots.Length; i++)
        {
            skillSlots[i]?.UpdateCooldown(Time.deltaTime);

            // ? Auto-use for AI-controlled characters
            if (autoUseSkills && skillSlots[i]?.IsReady == true)
            {
                skillSlots[i].Activate();
            }
        }
    }

    public void UseSkill(int index)
    {
        if (index >= 0 && index < skillSlots.Length)
        {
            skillSlots[index]?.Activate();
        }
    }

    public SkillData GetSkill(int index) => equippedSkills[index];
    public float GetCooldownPercent(int index) => skillSlots[index]?.GetCooldownPercent() ?? 0f;
}
