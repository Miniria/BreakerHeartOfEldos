using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum UnitState
{
    Idle,
    Attack,
    GetHit,
    Dead,
    Cast,
    Dashing
}

public class ActiveStatusEffect
{
    public StatusEffectSO effectSO;
    public int remainingDuration;
    public GameObject vfxInstance;

    public ActiveStatusEffect(StatusEffectSO so)
    {
        effectSO = so;
        remainingDuration = so.durationInTurns;
    }
}

public class BaseUnit : MonoBehaviour
{
    [Header("Unit Data")]
    public UnitsDataSO unitData;
    [SerializeField] protected string unitName;
    
    protected static float defaultMaxActionGauge = 100f;
    public static float DefaultMaxActionGauge => defaultMaxActionGauge;
    
    [Header("Runtime Stats")]
    public UnitStats currentStats;
    protected UnitStats baseStats; 
    protected float currentMaxHealth;
    public List<SkillDataSO> skills;
    
    [Header("Action Setting")]
    [SerializeField] private float _actionGauge = 0f;
    
    private CombatTextController combatTextController;

    private void Awake()
    {
        combatTextController = GetComponent<CombatTextController>();
        if (combatTextController == null)
        {
            combatTextController = gameObject.AddComponent<CombatTextController>();
        }
    }
    
    public float ActionGauge
    {
        get => _actionGauge;
        set
        {
            if (_actionGauge == value) return;
            _actionGauge = value;
            OnActionGaugeChanged?.Invoke(_actionGauge, DefaultMaxActionGauge);
        }
    }
    [HideInInspector] public float actionGaugeMultiplier;
    public Unit targetUnit;
    
    public UnitState currentState = UnitState.Idle;
    public delegate void OnStateChange(UnitState newState);
    public event OnStateChange StateChanged;
    
    [Header("Setting")]
    [SerializeField] protected Animator _animator;
    [SerializeField] protected Transform unitModelTransform;

    public List<ActiveStatusEffect> activeStatusEffects = new List<ActiveStatusEffect>();

    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnActionGaugeChanged;
    public event Action OnStatusEffectsChanged;

    public void InitializeFromSO()
    {
        if(unitData == null) { Debug.LogError("UnitDataSO is not assigned!", this.gameObject); return; }
        UnitStats calculatedStats = StatCalculator.CalculateBaseStatsFromAttributes(unitData.strength, unitData.agility, unitData.intelligence);
        calculatedStats.health += unitData.stats.health;
        calculatedStats.attack += unitData.stats.attack;
        calculatedStats.defense += unitData.stats.defense;
        calculatedStats.speed += unitData.stats.speed;
        
        // --- แก้ไขการเรียกใช้ให้ถูกต้อง ---
        ApplyBaseStats(calculatedStats);
        
        unitName = unitData.unitName;
        SpawnModel();
        _animator = GetComponentInChildren<Animator>();
        OverrideAnim();
        skills = unitData.skills.Select(skillSO => Instantiate(skillSO)).ToList();
        ActionGauge = 0;
    }

    public void InitializeFromPlayerData(UnitsDataSO so, UnitStats finalStats)
    {
        this.unitData = so;
        ApplyBaseStats(finalStats);
        unitName = unitData.unitName;
        SpawnModel();
        _animator = GetComponentInChildren<Animator>();
        OverrideAnim();
        skills = unitData.skills.Select(skillSO => Instantiate(skillSO)).ToList();
        ActionGauge = 0;
    }
    
    public void ApplyBaseStats(UnitStats newBaseStats)
    {
        baseStats = new UnitStats(newBaseStats);
        RecalculateStats();
    }
    
    public virtual void Attack(Action onActionComplete = null) { }

    public void TakeDamage(float damageAmount)
    {
        DamageResult result = new DamageResult { finalDamage = damageAmount };
        TakeDamage(result, null);
    }

