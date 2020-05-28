using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;

public partial class Character : Model
{
    Controller Controller { set; get; }
    public QuickSlot QuickSlot { private set; get; }
    public EquipmentView EquipmentView { private set; get; }
    bool IsinField { set; get; } = true;
    public enum AnimatorState { Idle, Running, Battle, GetHit, Attak_Nomal, Dead }
    public AnimatorState NowAnimatorState { set; get; } = AnimatorState.Idle;
    new void Awake()
    {
        SetInfo("temp",100, 100, 10, 10, 10);
        isPlayer = true;
        base.Awake();
    }
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        StateViewer.GetNew(this);
        QuickSlot = QuickSlot.GetNew();
        EquipmentView = EquipmentView.GetNew(this);
        Controller = Controller.GetNew(this);

        nowHP = 10;
        Inventory.gold = 1000;
        Inventory.AddItem(new ItemManager.ItemCounter(ItemManager.Kinds.activeItemList, 0, 3));
        Inventory.AddItem(new ItemManager.ItemCounter(ItemManager.Kinds.activeItemList, 0, 2));
        Inventory.AddItem(new ItemManager.ItemCounter(ItemManager.Kinds.activeItemList, 1, 1));
        Inventory.AddItem(new ItemManager.ItemCounter(ItemManager.Kinds.activeItemList, 0, 10));
        Inventory.AddItem(new ItemManager.ItemCounter(ItemManager.Kinds.activeItemList, 0, 3));
        Inventory.AddItem(new ItemManager.ItemCounter(ItemManager.Kinds.activeItemList, 0, 25));
        Inventory.AddItem(new ItemManager.ItemCounter(ItemManager.Kinds.activeItemList, 2, 1));
        Inventory.AddItem(new ItemManager.ItemCounter(ItemManager.Kinds.keyItemList, 0, 1));
        Inventory.AddItem(new ItemManager.ItemCounter(ItemManager.Kinds.EquipmentItemList, 0, 1));
        Inventory.AddItem(new ItemManager.ItemCounter(ItemManager.Kinds.EquipmentItemList, 1, 1));
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
                case AnimatorState.Attak_Nomal: Animator.SetBool("IsAttack", true); break;
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
    public void IntoDialogueUi() { Controller.SetAllActive(false); QuickSlot.TurnOn(false); }
    public void IntoNomalUI() { Controller.SetAllActive(true); QuickSlot.TurnOn(true); }
}
