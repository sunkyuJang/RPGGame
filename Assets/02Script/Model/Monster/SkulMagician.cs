using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
public class SkulMagician : Monster
{
    public GameObject HitBox;
    public GameObject HitBoxFX;
     
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

    protected override IEnumerator DoAttack()
    {
        canAttack = false;
        canGetHit = false;
        DoAnimator(ActionState.attack);
        while (!NowAnimatorInfo.IsName("NomalAttack"))
            yield return new WaitForFixedUpdate();

        //StateEffecterManager.EffectToModelBySkill(Character, ATK, null, false);

        while (BeforeState == ActionState.attack)
        {
            yield return new WaitForFixedUpdate();
            transform.LookAt(Character.transform.position);

            StartCoroutine(MoveFireBall());
            NowState = ActionState.battle;
            break;
            /*if (NowAnimatorInfo.normalizedTime >= 0.9f)
            {
                NowState = ActionState.battle;
                canGetHit = true;
                StartCoroutine(StartAttackDelayTimer(2f));
                break;
            }*/
        }
    }

    protected IEnumerator MoveFireBall()
    {
        var hitBox = HitBoxCollider.StartHitBox(HitBox, FXStartPoint.position, HitBoxFX, true);
        var hitBoxTransform = hitBox.GetComponent<Transform>();
        
        hitBoxTransform.forward = transform.forward;

        Vector3 firstShotDirction = (hitBoxTransform.forward * 0.5f + Vector3.up).normalized * 0.3f;

        Vector3 downAcceleration = (Physics.gravity * Time.fixedDeltaTime) ;

        while (hitBoxTransform.position.y >= 0f)
        {
            hitBoxTransform.position = hitBoxTransform.position + firstShotDirction + downAcceleration;
            
            yield return new WaitForFixedUpdate();
            downAcceleration += downAcceleration * Time.fixedDeltaTime;
            print(downAcceleration);

            if (hitBox.IsEnteredTrigger)
            {
                for(int i = 0; i < hitBox.colliders.Count; i++)
                {
                    var nowCollider = hitBox.colliders[i];
                    if (nowCollider.CompareTag("Monster"))
                        hitBox.colliders.RemoveAt(i--);
                    else if (nowCollider.CompareTag("Character"))
                    {
                        StateEffecterManager.EffectToModelBySkill(Character, MP * 10, null, false);
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
}
