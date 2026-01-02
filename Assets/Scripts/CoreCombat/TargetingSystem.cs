using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class TargetingSystem
{
    /// <summary>
    /// ค้นหาเป้าหมายตามประเภทที่กำหนด
    /// </summary>
    public static List<Unit> FindTargets(Unit caster, SkillTargetType targetType)
    {
        List<Unit> foundTargets = new List<Unit>();
        
        // 1. กำหนดก่อนว่าจะหาเป้าหมายจากฝั่งไหน
        List<Unit> sourceList;
        bool isTargetingAllies = targetType == SkillTargetType.Self || 
                                 targetType == SkillTargetType.Single_Ally || 
                                 targetType == SkillTargetType.All_Allies || 
                                 targetType == SkillTargetType.Random_Ally;

        if (isTargetingAllies)
        {
            sourceList = (caster.CompareTag("Alliance")) ? GameManager.Instance.allianceUnits : GameManager.Instance.enemyUnits;
        }
        else // Targeting enemies
        {
            sourceList = (caster.CompareTag("Alliance")) ? GameManager.Instance.enemyUnits : GameManager.Instance.allianceUnits;
        }

        // 2. กรองเฉพาะยูนิตที่ยังมีชีวิตอยู่
        List<Unit> aliveTargets = sourceList.Where(u => u != null && u.currentState != UnitState.Dead).ToList();
        if (aliveTargets.Count == 0)
        {
            return foundTargets; // คืนค่า List ว่างเปล่า
        }

        // 3. เลือกเป้าหมายตามประเภท
        switch (targetType)
        {
            case SkillTargetType.Self:
                foundTargets.Add(caster);
                break;

            case SkillTargetType.Single_Enemy: // TODO: เพิ่ม Logic การเลือกเป้าหมาย (ตอนนี้ทำงานเหมือน Random)
            case SkillTargetType.Random_Enemy:
            case SkillTargetType.Single_Ally:   // TODO: เพิ่ม Logic การเลือกเป้าหมาย
            case SkillTargetType.Random_Ally:
                foundTargets.Add(aliveTargets[Random.Range(0, aliveTargets.Count)]);
                break;

            case SkillTargetType.All_Enemies:
            case SkillTargetType.All_Allies:
                foundTargets.AddRange(aliveTargets);
                break;
        }

        return foundTargets;
    }
}
