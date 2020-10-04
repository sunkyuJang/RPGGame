﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
public class Slime : NormalMonster
{
    new private void Awake()
    {
        SetInfo("허접한 슬라임", 100, 0, 15, 100, 5);
        base.Awake();

    }
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    new protected void OnEnable()
    {
        base.OnEnable();
    }

    new private void OnDisable()
    {
        base.OnDisable();
    }

    new private void FixedUpdate()
    {
        base.FixedUpdate();
        SelectedNextAction();
    }
    new private void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    protected override IEnumerator DoAttack()
    {
        canAttack = false;
        canGetHit = false;
        DoAnimator(ActionState.attack);
        
        yield return StartCoroutine(WaitTillAnimator("NomalAttack", true));

        var nowSkill = GetSkillData(SkillDataName + "NormalAttack");
        nowSkill.ActivateSkill();

        while (BeforeState == ActionState.attack)
        {
            yield return new WaitForFixedUpdate();
            transform.LookAt(Character.transform.position);

            if (NowAnimatorInfo.normalizedTime >= 0.9f)
            {
                NowState = ActionState.battle;
                canGetHit = true;
                StartCoroutine(StartAttackDelayTimer(2f));
                break;
            }
        }
    }
}