    public void TakeDamage(DamageResult damageResult, Unit attacker)
    {
        if (currentState == UnitState.Dead) return;

        if (!damageResult.wasEvaded && damageResult.finalDamage > 0)
        {
            if (unitData != null && unitData.hitVfxPrefab != null)
            {
                Vector3 spawnPosition = (unitModelTransform != null) ? unitModelTransform.position : transform.position;
                SpawnVFX(unitData.hitVfxPrefab, spawnPosition, Quaternion.identity);
            }
        }

        if (combatTextController != null)
        {
            combatTextController.ShowDamageText(damageResult);
        }
        
        float damageAmount = Mathf.RoundToInt(damageResult.finalDamage);
        currentStats.health -= damageAmount;
        OnHealthChanged?.Invoke(currentStats.health, currentMaxHealth);
        
        if (currentStats.health <= 0)
        {
            currentStats.health = 0;
            ChangeState(UnitState.Dead);
        }
        else
        {
            ChangeState(UnitState.GetHit);
        }
        if (attacker != null)
        {
            ReflectDamage(damageResult.rawDamageToReflect, attacker);
        }
    }

    private void ReflectDamage(float rawDamage, Unit originalAttacker)
    {
        ActiveStatusEffect reflectionEffect = activeStatusEffects.Find(e => e.effectSO.behaviorType == EffectBehaviorType.Reflection);
        if (reflectionEffect != null)
        {
            float reflectionPercent = reflectionEffect.effectSO.reflectionPercentage;
            float damageToReflect = rawDamage * (reflectionPercent / 100f);
            DamageResult reflectedDamageResult = new DamageResult { finalDamage = damageToReflect };
            originalAttacker.TakeDamage(reflectedDamageResult, null);
        }
    }

    public void Heal(float healAmount)
    {
        if (currentState == UnitState.Dead) return;
        currentStats.health += healAmount;
        currentStats.health = Mathf.Min(currentStats.health, currentMaxHealth);
        OnHealthChanged?.Invoke(currentStats.health, currentMaxHealth);
    }

    public void AddStatusEffect(StatusEffectSO effectSO, float power = 0)
    {
        ActiveStatusEffect existingEffect = activeStatusEffects.Find(e => e.effectSO.name == effectSO.name);
        if (existingEffect != null)
        {
            existingEffect.remainingDuration = effectSO.durationInTurns;
        }
        else
        {
            StatusEffectSO runtimeEffectSO = Instantiate(effectSO);
            if (runtimeEffectSO.behaviorType == EffectBehaviorType.Reflection)
            {
                runtimeEffectSO.reflectionPercentage = power;
            }
            
            ActiveStatusEffect newActiveEffect = new ActiveStatusEffect(runtimeEffectSO);

            if (runtimeEffectSO.vfxPrefab != null)
            {
                if (unitModelTransform != null)
                {
                    newActiveEffect.vfxInstance = Instantiate(runtimeEffectSO.vfxPrefab, unitModelTransform);
                }
                else
                {
                    newActiveEffect.vfxInstance = Instantiate(runtimeEffectSO.vfxPrefab, transform.position, transform.rotation);
                }
            }
            
            activeStatusEffects.Add(newActiveEffect);
        }
        RecalculateStats();
        OnStatusEffectsChanged?.Invoke();
    }

    public void RemoveStatusEffect(ActiveStatusEffect effect)
    {
        if (effect == null) return;

        if (effect.vfxInstance != null)
        {
            Destroy(effect.vfxInstance);
        }

        activeStatusEffects.Remove(effect);
        RecalculateStats();
        OnStatusEffectsChanged?.Invoke();
    }

    public void RecalculateStats()
    {
        float healthPercentage = 1f;
        if (currentMaxHealth > 0 && currentStats != null)
        {
            healthPercentage = currentStats.health / currentMaxHealth;
        }

        currentStats = new UnitStats(baseStats);
    
        var percentBonuses = new Dictionary<string, float>();
        foreach (var activeEffect in activeStatusEffects)
        {
            if (activeEffect.effectSO.behaviorType == EffectBehaviorType.StatModifier)
            {
                foreach (var modifier in activeEffect.effectSO.statModifiers)
                {
                    if (modifier.type == StatModifierType.Additive)
                    {
                        ApplyStatModifier(currentStats, modifier.statToModify, modifier.value, true);
                    }
                    else if (modifier.type == StatModifierType.Percentage)
                    {
                        if (!percentBonuses.ContainsKey(modifier.statToModify))
                        {
                            percentBonuses[modifier.statToModify] = 0;
                        }
                        percentBonuses[modifier.statToModify] += modifier.value;
                    }
                }
            }
        }

        foreach (var bonus in percentBonuses)
        {
            ApplyStatModifier(currentStats, bonus.Key, bonus.Value, false);
        }

        currentMaxHealth = currentStats.health;
        currentStats.health = currentMaxHealth * healthPercentage;

        OnHealthChanged?.Invoke(currentStats.health, currentMaxHealth);
    }

