using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine;
using GLip;

public class BossMonster : Monster
{
    public BossHPBarViewer BossHPBarViewer { set; get; }
    public GameObject ingameSceneChangerPrefab;
    enum SkillType { NormalAttack, Stinger, SeedBoom, OverDrive }
    List<SkillData> skills { set; get; } = new List<SkillData>();
    SkillData skillNormalAttack { get { return skillListHandler.GetSkillData("NormalAttackForBossMonster"); } }
    SkillData skillOverDrive { get { return skillListHandler.GetSkillData("OverDrive"); } }
    SkillData skillSeedBoom { get { return skillListHandler.GetSkillData("SeedBoom"); } }
    SkillData skillStinger { get { return skillListHandler.GetSkillData("Stinger"); } }
    /*    List<bool> canAttackList = new List<bool>();
        bool canDoAttack { set { killMovements[0] = value; } get { return canAttackList[0]; } }
        bool canStinger { set { canAttackList[1] = value; } get { return canAttackList[1]; } }
        bool canSeedBoom { set { canAttackList[2] = value; } get { return canAttackList[2]; } }
        bool canOverDirve { set { canAttackList[3] = value; } get { return canAttackList[3]; } }*/
    bool canMove { set; get; } = true;
    bool canLookAt { set; get; } = true;
    new private void Awake()
    {
        SetInfo("보스", 1000, 100, 30, 10, 10);
        base.Awake();
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => skillListHandler.StartPass);

        skills.Add(skillNormalAttack);
        skills.Add(skillOverDrive);
        skills.Add(skillSeedBoom);
        skills.Add(skillStinger);
    }
    new protected void OnEnable()
    {
        base.OnEnable();
        BossHPBarViewer = iStateViewerHandler.GetGameObject().GetComponent<BossHPBarViewer>();
        BossHPBarViewer.BossMonster = this;
        BossHPBarViewer.SetName(CharacterName);
    }
    new protected void OnDisable()
    {
        base.OnDisable();
    }
    new private void Update()
    {
        base.Update();
    }

    new private void FixedUpdate()
    {
        base.FixedUpdate();
    }
    protected override IEnumerator DoDead()
    {
        IsAlreadyDead = true;
        DoAnimator(ActionState.getHit);

        while (!NowAnimatorInfo.IsName("GetHit"))
            yield return new WaitForEndOfFrame();

        DoAnimator(ActionState.dead);
        Rigidbody.velocity = Vector3.zero;

        while (!NowAnimatorInfo.IsName("Dead"))
            yield return new WaitForEndOfFrame();

        float waitTIme = NowAnimatorInfo.length - (NowAnimatorInfo.normalizedTime * NowAnimatorInfo.length);
        yield return new WaitForSeconds(waitTIme);

        Instantiate(ingameSceneChangerPrefab, transform.position, Quaternion.identity);
        SkillDataGrouper.instance.DestroySkillHitBox();
        Destroy(gameObject);
        //MonsterLocator.MonsterReturn(this);
    }

    protected override IEnumerator DoBattle()
    {
        DoAnimator(ActionState.battle);

        while (BeforeState == ActionState.battle)
        {
            if (canLookAt)
                transform.LookAt(Character.transform.position);
            yield return new WaitForFixedUpdate();

            if (isAllSkillRunning())
                if (!IsCloseEnoughWithChracter && canMove)
                    NowState = ActionState.following;
        }
    }

    bool isAllSkillRunning()
    {
        if (canAttack)
        {
            for (int i = 0; i < skills.Count; i++)
            {
                if (!skills[i].isCoolDown)
                {
                    if (skills[i].IsReachedTarget)
                    {
                        canAttack = false;
                        canMove = false;
                        NowState = ActionState.attack;
                        Rigidbody.velocity = Vector3.zero;
                        switch (i)
                        {
                            case 0: StartCoroutine(DoNormalAttack()); break;
                            case 1: StartCoroutine(DoOverDrive()); break;
                            case 2: StartCoroutine(DoSeedBoom()); break;
                            case 3: StartCoroutine(DoStinger()); break;
                        }
                        return false;
                    }
                }
            }
        }

        return true;
    }

    protected IEnumerator DoNormalAttack()
    {
        DoSkillAnimation(SkillType.NormalAttack, true);
        skillNormalAttack.ActivateSkill();
        yield return new WaitForFixedUpdate();

        yield return StartCoroutine(WaitTillAnimator("NormalAttack", true));

        DoSkillAnimation(SkillType.NormalAttack, false);
        yield return StartCoroutine(WaitAttackEnd(SkillType.NormalAttack));
    }

    protected IEnumerator DoStinger()
    {

        canLookAt = false;
        skillStinger.ActivateSkill();
        DoSkillAnimation(SkillType.Stinger, true);
        yield return new WaitForFixedUpdate();

        yield return StartCoroutine(WaitTillAnimator("Stinger", true));

        DoSkillAnimation(SkillType.Stinger, false);
        yield return StartCoroutine(WaitAttackEnd(SkillType.Stinger));
    }

    protected IEnumerator DoSeedBoom()
    {
        canLookAt = false;
        DoSkillAnimation(SkillType.SeedBoom, true);
        skillSeedBoom.ActivateSkill();
        yield return new WaitForFixedUpdate();

        yield return StartCoroutine(WaitTillAnimator("SeedBoom", true));
        DoSkillAnimation(SkillType.SeedBoom, false);

        yield return StartCoroutine(WaitAttackEnd(SkillType.SeedBoom));
    }

    protected IEnumerator DoOverDrive()
    {

        canLookAt = false;
        DoSkillAnimation(SkillType.OverDrive, true);
        skillOverDrive.ActivateSkill();
        yield return new WaitForFixedUpdate();

        yield return StartCoroutine(WaitTillAnimator("OverDrive", true));

        DoSkillAnimation(SkillType.OverDrive, false);
        yield return StartCoroutine(WaitAttackEnd(SkillType.OverDrive));
    }

    void DoSkillAnimation(SkillType type, bool IsOn)
    {
        if (IsOn)
            DoAnimator(ActionState.attack);

        switch (type)
        {
            case SkillType.NormalAttack: Animator.SetBool("NormalAttack", IsOn); break;
            case SkillType.Stinger: Animator.SetBool("Stinger", IsOn); break;
            case SkillType.SeedBoom: Animator.SetBool("SeedBoom", IsOn); break;
            case SkillType.OverDrive: Animator.SetBool("OverDrive", IsOn); break;
        }
    }

    IEnumerator WaitAttackEnd(SkillType skillType)
    {
        NowState = ActionState.battle;
        interrupt = 0;
        yield return new WaitForFixedUpdate();

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
