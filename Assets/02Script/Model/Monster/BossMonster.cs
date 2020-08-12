using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : Monster
{
    public BossHPBarViewer BossHPBarViewer { set; get; }
    public GameObject NormalAttackHitBoxObj;
    public GameObject HitBoxFX;
    int interrupt = 0;
    public void Interrupt(int i) { interrupt = i; }
    enum SkillType { NormalAttack, Stinger, SeedBoom, OverDrive }
    List<bool> canAttackList = new List<bool>();
    bool canDoAttack { set { canAttackList[0] = value; } get { return canAttackList[0]; } }
    bool canStinger { set { canAttackList[1] = value; } get { return canAttackList[1]; } }
    bool canSeedBoom { set { canAttackList[2] = value; } get { return canAttackList[2]; } }
    bool canOverDirve { set { canAttackList[3] = value; } get { return canAttackList[3]; } }

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
            transform.LookAt(Character.transform.position);
            yield return new WaitForFixedUpdate();

            if (!IsCloseEnoughWithChracter)
                NowState = ActionState.following;
            else
            {

                if (canAttack)
                {
                    canAttack = false;
                    NowState = ActionState.attack;
                }
            }
        }

        yield break;
    }
    protected override IEnumerator DoAttack()
    {
        DoAnimator(ActionState.attack);
        SelectNextAttack();
        yield break;
    }

    void SelectNextAttack()
    {
        try
        {
            print(canAttackList.Count);
            for (int i = 0; i < canAttackList.Count; i++)
            {
                if (canAttackList[i] == true)
                {
                    switch (i)
                    {
                        case 0: StartCoroutine(DoAttack()); break;
                        case 1: StartCoroutine(DoStinger()); break;
                        case 2: StartCoroutine(DoSeedBoom()); break;
                        case 3: StartCoroutine(DoOverDrive()); break;
                    }
                    break;
                }
            }
        }
        catch { }
    }

    protected IEnumerator DoNormalAttack()
    {
        canDoAttack = false;
        DoSkillAnimation(SkillType.NormalAttack);
        var NormalHitBoxScrip = NormalAttackHitBoxObj.GetComponent<HitBoxCollider>();

        int hitTIme = 0;
        var lastInterrut = interrupt;
        while (hitTIme < 3)
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
        }

        EndAttack();

        float startAttackAfter = 1f;
        float startThisAttackAfter = 3f;

        StartOtherAttackAfter(startAttackAfter);
        WaitTillTimeEnd(startThisAttackAfter);
        canDoAttack = true;

        yield break;
    }

    protected IEnumerator DoStinger() 
    {
        {// DamageStep
            canStinger = false;

            DoSkillAnimation(SkillType.Stinger);
            WaitTillInterrupt(0);

            var targetPosition = Character.transform.position;
            var forward = (targetPosition - transform.position).normalized;
            transform.forward = forward;
            Rigidbody.velocity = transform.forward * 20f;

            WaitTillInterrupt(1);
        }
        EndAttack();
        
        float startAttackAfter = 1f;
        float startThisAttackAfter = 5f;

        StartOtherAttackAfter(startAttackAfter);

        WaitTillTimeEnd(startThisAttackAfter);
        canStinger = true;

        yield break;
    }
    
    protected IEnumerator DoSeedBoom() 
    {
        canAttack = false;
        canSeedBoom = false;

        DoSkillAnimation(SkillType.SeedBoom);
        while (interrupt == 0)
            yield return new WaitForFixedUpdate();



        while (interrupt == 1)
            yield return new WaitForFixedUpdate();

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
        canSeedBoom = true;
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

    void StartOtherAttackAfter(float time)
    {
        WaitTillTimeEnd(time);
        canAttack = true;
    }

    void EndAttack()
    {
        NowState = ActionState.battle;
        interrupt = 0;
    }
}
