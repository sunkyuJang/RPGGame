using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
using UnityEditorInternal;

public partial class Character : Model
{
    Ray ray;
    RaycastHit hit = new RaycastHit();
    float rayDist = 3f;
    bool IsPlayerMove { set; get; }
    Vector3 Rotation { set; get; }
    public enum AnimatorState { Idle, Running, Battle, GetHit, Attak_Nomal }
    AnimatorState NowAnimatorState { set; get; } = AnimatorState.Idle;
    bool CanMoving { get { return !NowAnimatorInfo.IsName("NomalAttack"); } }
    bool CanAttack { get { return NowAnimatorInfo.IsName("BattleIdle"); } }
    bool IsAttaking { set; get; } = false;

    private bool isObjectExist
    {
        get
        {
            ray = new Ray(transform.position, transform.forward);
            Debug.DrawRay(transform.position + Vector3.up, transform.forward * rayDist);
            return Physics.Raycast(transform.position + Vector3.up, transform.forward * rayDist, out hit);
        }
    }
    public Model GetExistModel
    {
        get
        {
            print(isObjectExist);
            if (isObjectExist)
            {
                Model model = hit.collider.gameObject.GetComponent<Model>();
                if (model != null) { return model; }
            }
            return null;
        }
    }
    public void Move(bool isPlayerMove, float joypadRadian)
    {
        IsPlayerMove = isPlayerMove;
        if(IsPlayerMove) Rotation = new Vector3(0, joypadRadian * Mathf.Rad2Deg, 0);
    }
    public void GetHit(int Damege)
    {
        DoAnimator(AnimatorState.GetHit);
        Damege -= DEF;
        nowHP -= Damege <= 0 ? 0 : Damege;
        SetStateViewer();
    }
    private void FixedUpdatePartialMove()
    {
        if (IsPlayerMove && CanMoving)
        {
            DoAnimator(AnimatorState.Running);
            transform.rotation = Quaternion.Euler(Rotation);
            Rigidbody.velocity = transform.forward * SPD;
        }
        else
        {
            if (!IsAttaking)
            {
                DoAnimator(IsinField ? AnimatorState.Battle : AnimatorState.Idle);
            }
        }
    }

    public void DoAnimator(AnimatorState action)
    {
        if (NowAnimatorState != action)
        {
            ResetAnimatorState();
            switch (action)
            {
                case AnimatorState.Idle: Animator.SetBool("IsIdle", true); break;
                case AnimatorState.Running: Animator.SetBool("IsRunning", true); break;
                case AnimatorState.Battle: Animator.SetBool("IsBattle", true); break;
                case AnimatorState.GetHit: Animator.SetBool("IsGetHit", true); break;
                case AnimatorState.Attak_Nomal: Animator.SetBool("IsAttak_Nomal", true); break;
            }
            NowAnimatorState = action;
        }
    }
    public void ResetAnimatorState()
    {
        Animator.SetBool("IsRunning", false);
        Animator.SetBool("IsIdle", false);
        Animator.SetBool("IsBattle", false);
        Animator.SetBool("IsGetHit", false);
        Animator.SetBool("IsAttak_Nomal", false);
    }
    public void DoAttackMotion()
    {
        if (CanAttack)
        {
            StartCoroutine(CountingAttackTime());
        }
    }
    IEnumerator CountingAttackTime()
    {
        if (!IsAttaking) //locking
        {
            IsAttaking = true;
            DoAnimator(AnimatorState.Attak_Nomal);

            while (!NowAnimatorInfo.IsName("NomalAttack"))
                yield return new WaitForFixedUpdate();

            while (NowAnimatorInfo.normalizedTime < 1f)
                yield return new WaitForFixedUpdate();

            yield return new WaitForFixedUpdate();
            DoAnimator(AnimatorState.Battle);
            IsAttaking = false;
        }
    }
}
