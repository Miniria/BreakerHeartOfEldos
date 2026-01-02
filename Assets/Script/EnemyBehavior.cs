using UnityEngine;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour
{
    private string enemyName;
    private int maxHealth;
    private int currentHP;
    private int shield;
    private float attackSpeed;
    private int attack;
    private int defend;
    private EncounterManager encounterManager;

    [HideInInspector] public bool isSelected = false;
    private static EnemyBehavior currentTarget;

    private Renderer rend;

    private Slider healthSlider;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
    }

    public void Initialize(EnemyData data, EncounterManager manager)
    {
        enemyName = data.enemyName;
        maxHealth = data.maxHP;
        currentHP = maxHealth;
        shield = data.shield;
        attackSpeed = data.attackSpeed;
        attack = data.attack;
        defend = data.defend;
        encounterManager = manager;

        healthSlider = GetComponentInChildren<Slider>();
        if (healthSlider == null)
            Debug.LogWarning($"{name}: No health bar slider found.");

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHP;
        }
        Debug.Log($"Spawned {enemyName} with {currentHP} HP");
    }

    public void TakeDamage(int incomingDamage)
    {
        int damageAfterDefend = Mathf.Max(incomingDamage - defend, 0);

        if (shield > 0)
        {
            int damageToShield = Mathf.Min(shield, damageAfterDefend);
            shield -= damageToShield;
            damageAfterDefend -= damageToShield;
            Debug.Log($"{enemyName}'s shield absorbed {damageToShield} damage.");
        }

        if (damageAfterDefend > 0)
        {
            currentHP -= damageAfterDefend;
            currentHP = Mathf.Max(currentHP, 0);
            Debug.Log($"{enemyName} takes {damageAfterDefend} damage. HP: {currentHP}");

            if (healthSlider != null)
            {
                healthSlider.value = currentHP;
            }
        }

        if (currentHP <= 0)
        {
            Debug.Log($"{enemyName} defeated!");
            encounterManager.OnEnemyDefeated(gameObject);
        }
    }

    void OnMouseDown()
    {
        // Deselect previous
        if (currentTarget != null)
        {
            currentTarget.Deselect();
        }

        // Select this
        currentTarget = this;
        isSelected = true;
        Highlight(true);
        Debug.Log($"{name} selected as target");
    }

    void Deselect()
    {
        isSelected = false;
        Highlight(false);
    }

    void Highlight(bool enable)
    {
        if (rend != null)
        {
            rend.material.color = enable ? Color.yellow : Color.white;
        }
        else
        {
            Debug.LogWarning($"{name}: No Renderer found for highlight!");
        }
    }

    public static EnemyBehavior GetCurrentTarget()
    {
        return currentTarget;
    }

    public static void SetCurrentTarget(EnemyBehavior target)
    {
        if (currentTarget != null)
            currentTarget.Deselect();

        currentTarget = target;
        target.isSelected = true;
        target.Highlight(true);

        Debug.Log($"[Auto] Target set to: {target.name}");
    }

    public static void ClearCurrentTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.Deselect();
            currentTarget = null;
        }
    }
}
