using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
using System;

public class SkulMagician : NormalMonster
{
    public GameObject HitBox;
    public GameObject HitBoxFX;

    public float farEnogh { set; get; }
    new private void Awake()
    {
        SetInfo("허름한 마법사", 100, 250, 0, 10, 5);
        base.Awake();
    }
    new private void OnEnable()
    {
        base.OnEnable();
        closeEnough = 6f;
        farEnogh = 5f;
    }

    new private void OnDisable()
    {
        base.OnDisable();
    }

    new void Update()
    {
        base.Update();
    }

    new private void FixedUpdate()
    {
        base.FixedUpdate();
    }
    new private void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    protected override IEnumerator DoBattle()
    {
        DoAnimator(ActionState.battle);
        while (BeforeState == ActionState.battle)
        {
            transform.LookAt(Character.transform.position);
            yield return new WaitForFixedUpdate();

            if (!IsCloseEnoughWithChracter)
            {
                NowState = ActionState.following;
            }
            else
            {
                if (canAttack)
                    NowState = ActionState.attack;
                else
                {
                    if (!NowAnimatorInfo.IsName("NomalAttack"))
                        if (!IsFarEnoughWithCharacter)
                            Rigidbody.velocity = transform.forward * (-SPD);
                        else
                            Rigidbody.velocity = Vector3.zero;
                }
            }
        }
    }

    protected override IEnumerator DoAttack()
    {
        canAttack = false;
        canGetHit = false;
        Rigidbody.velocity = Vector3.zero;

        DoAnimator(ActionState.attack);
        yield return StartCoroutine(WaitTillAnimator("NomalAttack", true));

        var nowAttack = skillListHandler.skillDatas[0];
        nowAttack.ActivateSkill();

        NowState = ActionState.battle;
        canGetHit = true;
        yield return StartCoroutine(WaitTillAnimator("NomalAttack", false));

        while (nowAttack.isCoolDown)
            yield return new WaitForFixedUpdate();
        canAttack = true;
    }

    new protected virtual bool IsCloseEnoughWithChracter
    {
        get
        {
            return Vector3.Distance(Character.transform.position, transform.position) <= closeEnough;
        }
    }

    protected bool IsFarEnoughWithCharacter
    {
        get
        {
            return Vector3.Distance(Character.transform.position, transform.position) >= farEnogh;
        }
    }
}
