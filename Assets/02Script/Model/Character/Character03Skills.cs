using GLip;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Character : Model
{
    public bool canAttacking = true;
    public bool isManaEnough(SkillData skill) { return nowMP - skill.ManaCost >= 0; }
    void ActivateSkill(SkillData skill)
    {
        if (IsinField)
        {
            if (canAttacking)
            {
                if (isManaEnough(skill))
                {
                    if (!skill.isCoolDown)
                    {
                        if (skill.IsReachedTarget)
                        {
                            StartCoroutine(DeActivateSkill(skill));
                            return;
                        }
                        else
                            ShowAlert("주변에 대상이 없습니다.", Color.red);
                    }
                    else
                        ShowAlert("쿨타임이 남았습니다", Color.red);
                }
                else
                    ShowAlert("마나가 모자릅니다.", Color.red);
            }
        }

        NowState = ActionState.Idle;
    }

    public IEnumerator DeActivateSkill(SkillData skill)
    {
        canAttacking = false;
        canGetHit = false;
        nowMP -= skill.ManaCost;
        RefreshedHPBar();

        SetSkillAnimator(skill, true);

        yield return StartCoroutine(WaitTillInterrupt(1));

        skill.ActivateSkill();

        var lestTime = NowAnimatorInfo.length - (NowAnimatorInfo.normalizedTime * NowAnimatorInfo.length);

        SetSkillAnimator(skill, false);

        NowState = ActionState.Idle;
        canGetHit = true;

        float nowTime = -0.5f;
        while (nowTime < lestTime)
        {
            yield return new WaitForFixedUpdate();
            nowTime += Time.fixedDeltaTime;
        }
        Animator.SetBool(skill.gameObject.name, false);
        canAttacking = true;
    }

    void SetSkillAnimator(SkillData skill, bool isStart)
    {
        Animator.SetBool(skill.gameObject.name, isStart);
        Animator.SetBool(skill.attackType == SkillData.AttackType.Physic ? "IsPhysic" : "IsMagic", isStart);
        if (isStart)
            DoAnimator(AnimatorState.Attak);
    }
}