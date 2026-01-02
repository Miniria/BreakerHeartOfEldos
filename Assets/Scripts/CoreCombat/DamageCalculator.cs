using UnityEngine;

public struct DamageResult
{
    public float finalDamage;
    public float rawDamageToReflect;
    public bool wasEvaded;
    public bool wasCritical;
}

public static class DamageCalculator
{
    private const float DEFENSE_CONSTANT = 200f;
    private const float EVASION_CONSTANT = 200f;
    private const float GLANCING_DAMAGE_MULTIPLIER = 0.4f;
    private const float NORMAL_ATTACK_POWER = 100f;

    public static DamageResult CalculateDamage(Unit attacker, Unit target, SkillDataSO skill)
    {
        return Calculate(attacker, target, skill.power);
    }
    public static DamageResult CalculateDamage(Unit attacker, Unit target)
    {
        return Calculate(attacker, target, NORMAL_ATTACK_POWER);
    }

    private static DamageResult Calculate(Unit attacker, Unit target, float skillPower)
    {
        Debug.Log("--- [DamageCalculator] Starting Calculation ---");
        DamageResult result = new DamageResult();

        // 1. คำนวณดาเมจพื้นฐาน
        float baseDamage = (skillPower * attacker.currentStats.attack) / 100f;
        Debug.Log($"[1] Attacker: {attacker.name} (ATK:{attacker.currentStats.attack}), SkillPower: {skillPower} -> BaseDamage: {baseDamage}");

        // 2. ตรวจสอบ Critical
        bool isCritical = Random.Range(0f, 100f) <= attacker.currentStats.critical;
        result.wasCritical = isCritical;
        Debug.Log($"[2] Crit Chance: {attacker.currentStats.critical}% -> IsCritical: {isCritical}");

        float damageBeforeDefense = baseDamage;
        if (isCritical)
        {
            damageBeforeDefense = (baseDamage * attacker.currentStats.criticalDamage) / 100f;
            Debug.Log($"[2a] Crit Damage Multiplier: {attacker.currentStats.criticalDamage}% -> Damage after Crit: {damageBeforeDefense}");
        }
        
        result.rawDamageToReflect = damageBeforeDefense;

        // 3. ตรวจสอบ Evation (Glancing Hit)
        float statDifference = target.currentStats.evation - attacker.currentStats.accurate;
        float evasionChance = 0f;
        if (statDifference > 0)
        {
            evasionChance = (statDifference / (statDifference + EVASION_CONSTANT)) * 100f;
        }
        Debug.Log($"[3] Target: {target.name} (EVA:{target.currentStats.evation}), Attacker (ACC:{attacker.currentStats.accurate}) -> EvasionChance: {evasionChance}%");

        if (Random.Range(0f, 100f) <= evasionChance)
        {
            result.wasEvaded = true;
            result.wasCritical = false; // Glancing Hit ยกเลิก Critical
            result.finalDamage = damageBeforeDefense * GLANCING_DAMAGE_MULTIPLIER;
            Debug.LogWarning($"[3a] GLANCING HIT! Damage reduced by {100 - (GLANCING_DAMAGE_MULTIPLIER * 100)}%. -> Final Damage: {result.finalDamage}");
            Debug.Log("--- [DamageCalculator] Finished ---");
            return result;
        }

        // 4. ถ้าไม่ติด Evation, ค่อยหักลบด้วย DEF
        float defenseMultiplier = DEFENSE_CONSTANT / (DEFENSE_CONSTANT + target.currentStats.defense);
        Debug.Log($"[4] Target DEF: {target.currentStats.defense} -> DefenseMultiplier: {defenseMultiplier}");
        
        result.finalDamage = damageBeforeDefense * defenseMultiplier;
        Debug.Log($"[5] Final Damage: {damageBeforeDefense} * {defenseMultiplier} = {result.finalDamage}");
        Debug.Log("--- [DamageCalculator] Finished ---");
        
        return result;
    }
}
