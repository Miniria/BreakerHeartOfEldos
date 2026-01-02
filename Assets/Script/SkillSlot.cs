using UnityEngine;
public class SkillSlot
{
    public SkillData skill;
    public float cooldownTimer;
    private PlayerBehavior player;

    public bool IsReady => cooldownTimer <= 0f;

    public SkillSlot(SkillData data, PlayerBehavior owner)
    {
        skill = data;
        player = owner;
        cooldownTimer = 0f;
    }

    public void UpdateCooldown(float deltaTime)
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= deltaTime;
    }

    public void Activate()
    {
        if (!IsReady || skill == null)
            return;

        EnemyBehavior target = EnemyBehavior.GetCurrentTarget();
        if (target == null) return;

        switch (skill.type)
        {
            case SkillsType.Damage:
                int baseDmg = player.GetAttackPower();
                int totalDmg = Mathf.RoundToInt(baseDmg * skill.powerMultiplier);
                target.TakeDamage(totalDmg);
                Debug.Log($"{player.GetCharacterName()} used {skill.skillName}, dealt {totalDmg}!");
                break;

            case SkillsType.Heal:
                // player.Heal(...)
                break;

            case SkillsType.Buff:
                // Apply buff logic here
                break;
        }

        cooldownTimer = skill.cooldown;
    }

    public float GetCooldownPercent()
    {
        return Mathf.Clamp01(cooldownTimer / skill.cooldown);
    }
}
