using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Character : Model
{
    public bool isHitTriggerActivate { private set; get; }
    public void HitTrigger(int i) { isHitTriggerActivate = i == 0 ? false : true; print("isIn"); }

    public void ActivateSkill(bool isPhysic, int tier, int index )
    {
        Animator.SetInteger("SkillTier", tier);
        Animator.SetInteger("SkillIndex", index);
        Animator.SetBool(isPhysic ? "IsPhysic" : "IsMagic", true);
        DoAnimator(AnimatorState.Attak_Nomal);
    }
    public void DeActivateSkill()
    {
        Animator.SetInteger("SkillTier", 0);
        Animator.SetInteger("SkillIndex", 0);
        Animator.SetBool("IsPhysic" , false);
        Animator.SetBool("IsMagic" , false);
        SetActionState(ActionState.Attack);
    }
}
