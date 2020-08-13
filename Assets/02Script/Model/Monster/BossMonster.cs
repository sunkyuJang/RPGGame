using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;

public class BossMonster : Monster
{
    public BossHPBarViewer BossHPBarViewer { set; get; }
    public GameObject NormalAttackHitBoxObj;
    public GameObject SeedBoomHitBoxObj;
    public GameObject HitBoxFX;
    int interrupt = 0;
    public void Interrupt(int i) { interrupt = i; }
    enum SkillType { NormalAttack, Stinger, SeedBoom, OverDrive }
    List<bool> canAttackList = new List<bool>();
    bool canDoAttack { set { canAttackList[0] = value; } get { return canAttackList[0]; } }
    bool canStinger { set { canAttackList[1] = value; } get { return canAttackList[1]; } }
    bool canSeedBoom { set { canAttackList[2] = value; } get { return canAttackList[2]; } }
    bool canOverDirve { set { canAttackList[3] = value; } get { return canAttackList[3]; } }
    bool canMove { set; get; } = true;
    bool canLookAt { set; get; } = true;
    new private void Awake()
    {
        SetInfo("보스", 1000, 1000, 10, 10, 10);
        MonsterSetInfo(new Rect(new Vector2(-500, -500), new Vector2(1000, 1000)));
        for (int i = 0; i < 4; i++)
            canAttackList.Add(true);
        base.Awake();
    }

    new private void Start()
    {
        base.Start();
        BossHPBarViewer = HPBar.GetComponent<BossHPBarViewer>();
        BossHPBarViewer.BossMonster = this;
    }
    new private void Update()
    {
        base.Update();
    }

    new private void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override IEnumerator DoBattle()
    {
        DoAnimator(ActionState.battle);
        while (BeforeState == ActionState.battle)
        {
            if(canLookAt)
                transform.LookAt(Character.transform.position);

            yield return new WaitForFixedUpdate();

            if (!IsCloseEnoughWithChracter && canMove)
                NowState = ActionState.following;

            else
            {

                if (canAttack)
                {
                    NowState = ActionState.attack;
                }
            }
        }

        yield break;
    }
    protected override IEnumerator DoAttack()
    {
        SelectNextAttack();
        yield break;
    }

    void SelectNextAttack()
    {
        //print(canAttackList.Count);
        for (int i = 0; i < canAttackList.Count; i++)
        {
            if (canAttackList[i] == true)
            {
                canAttack = false;
                DoAnimator(ActionState.attack);
                switch (i)
                {
                    case 0: StartCoroutine(DoNormalAttack()); break;
                    case 1: StartCoroutine(DoStinger()); break;
                    //case 2: StartCoroutine(DoSeedBoom()); break;
                    //case 3: StartCoroutine(DoOverDrive()); break;
                }
                return;
            }
        }

        NowState = ActionState.battle;
    }

    int counto;
    protected IEnumerator DoNormalAttack()
    {
        print(counto++);
        canMove = false;
        canDoAttack = false;
        DoSkillAnimation(SkillType.NormalAttack);
        yield return StartCoroutine(WaitTillAnimator("NormalAttack", true));

        var NormalHitBoxScrip = NormalAttackHitBoxObj.GetComponent<HitBoxCollider>();

        int hitTIme = 0;
        var lastInterrut = interrupt;
        /*while (hitTIme < 3)
        {
            if (lastInterrut != interrupt)
            {
                lastInterrut = interrupt;
                var colliderBox = HitBoxCollider.StartHitBox(NormalAttackHitBoxObj, transform.forward, null, false);
                var closeOne = colliderBox.GetColsedCollider(transform.position, "Character");
                if (closeOne != null)
                {
                    var Character = closeOne.GetComponent<Model>();
                    StateEffecterManager.EffectToModelBySkill(Character, ATK, null, false);
                }
            }
            else
            {
                WaitTillInterrupt(interrupt == 0 ? 1 : 0);
            }
        }*/

        EndAttack();

        yield return StartCoroutine(WaitTillAnimator("NormalAttack", false));
        canMove = true;
        float startAttackAfter = 1f;
        float startThisAttackAfter = 3f;

        StartCoroutine(StartOtherAttackAfter(startAttackAfter));
        yield return StartCoroutine(WaitTillTimeEnd(startThisAttackAfter));
        canDoAttack = true;

        yield break;
    }

