﻿using GLip;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Character : Model
{
    public bool canAttacking = true;
    public bool isHitTriggerActivate { private set; get; }
    public void HitTrigger(int i) { isHitTriggerActivate = i == 0 ? false : true; }

    void ActivateSkill(SkillData skill)
    {
        if (IsinField)
        {
            if (canAttacking)
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
        }
    }

    public IEnumerator DeActivateSkill(SkillData skill)
    {
        canAttacking = false;
        canGetHit = false;

        SetSkillAnimator(skill, true);
        yield return new WaitForFixedUpdate();

        while (!isHitTriggerActivate)
        {
            //print("isHitTriggerActivate Stuck");
            yield return new WaitForFixedUpdate();
        }

        skill.ActivateSkill();

        var lestTime = NowAnimatorInfo.length - (NowAnimatorInfo.normalizedTime * NowAnimatorInfo.length);
        //print(lestTime);

        SetSkillAnimator(skill, false);

        NowState = ActionState.Idle;
        canGetHit = true;
        HitTrigger(0);

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
        if(isStart)
            DoAnimator(AnimatorState.Attak);
    }
}