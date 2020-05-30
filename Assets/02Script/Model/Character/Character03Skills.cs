using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Character : Model
{
    public bool isHitTriggerActivate { private set; get; }
    public void HitTrigger(int i) { isHitTriggerActivate = i == 0 ? false : true; }

    public void ActivateSkill(SkillManager.Skill skill)
    {
        print(SkillManager.IsDeActivateSkill(skill));
        if (SkillManager.IsDeActivateSkill(skill))
        {
            Animator.SetInteger("SkillTier", skill.data.SkillTier);
            Animator.SetInteger("SkillIndex", skill.data.Index);
            Animator.SetBool(skill.data.InfluencedBy == "Physic" ? "IsPhysic" : "IsMagic", true);
            DoAnimator(AnimatorState.Attak);
            SkillManager.ActivateSkiil(skill, this);
        }
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
