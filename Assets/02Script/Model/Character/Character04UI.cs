using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class Character : Model
{
    public GameObject quickSlotPrefab;
    public GameObject equipmentPrefab;
    public GameObject skillViewPrefab;
    public GameObject questViewPrefab;
    public QuickSlot QuickSlot { set; get; }
    public EquipmentView EquipmentView { set; get; }
    public CharacterStateViewer StateViewer { set; get; }
    public CharacterSkiilViewer CharacterSkiilViewer { set; get; }
    public QuestViewer QuestViewer { set; get; }
    public enum UIList { controller, quickSlot, inventory, equipment, skillViewer, stateView, questViewer }
    void AwakeInUI()
    {
        QuickSlot = Instantiate(quickSlotPrefab, GameManager.mainCanvas).GetComponent<QuickSlot>();
        QuickSlot.Character = this;
        EquipmentView = Instantiate(equipmentPrefab, GameManager.mainCanvas).GetComponent<EquipmentView>();
        EquipmentView.SetCharacter(this);
        CharacterSkiilViewer = Instantiate(skillViewPrefab, GameManager.mainCanvas).GetComponent<CharacterSkiilViewer>();
        CharacterSkiilViewer.SetCharater(this);
        QuestViewer = Instantiate(questViewPrefab, GameManager.mainCanvas).GetComponent<QuestViewer>();
        QuestViewer.Character = this;
    }

    public void IntoNormalUI()
    {
        ShowGameUI(UIList.controller, true);
        ShowGameUI(UIList.quickSlot, true);
        ShowGameUI(UIList.stateView, true);
    }

    public void IntoClearUI()
    {
        ShowGameUI(UIList.quickSlot, false);
        ShowGameUI(UIList.stateView, false);
        ShowGameUI(UIList.controller, false);
    }
    public void ShowGameUI(UIList uiList, bool needShow)
    {
        switch (uiList)
        {
            case UIList.quickSlot: QuickSlot.gameObject.SetActive(needShow); break;
            case UIList.inventory: if (needShow) Inventory.ShowInventory(); else Inventory.HideInventory(); break;
            case UIList.equipment: EquipmentView.gameObject.SetActive(needShow); break;
            case UIList.skillViewer: if (needShow) CharacterSkiilViewer.ShowSKillTree(); else CharacterSkiilViewer.HideSkillTree();  break;
            case UIList.stateView: StateViewer.gameObject.SetActive(needShow); break;
            case UIList.controller: controller.SetAllActive(needShow); break;
            case UIList.questViewer: if (needShow) QuestViewer.ShowQuestList(ProcessingQuestList); else QuestViewer.HideQuestList(); break;
        }
    }

    void SetStateView()
    {
        StateViewer = iStateViewerHandler.GetGameObject().GetComponent<CharacterStateViewer>();
        StateViewer.Character = this;
        iStateViewerHandler.RefreshState();
    }
}