    private void ApplyStatModifier(UnitStats stats, string statName, float value, bool isAdditive)
    {
        switch (statName.ToLower())
        {
            case "health": 
                stats.health = isAdditive ? stats.health + value : stats.health * (1 + value / 100f); 
                break;
            case "attack": 
                stats.attack = isAdditive ? stats.attack + value : stats.attack * (1 + value / 100f); 
                break;
            case "defense": 
                stats.defense = isAdditive ? stats.defense + value : stats.defense * (1 + value / 100f); 
                break;
            case "speed": 
                stats.speed = isAdditive ? stats.speed + value : stats.speed * (1 + value / 100f); 
                break;
            case "critical": 
                stats.critical = isAdditive ? stats.critical + value : stats.critical * (1 + value / 100f); 
                break;
            case "criticaldamage": 
                stats.criticalDamage = isAdditive ? stats.criticalDamage + value : stats.criticalDamage * (1 + value / 100f); 
                break;
            case "accurate": 
                stats.accurate += value; 
                break;
            case "evation": 
                stats.evation += value; 
                break;
        }
    }

    public void ProcessStatusEffects()
    {
        List<ActiveStatusEffect> effectsToRemove = new List<ActiveStatusEffect>();
        foreach (var activeEffect in activeStatusEffects)
        {
            if (activeEffect.effectSO.behaviorType == EffectBehaviorType.DamageOverTime)
            {
                TakeDamage(activeEffect.effectSO.damagePerTurn);
            }
            activeEffect.remainingDuration--;
            if (activeEffect.remainingDuration <= 0)
            {
                effectsToRemove.Add(activeEffect);
            }
        }
        if (effectsToRemove.Count > 0)
        {
            foreach (var effect in effectsToRemove)
            {
                RemoveStatusEffect(effect);
            }
        }
    }
    
    public void ChangeState(UnitState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        StateChanged?.Invoke(newState);
    }

    public void ReduceAllSkillCooldowns(float deltaTime)
    {
        if (skills == null) return;
        foreach (var skill in skills)
        {
            skill.currentCooldown = Mathf.Max(0, skill.currentCooldown - deltaTime);
        }
    }

    public void OverrideAnim()
    {
        if (unitData == null || !unitData.hasAnimation) return;
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
        if (_animator == null) return;
        var overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        overrideController["Attack"] = unitData.attack;
        overrideController["Idle"] = unitData.idle;
        overrideController["Hit"] = unitData.gethit;
        overrideController["Dead"] = unitData.dead;
        _animator.runtimeAnimatorController = overrideController;
    }

    public void SpawnModel()
    {
        if (unitModelTransform == null)
        {
            Debug.LogError($"'Unit Model Transform' is not assigned on the prefab for {gameObject.name}. Please re-assign it in the Inspector.", this.gameObject);
            return;
        }
        if (unitData.unitPrefab == null)
        {
            Debug.LogError($"'unitPrefab' is not assigned in the UnitDataSO '{unitData.name}'.", this.gameObject);
            return;
        }
        for (int i = unitModelTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(unitModelTransform.GetChild(i).gameObject);
        }
        GameObject model = Instantiate(unitData.unitPrefab, unitModelTransform);
    }

    public void SpawnVFX(GameObject vfxPrefab, Vector3 position, Quaternion rotation)
    {
        if (vfxPrefab != null)
        {
            Instantiate(vfxPrefab, position, rotation);
        }
    }
}
