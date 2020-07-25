using GLip;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Character : Model
{
    public bool canAttacking = true;
    public bool isHitTriggerActivate { private set; get; }
    public void HitTrigger(int i) { isHitTriggerActivate = i == 0 ? false : true; }

    void ActivateSkill(SkillManager.Skill skill)
    {
        if (IsinField)
        {
            if (canAttacking)
            {
                if (!skill.isCoolDownNow)
                {
                    if (skill.IsThereMonsterAround(this.transform))
                    {
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
        canGetHit = false;

        SetSkillAnimator(skill);
        yield return new WaitForFixedUpdate();

        while (!isHitTriggerActivate)
        {
            print("isHitTriggerActivate Stuck");
            yield return new WaitForFixedUpdate();
        }

        var lestTime = NowAnimatorInfo.length - (NowAnimatorInfo.normalizedTime * NowAnimatorInfo.length);
        print(lestTime);

        Animator.SetInteger("SkillTier", 0);
        Animator.SetInteger("SkillIndex", 0);
        Animator.SetBool("IsPhysic", false);
        Animator.SetBool("IsMagic", false);
        Animator.SetBool("IsAttack", false);

        NowState = ActionState.Idle;
        canGetHit = true;
        HitTrigger(0);

        float nowTime = -0.5f;
        while (nowTime < lestTime)
        {
            nowTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

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