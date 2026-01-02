using System;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField]private Animator anim;
    [SerializeField]private Unit unit;
    
    public event Action<UnitState> AnimationFinished;

    // --- สร้าง ID ของ Parameter เก็บไว้ ---
    private readonly int IsIdleHash = Animator.StringToHash("IsIdle");
    private readonly int IsHittedHash = Animator.StringToHash("IsHitted");
    private readonly int IsAttackHash = Animator.StringToHash("IsAttack");
    private readonly int IsDeadHash = Animator.StringToHash("IsDead");
    private readonly int IsCastHash = Animator.StringToHash("IsCast");
    private readonly int IsDashingHash = Animator.StringToHash("IsDashing");
    // ------------------------------------

    private void Start()
    {
        anim = GetComponent<Animator>();
        unit = GetComponentInParent<Unit>();

        unit.StateChanged += OnStateChanged; // subscribe event
        OnStateChanged(unit.currentState);   // set initial animation
    }

    private void OnStateChanged(UnitState state)
    {
        // reset all bools
        anim.SetBool(IsIdleHash, false);
        anim.SetBool(IsHittedHash, false);
        anim.SetBool(IsAttackHash, false);
        anim.SetBool(IsDeadHash, false);
        anim.SetBool(IsCastHash, false);
        anim.SetBool(IsDashingHash, false); 

        switch(state)
        {
            case UnitState.Idle: anim.SetBool(IsIdleHash, true); break;
            case UnitState.Attack: anim.SetBool(IsAttackHash, true); break;
            case UnitState.Dead: anim.SetBool(IsDeadHash, true); break;
            case UnitState.GetHit: anim.SetBool(IsHittedHash, true); break;
            case UnitState.Cast: anim.SetBool(IsCastHash, true); break;
            case UnitState.Dashing: anim.SetBool(IsDashingHash, true); break; 
        }
    }
    
    public void PlayAnimation(AnimationClip clip)
    {
        if (anim == null || clip == null) return;

        // Animator ต้องมี State ชื่อเดียวกับ clip
       anim.Play(clip.name);
    }
    
    public void PlayCastSkill(int skillIndex, SkillDataSO[] skills)
    {
        if (anim == null || skills == null || skillIndex < 0 || skillIndex >= skills.Length) return;

        AnimationClip clip = skills[skillIndex].skillAnimation;
        if (clip == null) return;

        // สร้าง AnimatorOverrideController runtime
        AnimatorOverrideController overrideController = new AnimatorOverrideController(anim.runtimeAnimatorController);
        overrideController["Cast"] = clip; // "Cast" คือ state ใน Animator
        anim.runtimeAnimatorController = overrideController;

        anim.SetTrigger("Cast"); // trigger ให้เล่น state Cast
    }
    
    public void AnimationEvent_AttackEnd()
    {
        //Debug.Log("Attack Animation Finished Event Fired!");
        AnimationFinished?.Invoke(UnitState.Attack);
    }

    public void AnimationEvent_GetHitEnd()
    {
        AnimationFinished?.Invoke(UnitState.GetHit);
    }

    public void AnimationEvent_DeadEnd()
    {
        AnimationFinished?.Invoke(UnitState.Dead);
    }

    public void AnimationEvent_CastEnd()
    {
        AnimationFinished?.Invoke(UnitState.Cast);
    }
}
