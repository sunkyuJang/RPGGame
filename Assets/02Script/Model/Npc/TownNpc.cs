using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownNpc : Npc
{
    new void Awake()
    {
        SetInfo("길버트" ,100, 100, 10, 10, 10);
        base.Awake();
    }
    new void Start()
    {
        base.Start();
        Inventory.AddItem(new ItemManager.ItemCounter(ItemManager.Kinds.activeItemList, 0, 20));
        Inventory.AddItem(new ItemManager.ItemCounter(ItemManager.Kinds.activeItemList, 1, 5));
    }
}
