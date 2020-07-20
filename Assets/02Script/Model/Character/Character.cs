using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;

public partial class Character : Model
{
    Controller Controller { set; get; }
    public QuickSlot QuickSlot { private set; get; }
    public EquipmentView EquipmentView { private set; get; }
    public StateViewer StateViewer { private set; get; }

    public int level { private set; get; }
    bool IsinField { set; get; } = true;
    public enum AnimatorState { Idle, Running, Battle, GetHit, Attak, Dead }
    public AnimatorState NowAnimatorState { set; get; } = AnimatorState.Battle;
    new void Awake()
    {
        SetInfo("temp",100, 100, 10, 10, 10);
        level = 4;
        isPlayer = true;
        base.Awake();
    }
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        QuickSlot = QuickSlot.GetNew(this);
        EquipmentView = EquipmentView.GetNew(this);
        StateViewer = StateViewer.GetNew(this);
        Controller = Controller.GetNew(this);
        DoAnimator(AnimatorState.Battle);

        nowHP = 10;
        Inventory.gold = 1000;

        Inventory.AddItem(0, 5);
        Inventory.AddItem(0, 3);
        Inventory.AddItem(0, 10);
        Inventory.AddItem(1, 8);
        Inventory.AddItem(0, 1);
        Inventory.AddItem(1, 10);
        Inventory.AddItem(2, 1);
        Inventory.AddItem(2, 1);
        Inventory.AddItem(3, 1);
    }

    private void FixedUpdate()
    {
        FixedUpdateInAction();
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
                case AnimatorState.Attak: Animator.SetBool("IsAttack", true); break;
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
        Animator.SetBool("IsAttack", false);
    }
    public void IntoDialogueUi() { Controller.SetAllActive(false); QuickSlot.gameObject.SetActive(false); }
    public void IntoNomalUI() { Controller.SetAllActive(true); QuickSlot.gameObject.SetActive(true); }
}
