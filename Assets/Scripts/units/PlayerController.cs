using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Unit playerUnit;

    public event Action<List<SkillDataSO>> OnQueueUpdated;

    private void Awake()
    {
        if (playerUnit == null)
            playerUnit = GetComponent<Unit>();
    }

    public bool IsSkillOnCooldown(SkillDataSO skill)
    {
        return skill.currentCooldown > 0;
    }

    public void EnqueueSkill(int skillIndex)
    {
        // --- กลไกป้องกันการกดซ้ำ ---
        if (playerUnit.currentState != UnitState.Idle)
        {
            Debug.LogWarning($"Cannot enqueue skill. Player is currently in '{playerUnit.currentState}' state.");
            return; // ออกจากเมธอดทันทีถ้า Unit ไม่ได้อยู่ในสถานะ Idle
        }
        // --------------------------

        if (playerUnit.skills == null || skillIndex >= playerUnit.skills.Count)
        {
            Debug.LogError($"Invalid skill index: {skillIndex}");
            return;
        }

        SkillDataSO skill = playerUnit.skills[skillIndex];

        if (IsSkillOnCooldown(skill))
        {
            Debug.Log($"Skill '{skill.skillName}' is on cooldown.");
            return;
        }

        if (playerUnit.selectedSkill.Contains(skill))
        {
            playerUnit.selectedSkill.Remove(skill);
        }
        else
        {
            // จำกัดให้มีสกิลในคิวได้แค่ 1 สกิล (ถ้าต้องการ)
            // if (playerUnit.selectedSkill.Count > 0)
            // {
            //     playerUnit.selectedSkill.Clear();
            // }
            playerUnit.selectedSkill.Add(skill);
        }

        OnQueueUpdated?.Invoke(playerUnit.selectedSkill.ToList());
    }

    public void ClearQueue()
    {
        playerUnit.selectedSkill.Clear();
        OnQueueUpdated?.Invoke(playerUnit.selectedSkill.ToList());
    }
}
