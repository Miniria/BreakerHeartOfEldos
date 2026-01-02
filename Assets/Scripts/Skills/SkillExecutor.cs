using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class SkillExecutor
{
    public static void Execute(Unit caster, SkillDataSO skill)
    {
        Debug.Log($"[SkillExecutor] Executing effects for {skill.skillName}");

        List<Unit> targets = TargetingSystem.FindTargets(caster, skill.targetType);
        if (targets.Count == 0)
        {
            Debug.LogWarning($"[SkillExecutor] No valid targets found for skill {skill.skillName}.");
            return;
        }

        // --- ลบส่วนจัดการ VFX ของสกิลออกจากที่นี่ ---

        foreach (var target in targets)
        {
            DamageResult mainDamageResult = new DamageResult();
            bool wasEvaded = false;

            switch (skill.mainEffectType)
            {
                case MainEffectType.Damage:
                    mainDamageResult = DamageCalculator.CalculateDamage(caster, target, skill);
                    target.TakeDamage(mainDamageResult, caster);
                    wasEvaded = mainDamageResult.wasEvaded;
                    break;
                case MainEffectType.Heal:
                    float healAmount = (skill.power * caster.currentStats.attack) / 100f;
                    target.Heal(healAmount);
                    break;
            }

            if (wasEvaded)
            {
                Debug.Log($"[SkillExecutor] Main effect was evaded. Skipping additional effects on {target.name}.");
                continue;
            }

            if (skill.additionalEffects != null)
            {
                foreach (var effect in skill.additionalEffects)
                {
                    ApplyAdditionalEffect(caster, target, effect, skill.power);
                }
            }
        }
    }

    private static void ApplyAdditionalEffect(Unit caster, Unit target, SkillEffect effect, float mainSkillPower)
    {
        switch (effect.effectType)
        {
            case AdditionalEffectType.ApplyStatusEffect:
                if (Random.Range(0f, 100f) <= effect.chanceToApply)
                {
                    float powerForEffect = (effect.power > 0) ? effect.power : mainSkillPower;
                    target.AddStatusEffect(effect.statusEffectToApply, powerForEffect);
                }
                break;
        }
    }
}
