using GLip;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Character : Model
{
    public bool canAttacking = true;
    public bool isHitTriggerActivate { private set; get; }
    public void HitTrigger(int i) { isHitTriggerActivate = i == 0 ? false : true; }
    public SkillManager.Skill ReservedSkill { set; get; }
    IEnumerator ActivateSkill(SkillManager.Skill skill)
    {
        if (IsinField)
        {
            if (canAttacking)
            {
                if (!skill.isCoolDownNow)
                {
                    if (skill.IsThereMonsterAround(this.transform))
                    {
                        NowState = ActionState.Attack;
                        BeforeState = NowState;
                        SetSkillAnimator(skill);
                        SkillManager.ActivateSkiil(skill, this);
                        StartCoroutine(DeActivateSkill(skill));
                        return;
                    }
                    else
                        ShowAlert("주변에 대상이 없습니다.", Color.red);
                }
                else
                    ShowAlert("쿨타임이 남았습니다", Color.red);
            }
        }
    }

    public IEnumerator DeActivateSkill(SkillManager.Skill skill)
    {
        canAttacking = false;

        while (!isHitTriggerActivate && BeforeState == ActionState.Attack)
        {
            print("isHitTriggerActivate Stuck");
            yield return new WaitForFixedUpdate();
        }

        DoAnimator(AnimatorState.Battle);

        while (NowAnimatorInfo.IsName(skill.data.Name_Eng))
        {
            print("now animation is running Stuck");
            yield return new WaitForFixedUpdate();
        }

        Animator.SetInteger("SkillTier", 0);
        Animator.SetInteger("SkillIndex", 0);
        Animator.SetBool("IsPhysic" , false);
        Animator.SetBool("IsMagic" , false);
        NowState = ActionState.Idle;
        HitTrigger(0);
        
        canAttacking = true;
    }

    void SetSkillAnimator(SkillManager.Skill skill)
    {
        Animator.SetInteger("SkillTier", skill.data.SkillTier);
        Animator.SetInteger("SkillIndex", skill.data.Index);
        Animator.SetBool(skill.data.InfluencedBy == "Physic" ? "IsPhysic" : "IsMagic", true);
        DoAnimator(AnimatorState.Attak);
    }
}