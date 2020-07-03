using GLip;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Character : Model
{
    public bool canUsingAnotherSkill = true;
    public bool isHitTriggerActivate { private set; get; }
    public void HitTrigger(int i) { isHitTriggerActivate = i == 0 ? false : true; }

    public void ActivateSkill(SkillManager.Skill skill)
    {
        if (IsinField)
        {
            if (canUsingAnotherSkill)
            {
                if (!skill.isCoolDownNow)
                {
                    if (skill.IsThereMonsterAround(this.transform))
                    {
                        NowState = ActionState.Attack;
                        SetSkillAnimator(skill);
                        SkillManager.ActivateSkiil(skill, this);
                        StartCoroutine(DeActivateSkill(skill));
                        return;
                    }
                    else
                        StaticManager.ShowAlert("주변에 대상이 없습니다.", Color.red);
                }
                else
                    StaticManager.ShowAlert("쿨타임이 남았습니다", Color.red);
            }
        }
        
        NowState = ActionState.Idle;
    }
    public IEnumerator DeActivateSkill(SkillManager.Skill skill)
    {
        canUsingAnotherSkill = false;
        
        while(!isHitTriggerActivate)
            yield return new WaitForFixedUpdate();

        while (!NowAnimatorInfo.IsName(skill.data.Name_Eng))
            yield return new WaitForFixedUpdate();

        Animator.SetInteger("SkillTier", 0);
        Animator.SetInteger("SkillIndex", 0);
        Animator.SetBool("IsPhysic" , false);
        Animator.SetBool("IsMagic" , false);
        NowState = ActionState.Idle;

        while (NowAnimatorInfo.IsName(skill.data.Name_Eng))
            yield return new WaitForFixedUpdate();

        canUsingAnotherSkill = true;
        HitTrigger(0);
    }

    void SetSkillAnimator(SkillManager.Skill skill)
    {
        Animator.SetInteger("SkillTier", skill.data.SkillTier);
        Animator.SetInteger("SkillIndex", skill.data.Index);
        Animator.SetBool(skill.data.InfluencedBy == "Physic" ? "IsPhysic" : "IsMagic", true);
        DoAnimator(AnimatorState.Attak);
    }
}
