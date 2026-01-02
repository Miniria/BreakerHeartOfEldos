using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutoAttackController : MonoBehaviour
{
    public WeaponData weaponData;
    public Animator animator;

    private float cooldownTimer;
    private bool isAttacking = false;
    private PlayerBehavior player;

    private Slider cooldownSlider;
    private float lastCooldownMax = 1f;

    void Start()
    {
        player = GetComponent<PlayerBehavior>();
        if (player == null)
        {
            Debug.LogError("AutoAttackController requires PlayerBehavior on same GameObject.");
            enabled = false;
            return;
        }

        cooldownSlider = GetComponentInChildren<Slider>();
        if (cooldownSlider == null)
            Debug.LogWarning($"{name}: No Slider found for cooldown!");

        ResetCooldown();
    }

    void Update()
    {
        if (isAttacking)
            return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownSlider != null && lastCooldownMax > 0f)
        {
            float fill = 1f - Mathf.Clamp01(cooldownTimer / lastCooldownMax);
            cooldownSlider.value = fill;
        }

        if (cooldownTimer <= 0f)
        {
            EnemyBehavior target = EnemyBehavior.GetCurrentTarget();
            if (target != null)
            {
                StartCoroutine(PerformAttack(target));
            }
            else
            {
                ResetCooldown();
            }
        }
    }

    float GetEffectiveCooldown()
    {
        float attackSpeed = player.GetAttackSpeed();
        float baseCooldown = player.GetWeaponCooldown();
        float minCooldown = player.GetWeaponMinCooldown();
        float reduction = (baseCooldown - minCooldown) * (attackSpeed / 100f);
        return Mathf.Clamp(baseCooldown - reduction, minCooldown, baseCooldown);
    }

    void ResetCooldown()
    {
        lastCooldownMax = GetEffectiveCooldown();
        cooldownTimer = lastCooldownMax;

        if (cooldownSlider != null)
            cooldownSlider.value = 0f;
    }

    IEnumerator PerformAttack(EnemyBehavior target)
    {
        isAttacking = true;

        // Play animation
        if (animator != null)
        {
            animator.SetBool("IsAttack",true);
            yield return new WaitForSeconds(2.2f); // Replace with animation length
            animator.SetBool("IsAttack", false);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        int damage = player.GetAttackPower();
        Debug.Log($"{player.GetCharacterName()} attacks {target.name} for {damage} damage!");
        target.TakeDamage(damage);

        ResetCooldown();
        isAttacking = false;
    }
}
