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
        SetInfo("허름한 마법사", 100, 25, 0, 100, 5);
        base.Awake();
    }
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        Inventory.AddItemForMonster(0, 1, 0.8f);
        Inventory.AddItemForMonster(2, 1, 1f);

        closeEnough = 6f;
        farEnogh = 5f;
    }
    private void OnEnable()
    {
        base.OnEnable();
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

        var nowAttack = skillsMovements[0];
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
