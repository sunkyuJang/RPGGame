using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;

public partial class Character : Model
{
    Vector3 Rotation { set; get; }
    public void Move(bool isPlayerMove, float joypadRadian)
    {
        NowState = isPlayerMove ? ActionState.Running : ActionState.Idle;
        if (isPlayerMove) Rotation = new Vector3(0, joypadRadian * Mathf.Rad2Deg, 0);
    }
    Model TargetModel { set; get; }
    Collider[] SurroundingObj { set; get; }
    Collider[] GetSurroundingObj() 
    { 
        return Physics.OverlapSphere(transform.position, SigthLength, (int)GGameInfo.LayerMasksList.Floor, QueryTriggerInteraction.Ignore); 
    }
    Model GetNearestModel(Collider[] colliders)
    {
        Model nearest = null;
        foreach (Collider collider in colliders)
        {
            try
            {
                if (collider.Equals(gameObject.GetComponent<Collider>())) 
                    continue;
                else
                {
                    nearest = nearest == null ? collider.GetComponent<Model>()
                        : collider.transform.position.z < nearest.transform.position.z ? collider.GetComponent<Model>() : nearest;
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
    float SigthLength { get { return 5f; } }
    float SigthDegLimit { get { return 30f; } }
    public enum ActionState { Idle, Running, Action, Attack, GetHit, Talk, Trade, Dead }
    ActionState BeforeState { set; get; }
    ActionState NowState { set; get; }
    public void SetActionState(ActionState actionState) 
    { 
        NowState = actionState;
        FixedUpdateInAction();
    }
    void FixedUpdateInAction()
    {
        if(NowState != BeforeState)
        {
            BeforeState = NowState;
            StopAllCoroutines();
            switch (BeforeState)
            {
                case ActionState.Idle: StartCoroutine(DoIdle()); break;
                case ActionState.Running: StartCoroutine(DoRunning()); break;
                case ActionState.Action: StartCoroutine(DoAction()); break;
                case ActionState.Talk: StartCoroutine(DoTalk()); break;
                case ActionState.Trade: StartCoroutine(DoTrade()); break;
                case ActionState.Attack: StartCoroutine(DoAttack()); break;
                case ActionState.GetHit: StartCoroutine(DoGetHit()); break;
                case ActionState.Dead: StartCoroutine(DoDead()); break;
            }
        }
    }
    IEnumerator DoIdle()
    {
        DoAnimator(IsinField ? AnimatorState.Battle : AnimatorState.Idle);
        Rigidbody.velocity = Vector3.zero;
        yield return null;
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
        SurroundingObj = GetSurroundingObj();
        TargetModel = GetNearestModel(SurroundingObj);

        if (IsSameObjWithFrontObj(TargetModel.gameObject))
        {
            if(TargetModel is Npc)
            {
                SetActionState(ActionState.Talk);
            }
            else if(TargetModel is Monster)
            {
                SetActionState(ActionState.Attack);
            }
            yield break;
        }

        NowState = ActionState.Idle;
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
    IEnumerator DoAttack()
    {
        DoAnimator(AnimatorState.Attak_Nomal);
        (TargetModel as Monster).GetHit(ATK);
        while(BeforeState == ActionState.Attack)
        {
            while (!NowAnimatorInfo.IsName("NomalAttack"))
                yield return new WaitForFixedUpdate();

            while(NowAnimatorInfo.normalizedTime <= 0.80f)
                yield return new WaitForFixedUpdate();
        }
        NowState = ActionState.Idle;
        yield break;
    }
    public void GetHit(int damege)
    {
        nowHP -= damege - DEF;
        NowState = nowHP > 0 ? ActionState.GetHit : ActionState.Dead;
    }
    IEnumerator DoGetHit() 
    {
        DoAnimator(AnimatorState.GetHit);
        yield break;
    }    
    
    IEnumerator DoDead() 
    {
        DoAnimator(AnimatorState.Dead);
        yield break;
    }
}
