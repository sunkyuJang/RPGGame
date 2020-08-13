﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
using System;

public class SkulMagician : NormalMonster
{
    public GameObject HitBox;
    public GameObject HitBoxFX;
    
    public float farEnogh = 3f;
    new private void Awake()
    {
        SetInfo("허름한 마법사", 100, 25, 0, 100, 5);
        MonsterSetInfo(GMath.GetRect(GMath.ConvertV3xzToV2(transform.position), new Vector2(8, 8)));
        base.Awake();
    }
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        Inventory.AddItemForMonster(0, 1, 0.8f);
        Inventory.AddItemForMonster(2, 1, 1f);

        closeEnough = 5f;
    }
    new void Update()
    {
        base.Update();
    }

    new private void FixedUpdate()
    {
        SelectedNextAction();
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
                    {
                        if (!IsFarEnoughWithCharacter)
                        {
                            Rigidbody.velocity = transform.forward * (-SPD);
                        }
                        else
                        {
                            Rigidbody.velocity = Vector3.zero;
                        }
                    }
                    
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
        while (!NowAnimatorInfo.IsName("NomalAttack"))
            yield return new WaitForFixedUpdate();

        while (BeforeState == ActionState.attack)
        {
            yield return new WaitForFixedUpdate();
            transform.LookAt(Character.transform.position);

            StartCoroutine(MoveFireBall());
            NowState = ActionState.battle;
            break;
        }
    }

    protected IEnumerator MoveFireBall()
    {
        var hitBox = HitBoxCollider.StartHitBox(HitBox, FXStartPoint.position, HitBoxFX, true);
        var hitBoxTransform = hitBox.GetComponent<Transform>();
        
        hitBoxTransform.forward = transform.forward;
        float upperDegree = 90f * Mathf.Deg2Rad;
        float ratio = 0.9f;
        float totalSpeed = 0.3f;
        var upperVector2 = GMath.DegreeToVector2(upperDegree);
        print((hitBoxTransform.forward + new Vector3(0, upperVector2.y, 0)));
        Vector3 firstShotDirction = (hitBoxTransform.forward + new Vector3(0, upperVector2.y, 0));
        Vector3 downAcceleration = (Physics.gravity * Time.fixedDeltaTime) ;

        while (hitBoxTransform.position.y >= 0f)
        {
            hitBoxTransform.position = hitBoxTransform.position + firstShotDirction + downAcceleration;
            
            yield return new WaitForFixedUpdate();
            downAcceleration += downAcceleration * Time.fixedDeltaTime;

            if (hitBox.IsEnteredTrigger)
            {
                for(int i = 0; i < hitBox.colliders.Count; i++)
                {
                    var nowCollider = hitBox.colliders[i];
                    if (nowCollider.CompareTag("Monster"))
                        hitBox.colliders.RemoveAt(i--);
                    else if (nowCollider.CompareTag("Character"))
                    {
                        StateEffecterManager.EffectToModelBySkill(Character, MP, null, false);
                        break;
                    }
                }
            }
        }

        Destroy(hitBox.HitBoxFXTransform.gameObject);
        Destroy(hitBox.gameObject);

        yield return new WaitForSeconds(2f);
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
