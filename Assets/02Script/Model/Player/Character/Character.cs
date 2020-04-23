using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;

public partial class Character : Model
{
    private StateViewer StateViewer { set; get; }
    public QuickSlot QuickSlot { private set; get; }
    public EquipmentView EquipmentView { private set; get; }
    bool IsinField { set; get; } = true;

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
        StateViewer = StateViewer.GetNew();
        QuickSlot = QuickSlot.GetNew();
        EquipmentView = EquipmentView.GetNew(this);

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
        SetStateViewer();
    }

    private void FixedUpdate()
    {
        FixedUpdateWithMove();
    }

    public void SetStateViewer()
    {
        StateViewer.DrawState(StateViewer.state.hp, HP, nowHP);
        StateViewer.DrawState(StateViewer.state.mp, MP, nowMP);
    }
}