    protected IEnumerator DoStinger() 
    {
        canMove = false;
        canStinger = false;
        canLookAt = false;
        DoSkillAnimation(SkillType.Stinger);
        yield return StartCoroutine(WaitTillAnimator("Stinger", true));
        //WaitTillInterrupt(0);

        var targetPosition = Character.transform.position;
        var forward = (targetPosition - transform.position).normalized;
        transform.forward = forward;
        Rigidbody.velocity = transform.forward * 20f;

        //WaitTillInterrupt(1);
        
        EndAttack();

        yield return StartCoroutine(WaitTillAnimator("Stinger", false));
        canMove = true;
        canLookAt = true;
        float startAttackAfter = 1f;
        float startThisAttackAfter = 5f;

        StartCoroutine(StartOtherAttackAfter(startAttackAfter));
        yield return StartCoroutine(WaitTillTimeEnd(startThisAttackAfter));
        canStinger = true;

        yield break;
    }
    
    protected IEnumerator DoSeedBoom() 
    {
        canAttack = false;
        canSeedBoom = false;
        canLookAt = false;

        DoSkillAnimation(SkillType.SeedBoom);
        yield return StartCoroutine(WaitTillAnimator("SeedBoom", true));

        List<HitBoxCollider> seedbooms = new List<HitBoxCollider>();
        for(int i = 0; i < 3; i++)
        {
            var hitBoxTransform = HitBoxCollider.StartHitBox(SeedBoomHitBoxObj, transform.position, null, false).transform;// Instantiate(SeedBoomHitBoxObj).GetComponent<HitBoxCollider>();

            float ratio = 0.3f;
            float totalSpeed = 0.3f;
            Vector3 firstShotDirction = (hitBoxTransform.forward * ratio + Vector3.up).normalized * totalSpeed;
            Vector3 downAcceleration = (Physics.gravity * Time.fixedDeltaTime);

            while (hitBoxTransform.position.y >= 0f)
            {
                hitBoxTransform.position = hitBoxTransform.position + firstShotDirction + downAcceleration;

                yield return new WaitForFixedUpdate();
                downAcceleration += downAcceleration * Time.fixedDeltaTime;
                print(downAcceleration);

                /*if (hitBox.IsEnteredTrigger)
                {
                    for (int i = 0; i < hitBox.colliders.Count; i++)
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
                }*/
            }
        }
        IEnumerator MoveSeedBoom(Transform transform, float speed) 
        {
            yield break;
        }
        /*while (interrupt == 0)
            yield return new WaitForFixedUpdate();



        while (interrupt == 1)
            yield return new WaitForFixedUpdate();*/
/*
        NowState = ActionState.battle;

        float delayed = 0;
        while (delayed <= 1f)
        {
            delayed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        canAttack = true;

        while (delayed <= 5f)
        {
            delayed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        canSeedBoom = true;*/
    }
    
    protected IEnumerator DoOverDrive() 
    {
        yield break;
    }

    void DoSkillAnimation(SkillType type)
    {
        switch (type)
        {
            case SkillType.NormalAttack: Animator.SetInteger("AttackNum", 0); break;
            case SkillType.Stinger: Animator.SetInteger("AttackNum", 1); break;
            case SkillType.SeedBoom: Animator.SetInteger("AttackNum", 2); break;
            case SkillType.OverDrive: Animator.SetInteger("AttackNum", 3); break;
        }
    }

    IEnumerator WaitTillInterrupt(int n)
    {
        while (interrupt != n) yield return new WaitForFixedUpdate();
    }

    IEnumerator StartOtherAttackAfter(float time)
    {
        yield return StartCoroutine(WaitTillTimeEnd(time));
        canAttack = true;
    }

    void EndAttack()
    {
        NowState = ActionState.battle;
        interrupt = 0;
    }
}
