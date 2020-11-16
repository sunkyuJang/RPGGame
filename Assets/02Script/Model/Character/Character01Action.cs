﻿using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GLip;

public partial class Character : Model
{
    Vector3 Rotation { set; get; }
    bool IsPlayerMove { set; get; }
    public void Move(bool isPlayerMove, float joypadRadian)
    {
        if (IsPlayerMove != isPlayerMove || isPlayerMove)
        {
            IsPlayerMove = isPlayerMove;
            if (isPlayerMove)
            {
                Rotation = new Vector3(0, joypadRadian * Mathf.Rad2Deg, 0);
                IsPlayerMove = isPlayerMove;
            }
            else
            {
                Rigidbody.velocity = Vector3.zero;
            }

            NowState = isPlayerMove ? ActionState.Running : ActionState.Idle;
        }
    }
    float SigthLength { get { return 5f; } }
    float SigthDegLimit { get { return 30f; } }
    public Model TargetModel { private set; get; }
    List<Collider> SurroundingObj { set; get; }
    Collider[] GetSurroundingObj()
    {
        return Physics.OverlapSphere(transform.position, SigthLength, (int)GGameInfo.LayerMasksList.Floor, QueryTriggerInteraction.Ignore);
    }
    Model GetNearestModel(List<Collider> colliders)
    {
        Model nearest = null;
        float beforeDist = SigthLength;
        foreach (Collider collider in colliders)
        {
            try
            {
                if (collider.Equals(gameObject.GetComponent<Collider>()))
                    continue;
                else
                {
                    float Dist = Vector3.Distance(transform.position, collider.transform.position);
                    if (Dist < beforeDist)
                    {
                        nearest = collider.GetComponent<Model>();
                        beforeDist = Dist;
                    }
                }
            }
            catch { };

        }
        return nearest;
    }
    bool IsSameObjWithFrontObj(GameObject target)
    {
        Vector3 FromThisTotargetModelDirection = (target.transform.position - transform.position).normalized;
        float DegFowardToTarget = Vector3.Angle(transform.forward, FromThisTotargetModelDirection);
        if (DegFowardToTarget <= SigthDegLimit)
        {
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(transform.position + Vector3.up, FromThisTotargetModelDirection, out hit, Vector3.Distance(transform.position, target.transform.position)))
            {
                if (target.Equals(hit.collider.gameObject))
                {
                    return true;
                }
            }
        }
        return false;
    }
    public enum ActionState { Idle, Running, Action, Attack, GetHit, Talk, Trade, Dead, TimeLine }
    ActionState BeforeState { set; get; }
    public ActionState NowState { private set; get; }
    public void SetActionState(ActionState actionState)
    {
        if (BeforeState != actionState)
        {
            NowState = actionState;
        }
    }
    public SkillData ReservedSkill { set; get; }


    void UpdateInAction()
    {
        //switch
        DoNextAction();
    }

    void DoNextAction()
    {
        if (NowState != BeforeState)
        {
            BeforeState = NowState;
            switch (BeforeState)
            {
                case ActionState.Idle: StartCoroutine(DoIdle()); break;
                case ActionState.Running: StartCoroutine(DoRunning()); break;
                case ActionState.Action: StartCoroutine(DoAction()); break;
                case ActionState.Talk: StartCoroutine(DoTalk()); break;
                case ActionState.Trade: StartCoroutine(DoTrade()); break;
                case ActionState.GetHit: StartCoroutine(DoGetHit()); break;
                case ActionState.Dead: StartCoroutine(DoDead()); break;
                case ActionState.Attack: StartCoroutine(DoAttack()); break;
                case ActionState.TimeLine: StartCoroutine(DoIdle()); break;
                    //case ActionState.Dead: StartCoroutine(DoDead()); break;
            }
        }
    }

    IEnumerator AutoAttack()
    {
        while (!Input.GetKeyDown(KeyCode.K))
        {
            SetActionState(ActionState.Action);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator DoIdle()
    {
        DoAnimator(IsinField ? AnimatorState.Battle : AnimatorState.Idle);
        Rigidbody.velocity = Vector3.zero;

        yield return new WaitForFixedUpdate();
    }
    IEnumerator DoRunning()
    {
        if (!NowAnimatorInfo.IsName("Dead"))
        {
            DoAnimator(AnimatorState.Running);
            while (BeforeState == ActionState.Running)
            {
                if (ConfimBoxManager.instance.confirmBoxPrefabObj.activeSelf)
                {
                    Rigidbody.velocity = Vector3.zero;
                    NowState = ActionState.Idle;
                    break;
                }
                transform.rotation = Quaternion.Euler(Rotation);
                Rigidbody.velocity = transform.forward * SPD;
                yield return new WaitForFixedUpdate();
            }
        }
    }
    IEnumerator DoAction()
    {
        if (BeforeState == ActionState.Action)
        {
            SurroundingObj = GPosition.GetAllCollidersInFOV(transform, SigthLength, SigthDegLimit);
            TargetModel = GetNearestModel(SurroundingObj);
            try
            {
                if (IsSameObjWithFrontObj(TargetModel.gameObject))
                {
                    if (TargetModel is Npc)
                    {
                        NowState = ActionState.Talk;
                    }
                    else if (TargetModel is Monster)
                    {
                        NowState = ActionState.Attack;
                        ReservedSkill = CharacterSkiilViewer.SkillNormalAttack;
                    }
                    yield break;
                }
                NowState = ActionState.Idle;
                yield break;
            }
            catch { }
        }
        yield break;
    }
    IEnumerator DoTalk()
    {
        Npc npc = TargetModel as Npc;
        if (npc.HasDialogue)
        {
            IntoClearUI();
            DialogueManager.instance.ShowDialogue(this, npc);
        }
        NowState = ActionState.Idle;
        yield break;
    }
    IEnumerator DoTrade()
    {
        Controller.SetAllActive(false);
        QuickSlot.gameObject.SetActive(false);
        Inventory.ShowInventoryForTrade(TargetModel.Inventory);
        TargetModel.Inventory.ShowInventoryForTrade(Inventory);

        while (BeforeState == ActionState.Trade)
        {
            if (!Inventory.inventoryFrame.activeSelf || !TargetModel.Inventory.inventoryFrame.activeSelf)
            {
                Controller.SetAllActive(true);
                QuickSlot.gameObject.SetActive(true);
                IntoNormalUI();
                Inventory.inventoryFrame.SetActive(false);
                TargetModel.Inventory.inventoryFrame.SetActive(false);
                NowState = ActionState.Idle;
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator DoAttack()
    {
        print("InAttack");
        ActivateSkill(ReservedSkill);
        yield break;
    }

    bool canGetHit { set; get; } = true;
    public void GetHit()
    {
        if (canGetHit)
            NowState = ActionState.GetHit;
    }

    IEnumerator DoGetHit()
    {
        canGetHit = false;

        DoAnimator(AnimatorState.GetHit);
        yield return new WaitWhile(() => NowAnimatorInfo.IsName("GetHit"));

        if (nowHP > 0)
        {
            NowState = ActionState.Idle;
            canGetHit = true;
        }
        else
        {
            yield return StartCoroutine(DoDead());
        }
        // while (!NowAnimatorInfo.IsName("GetHit"))
        //     yield return new WaitForFixedUpdate();

    }

    bool isAlreadyDead { set; get; }
    IEnumerator DoDead()
    {
        isAlreadyDead = true;
        Rigidbody.velocity = Vector3.zero;
        DoAnimator(AnimatorState.Dead);

        IntoClearUI();

        // yield return new Wa

        ConfimBoxManager.instance.ShowConfirmBoxSimple("체력을 모두 소진하였습니다. \r\n안전장소로 돌아갑니다.");

        yield return new WaitUntil(() => ConfimBoxManager.instance.NowState == ConfimBoxManager.State.Yes);

        var lastSafeZone = LastSafeZone.Keys.ToList()[0];
        LoadSceneManager.LoadScene(GameManager.pathOfScenes + lastSafeZone, this, LastSafeZone.Values.ToList()[0]);
        // if (lastSafeZone != SceneManager.GetActiveScene().name)
        // else
        // {
        //     transform.position = LastSafeZone.Values.ToList()[0];
        // }

        nowHP = HP;
        nowMP = MP;
        RefreshedHPBar();
        IntoNormalUI();

        NowState = ActionState.Idle;
        yield break;
    }

    bool IsActionStateAre(ActionState actionState) { return BeforeState == actionState; }
}

/*    bool IsJustStartAttacking { set; get; } = true;
    int Counting { set; get; }
    IEnumerator DoAttack()
    {
        DoAnimator(AnimatorState.Attak);
        while (!NowAnimatorInfo.IsName("SkillsStateMachine"))
            yield return new WaitForEndOfFrame();

        print("isOver");
        float attackTime = NowAnimatorInfo.length - (NowAnimatorInfo.normalizedTime * NowAnimatorInfo.length);

        if (IsJustStartAttacking && NowAnimatorInfo.IsName("SkillsStateMachine") && attackTime >= 0f)
        {
            IsJustStartAttacking = false;

            yield return new WaitForSeconds(attackTime);

            IsJustStartAttacking = true;
            NowState = ActionState.Idle;
            yield break;
        }
        NowState = ActionState.Idle;
    }*/