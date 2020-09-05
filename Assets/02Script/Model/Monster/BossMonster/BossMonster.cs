using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;
using GLip;

public class BossMonster : Monster
{
    public BossHPBarViewer BossHPBarViewer { set; get; }
    public GameObject NormalAttackHitBoxObj;
    public GameObject SeedBoomHitBoxObj;
    public GameObject HitBoxFX;
    public GameObject SeedBoomHitBoxFX;

    enum SkillType { NormalAttack, Stinger, SeedBoom, OverDrive }
    SkillData skillNormalAttack { get { return skillsMovements[0]; } }
    SkillData skillOverDrive { get { return skillsMovements[1]; } }
    SkillData skillSeedBoom { get { return skillsMovements[2]; } }
    SkillData skillStinger { get { return skillsMovements[3]; } }
/*    List<bool> canAttackList = new List<bool>();
    bool canDoAttack { set { killMovements[0] = value; } get { return canAttackList[0]; } }
    bool canStinger { set { canAttackList[1] = value; } get { return canAttackList[1]; } }
    bool canSeedBoom { set { canAttackList[2] = value; } get { return canAttackList[2]; } }
    bool canOverDirve { set { canAttackList[3] = value; } get { return canAttackList[3]; } }*/
    bool canMove { set; get; } = true;
    bool canLookAt { set; get; } = true;
    new private void Awake()
    {
        SetInfo("보스", 1000, 1000, 10, 10, 10);
        MonsterSetInfo(new Rect(new Vector2(-500, -500), new Vector2(1000, 1000)));
/*        for (int i = 0; i < 4; i++)
            canAttackList.Add(true);*/
        base.Awake();
    }

    new private void Start()
    {
        base.Start();
        BossHPBarViewer = HPBar.GetComponent<BossHPBarViewer>();
        BossHPBarViewer.BossMonster = this;
        BossHPBarViewer.SetName(CharacterName);
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
        for (int i = 3; i < skillsMovements.Count; i++)
        {
            if (!skillsMovements[i].isCoolDown)
            {
                canAttack = false;
                canMove = false;
                DoAnimator(ActionState.attack);
                switch (i)
                {
                    case 0: StartCoroutine(DoNormalAttack()); break;
                    case 1: StartCoroutine(DoOverDrive()); break;
                    case 2: StartCoroutine(DoSeedBoom()); break;
                    case 3: StartCoroutine(DoStinger()); break;
                }
                return;
            }
        }

        NowState = ActionState.battle;
    }

    int counto;
    protected IEnumerator DoNormalAttack()
    {
        //canMove = false;
        //canDoAttack = false;
        DoSkillAnimation(SkillType.NormalAttack);
        skillsMovements[0].ActivateSkill();
        yield return StartCoroutine(WaitTillAnimator("NormalAttack", true));
        
        yield return StartCoroutine(WaitAttackEnd(SkillType.NormalAttack));
    }

    protected IEnumerator DoStinger() 
    {
        //canMove = false;
        //canStinger = false;
        canLookAt = false;
        skillStinger.ActivateSkill();
        DoSkillAnimation(SkillType.Stinger);
        yield return StartCoroutine(WaitTillAnimator("Stinger", true));
        //WaitTillInterrupt(0);

/*        var targetPosition = Character.transform.position;
        var forward = (targetPosition - transform.position).normalized;
        transform.forward = forward;
        Rigidbody.velocity = transform.forward * 20f;*/

        //WaitTillInterrupt(1);

        yield return StartCoroutine(WaitAttackEnd(SkillType.Stinger));
    }
    
    protected IEnumerator DoSeedBoom() 
    {
        canLookAt = false;
        DoSkillAnimation(SkillType.SeedBoom);
        skillSeedBoom.ActivateSkill();
        yield return StartCoroutine(WaitTillAnimator("SeedBoom", true));

        yield return StartCoroutine(WaitAttackEnd(SkillType.SeedBoom));
    }
    
    protected IEnumerator DoOverDrive() 
    {
        canLookAt = false;

        DoSkillAnimation(SkillType.OverDrive);
        skillOverDrive.ActivateSkill();
        yield return StartCoroutine(WaitTillAnimator("OverDrive", true));

        yield return StartCoroutine(WaitAttackEnd(SkillType.OverDrive));
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

    IEnumerator StartOtherAttackAfter(float time)
    {
        yield return StartCoroutine(WaitTillTimeEnd(time));
        canAttack = true;
    }

    IEnumerator WaitAttackEnd(SkillType skillType)
    {
        NowState = ActionState.battle;
        interrupt = 0;

        switch (skillType)
        {
            case SkillType.NormalAttack:
                yield return StartCoroutine(WaitTillAnimator("NormalAttack", false));
                //canAttack = true;
/*                yield return StartCoroutine(WaitTillTimeEnd(3f));
                canDoAttack = true;*/
                break;
            case SkillType.Stinger: 
                yield return StartCoroutine(WaitTillAnimator("Stinger", false));
                //StartCoroutine(StartOtherAttackAfter(1f));
                canLookAt = true;
                //canAttack = true;
/*                yield return StartCoroutine(WaitTillTimeEnd(4f));
                canStinger = true;*/
                break;
            case SkillType.SeedBoom: 
                yield return StartCoroutine(WaitTillAnimator("SeedBoom", false));
                //StartCoroutine(StartOtherAttackAfter(1f));
                //canAttack = true;
/*                yield return StartCoroutine(WaitTillTimeEnd(5f));
                canSeedBoom = true;*/
                break;
            case SkillType.OverDrive: 
                yield return StartCoroutine(WaitTillAnimator("OverDrive", false));
                canLookAt = true;
/*              StartCoroutine(StartOtherAttackAfter(1f));
                canAttack = true;
                yield return StartCoroutine(WaitTillTimeEnd(5f));
                canOverDirve = true;*/
                break;
        }
        canAttack = true;
        canMove = true;
    }
}
