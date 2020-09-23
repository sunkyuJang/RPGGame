using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
using System.Linq;

public partial class Character : Model
{
    public static Character instance;
    public QuickSlot QuickSlot { private set; get; }
    public EquipmentView EquipmentView { private set; get; }
    public StateViewer StateViewer { private set; get; }
    public Controller controller { set; get; }
    public int level { private set; get; }
    public int SkillPoint;
    bool IsinField { set; get; } = true;
    public bool GetIsInField { get { return IsinField; } }
    public enum AnimatorState { Idle, Running, Battle, GetHit, Attak, Dead }
    public AnimatorState NowAnimatorState { set; get; } = AnimatorState.Idle;
    new void Awake()
    {
        SetInfo("temp",100, 100, 10, 10, 10);
        base.Awake();
    }
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        QuickSlot = QuickSlot.GetNew(this);
        EquipmentView = EquipmentView.GetNew(this);
        SetStateView();
    }

    new private void FixedUpdate()
    {
        base.FixedUpdate();
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
    public void ShowGameUI(bool isNeedToShow) 
    { 
        controller.SetAllActive(isNeedToShow); 
        QuickSlot.gameObject.SetActive(isNeedToShow);
        StateViewer.gameObject.SetActive(isNeedToShow);
    }

    void SetStateView()
    {
        StateViewer = HPBar.GetComponent<StateViewer>();
        StateViewer.Character = this;
        iStateViewerHandler.RefreshState();
    }

    public void SetCharacterWithPlayerData(PlayerData playerData)
    {
        CharacterName = playerData.NickName;
        
        for(int i = 0; i < playerData.inventoryItem.Count; i++)
        {
            Inventory.AddItem
                (playerData.inventoryItem.Keys.ToList()[i],
                playerData.inventoryItem.Values.ToList()[i]);
        }

        level = playerData.level;
    }
}
