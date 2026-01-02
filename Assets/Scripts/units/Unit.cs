using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Unit : BaseUnit
{
    public event Action OnUnitDead;
    public List<SkillDataSO> selectedSkill = new List<SkillDataSO>();
    
    private AnimationController _animController;
    private Vector3 originalPosition;
    private bool isAttackAnimationFinished = false;
    private Action onCurrentActionComplete;

    private void Awake()
    {
        originalPosition = transform.position;
    }

    void Start()
    {
        _animController = GetComponentInChildren<AnimationController>();
        if (_animController != null)
        {
            _animController.AnimationFinished += OnAnimationFinished;
        }
        selectedSkill.RemoveAll(s => s == null);
    }

    private void Update()
    {
        ReduceAllSkillCooldowns(Time.deltaTime);
    }
    
    public override void Attack(Action onActionComplete = null)
    {
        Unit target = TargetingSystem.FindTargets(this, SkillTargetType.Random_Enemy).FirstOrDefault();
        if (target == null)
        {
            Debug.LogWarning($"{unitName} has no target to attack! Ending turn.");
            onActionComplete?.Invoke();
            ChangeState(UnitState.Idle);
            return;
        }
        
        targetUnit = target;
        this.onCurrentActionComplete = onActionComplete;
        StartCoroutine(DashAttackSequence(target));
    }

    private IEnumerator DashAttackSequence(Unit target)
    {
        isAttackAnimationFinished = false;
        transform.LookAt(target.transform);
        Vector3 directionToTarget = (transform.position - target.transform.position).normalized;
        Vector3 attackPosition = target.transform.position + directionToTarget * unitData.attackDistance;

        ChangeState(UnitState.Dashing);
        while (Vector3.Distance(transform.position, attackPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, attackPosition, unitData.dashSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = attackPosition;

        ChangeState(UnitState.Attack);
        yield return new WaitUntil(() => isAttackAnimationFinished);

        ChangeState(UnitState.Dashing);
        while (Vector3.Distance(transform.position, originalPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, unitData.dashSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = originalPosition;

        ActionGauge = 0;
        ChangeState(UnitState.Idle);
        this.onCurrentActionComplete?.Invoke();
        this.onCurrentActionComplete = null;
    }

    public void StartTurn()
    {
        ProcessStatusEffects();

        // ตรวจสอบว่าเป็นยูนิตของผู้เล่นและอยู่ในโหมด Auto หรือไม่
        if (this == GameManager.Instance.playerUnit && GameManager.Instance.isAutoMode)
        {
            DecideAutoSkill();
        }

        if (selectedSkill.Count > 0 && selectedSkill[0] != null)
        {
            SkillActivate();
        }
        else
        {
            Attack(onCurrentActionComplete);
        }
    }

    /// <summary>
    /// เมธอดสำหรับตัดสินใจใช้สกิลในโหมด Auto
    /// </summary>
    private void DecideAutoSkill()
    {
        // ล้างสกิลที่อาจจะค้างอยู่จากการกดของผู้เล่น
        selectedSkill.Clear(); 

        // วนลูปจากสกิลท้ายสุด (สกิล 3) มาสกิลแรก (สกิล 1)
        for (int i = skills.Count - 1; i >= 0; i--)
        {
            SkillDataSO skill = skills[i];
            if (skill.currentCooldown <= 0)
            {
                Debug.Log($"[Auto Mode] Found available skill: {skill.skillName}. Selecting it.");
                selectedSkill.Add(skill);
                return; // เจอสกิลที่ใช้ได้แล้ว, เลือกและออกจากเมธอด
            }
        }
        Debug.Log("[Auto Mode] No skills available. Will perform normal attack.");
    }
    
    private void OnAnimationFinished(UnitState finishedState)
    {
        switch (finishedState)
        {
            case UnitState.Attack:
                if (targetUnit != null)
                {
                    DamageResult result = DamageCalculator.CalculateDamage(this, targetUnit);
                    targetUnit.TakeDamage(result, this);
                    targetUnit = null;
                }
                isAttackAnimationFinished = true;
                break;

            case UnitState.GetHit:
                ChangeState(UnitState.Idle);
                onCurrentActionComplete?.Invoke();
                onCurrentActionComplete = null;
                break;

            case UnitState.Dead:
                Die();
                break;
            
            case UnitState.Cast:
                if (selectedSkill.Count > 0 && selectedSkill[0] != null)
                {
                    SkillExecutor.Execute(this, selectedSkill[0]);
                    SkillDataSO skillToUse = selectedSkill[0];
                    skillToUse.currentCooldown = skillToUse.cooldown;
                    selectedSkill.RemoveAt(0);
                }
                
                ActionGauge = 0;
                ChangeState(UnitState.Idle);
                onCurrentActionComplete?.Invoke();
                onCurrentActionComplete = null;
                break;
        }
    }

    public void Die()
    {
        OnUnitDead?.Invoke();
    }

    public void SkillActivate()
    {
        if (selectedSkill.Count == 0 || selectedSkill[0] == null) return;
        
        SkillDataSO skill = selectedSkill[0];
        if (skill.vfxPrefab != null)
        {
            List<Unit> targets = TargetingSystem.FindTargets(this, skill.targetType);
            if (skill.spawnVfxOnCaster || targets.Count > 1)
            {
                SpawnVFX(skill.vfxPrefab, transform.position, transform.rotation);
            }
            else if (targets.Count > 0)
            {
                SpawnVFX(skill.vfxPrefab, targets[0].transform.position, targets[0].transform.rotation);
            }
        }
        
        ChangeState(UnitState.Cast);
    }
}
