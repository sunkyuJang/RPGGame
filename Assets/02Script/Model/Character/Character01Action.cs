using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
using System.Runtime.InteropServices;
using TMPro;
using System.Diagnostics.PerformanceData;
using System.Threading;

public partial class Character : Model
{
    Vector3 Rotation { set; get; }
    bool IsPlayerMove { set; get; }
    public void Move(bool isPlayerMove, float joypadRadian)
    {
        if (isPlayerMove) Rotation = new Vector3(0, joypadRadian * Mathf.Rad2Deg, 0);
        if (IsPlayerMove != isPlayerMove)
        {
            IsPlayerMove = isPlayerMove;
            NowState = isPlayerMove ? ActionState.Running : ActionState.Idle;
        }
    }
    float SigthLength { get { return 5f; } }
    float SigthDegLimit { get { return 30f; } }
    Model TargetModel { set; get; }
    Collider[] SurroundingObj { set; get; }
    Collider[] GetSurroundingObj() 
    { 
        return Physics.OverlapSphere(transform.position, SigthLength, (int)GGameInfo.LayerMasksList.Floor, QueryTriggerInteraction.Ignore); 
    }
    Model GetNearestModel(Collider[] colliders)
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
                    if(Dist < beforeDist)
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
    public enum ActionState { Idle, Running, Action, Attack, GetHit, Talk, Trade, Dead }
    ActionState BeforeState { set; get; }
    ActionState NowState { set; get; }
    public void SetActionState(ActionState actionState)
    { 
        if (actionState == ActionState.Action)
        {
            if (BeforeState == ActionState.Idle)
            {
                NowState = ActionState.Action;
            }
        }
    }

    void FixedUpdateInAction()
    {
        //switch
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
                case ActionState.Attack: StartCoroutine(DoAttack()); break; 
                case ActionState.GetHit: StartCoroutine(DoGetHit()); break;
                //case ActionState.Dead: StartCoroutine(DoDead()); break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(AutoAttack());
        }
    }

    IEnumerator AutoAttack()
    {
        while (!Input.GetKeyDown(KeyCode.K)){
            SetActionState(ActionState.Action);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator DoIdle()
    {
        DoAnimator(IsinField ? AnimatorState.Battle : AnimatorState.Idle);
        Rigidbody.velocity = Vector3.zero;
        yield break;
    }
    IEnumerator DoRunning() 
    {
        DoAnimator(AnimatorState.Running);
        while(BeforeState == ActionState.Running)
        {
            transform.rotation = Quaternion.Euler(Rotation);
            Rigidbody.velocity = transform.forward * SPD;
            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator DoAction() 
    {
        if (BeforeState == ActionState.Action)
        {
            SurroundingObj = GetSurroundingObj();
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
        if(npc.dialogue == null) DialogueManager.GetScript(npc);
        IntoDialogueUi();
        DialogueManager.ShowDialogue(npc);
        NowState = ActionState.Idle;
        yield break;
    }
    IEnumerator DoTrade()
    {
        ShowInventory();
        TargetModel.ShowInventory();
        while(BeforeState == ActionState.Trade)
        {
            if(!Inventory.gameObject.activeSelf || !TargetModel.Inventory.gameObject.activeSelf)
            {
                IntoNomalUI();
                Inventory.gameObject.SetActive(false);
                TargetModel.Inventory.gameObject.SetActive(false);
                NowState = ActionState.Idle;
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    bool IsJustStartAttacking { set; get; } = true;
    int Counting { set; get; }
    IEnumerator DoingAttack()
    {
        IsJustStartAttacking = false;
        yield return new WaitForSeconds(NowAnimatorInfo.length);
        IsJustStartAttacking = true;
    }
    IEnumerator DoAttack()
    {
        DoAnimator(AnimatorState.Attak_Nomal);
        while (!NowAnimatorInfo.IsName("NomalAttack"))
            yield return new WaitForEndOfFrame();
        
        float attackTime = NowAnimatorInfo.length - (NowAnimatorInfo.normalizedTime * NowAnimatorInfo.length);

        if (IsJustStartAttacking && NowAnimatorInfo.IsName("NomalAttack") && attackTime >= 0f)
        {
            IsJustStartAttacking = false;
            (TargetModel as Monster).GetHit(ATK);
            //float attackTime = NowAnimatorInfo.length - (NowAnimatorInfo.normalizedTime * NowAnimatorInfo.length);
            //StartCoroutine(DoingAttack());

            yield return new WaitForSeconds(attackTime);
            /*
                        while (!IsJustStartAttacking)
                            yield return new WaitForFixedUpdate();*/

            IsJustStartAttacking = true;
            NowState = ActionState.Idle;
            //print(attackTime);
            print(Counting++);
            yield break;
        }
        NowState = ActionState.Idle;
    }

    public void GetHit(int damege)
    {
        damege -= DEF;
        nowHP -= damege <= 0 ? 0 : damege;
        //DoAnimator(nowHP > 0 ? AnimatorState.GetHit : AnimatorState.Dead);
        NowState = ActionState.GetHit;//nowHP > 0 ? ActionState.GetHit : ActionState.Dead;
    }

    bool isAreadyGetHitting { set; get; } = true;
    IEnumerator DoGetHit() 
    {
        DoAnimator(AnimatorState.GetHit);
        while (!NowAnimatorInfo.IsName("GetHit"))
            yield return new WaitForEndOfFrame();

        if (isAreadyGetHitting && NowAnimatorInfo.IsName("GetHit"))
        {
            isAreadyGetHitting = false;

            while (NowAnimatorInfo.normalizedTime <= 0.9f)
                yield return new WaitForFixedUpdate();

            isAreadyGetHitting = true;
            NowState = BeforeState == ActionState.Attack ? NowState : ActionState.Idle;
        }
        yield break;
    }    
    
    IEnumerator DoDead() 
    {
        DoAnimator(AnimatorState.Dead);
        NowState = ActionState.Idle;
        yield break;
    }
}
