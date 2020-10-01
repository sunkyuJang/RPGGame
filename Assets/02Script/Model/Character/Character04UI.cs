﻿using JetBrains.Annotations;
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
    public QuickSlot QuickSlot { set; get; }
    public EquipmentView EquipmentView { set; get; }
    public StateViewer StateViewer { set; get; }
    public CharacterSkiilViewer CharacterSkiilViewer { set; get; }
    public enum UIList { controller, quickSlot, inventory, equipment, skillViewer, stateView, all, non }
    void AwakeInUI()
    {
        QuickSlot = Instantiate(quickSlotPrefab, GameManager.mainCanvas).GetComponent<QuickSlot>();
        QuickSlot.Character = this;
        EquipmentView = Instantiate(equipmentPrefab, GameManager.mainCanvas).GetComponent<EquipmentView>();
        EquipmentView.Character = this;
        CharacterSkiilViewer = Instantiate(skillViewPrefab, GameManager.mainCanvas).GetComponent<CharacterSkiilViewer>();
        CharacterSkiilViewer.character = this;
    }

    public void IntoNormalUI()
    {
        ShowGameUI(UIList.controller, true);
        ShowGameUI(UIList.quickSlot, true);
        ShowGameUI(UIList.stateView, true);
    }

    public void IntoDialogueUI()
    {
        ShowGameUI(UIList.all, false);
    }
    public void ShowGameUI(UIList uiList, bool needShow)
    {
        switch (uiList)
        {
            case UIList.quickSlot: QuickSlot.gameObject.SetActive(needShow); break;
            case UIList.inventory: if (needShow) Inventory.ShowInventory(); else Inventory.HideInventory(); break;
            case UIList.equipment: EquipmentView.gameObject.SetActive(needShow); break;
            case UIList.skillViewer: CharacterSkiilViewer.gameObject.SetActive(needShow); break;
            case UIList.stateView: StateViewer.gameObject.SetActive(needShow); break;
            case UIList.controller: controller.SetAllActive(needShow); break;
            case UIList.all:
                foreach (UIList target in Enum.GetValues(typeof(UIList)))
                    ShowGameUI(target, needShow);
                break;
        }
    }

    void SetStateView()
    {
        StateViewer = HPBar.GetComponent<StateViewer>();
        StateViewer.Character = this;
        iStateViewerHandler.RefreshState();
    }
}
