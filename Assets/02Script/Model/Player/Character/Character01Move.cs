using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;

public partial class Character : Model
{
    Ray ray;
    RaycastHit hit = new RaycastHit();
    bool IsPlayerMove { set; get; }
    Vector3 Direction { set; get; }
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
            return Physics.SphereCast(ray, 2f, out hit, 5f);
        }
    }
    public Model GetExistModel
    {
        get
        {
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
        if(IsPlayerMove) Direction = new Vector3(0, joypadRadian * Mathf.Rad2Deg, 0);
    }
    private void FixedUpdateWithMove()
    {
        if (IsPlayerMove && CanMoving)
        {
            DoAnimator(AnimatorState.Running);
            transform.rotation = Quaternion.Euler(Direction);
            Rigidbody.velocity = transform.forward * SPD;
            //Rigidbody.MovePosition(transform.position + (transform.forward * SPD * Time.fixedDeltaTime));
            //print(transform.forward);
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
