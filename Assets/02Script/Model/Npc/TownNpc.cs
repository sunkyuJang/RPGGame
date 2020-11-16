using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownNpc : Npc
{
    new void Awake()
    {
        SetInfo("길버트", 100, 100, 10, 10, 10);
        base.Awake();
    }
    new void OnEnable()
    {
        base.OnEnable();

        AddItem(0, 10);
        AddItem(0, 5);
        AddItem(1, 2);
        AddItem(0, 3);
        AddItem(0, 5);
        AddItem(1, 2);
    }

    public override List<ItemManager.ItemCounter> RequestDialogueToGiveItemAfterIndex()
    {
        List<ItemManager.ItemCounter> list = null;
        if (lastDialog == 10)
        {
            list = new List<ItemManager.ItemCounter>();
            list.Add(new ItemManager.ItemCounter(ItemManager.Instance.GetitemData(3), 1));
            list.Add(new ItemManager.ItemCounter(ItemManager.Instance.GetitemData(4), 1));
            list.Add(new ItemManager.ItemCounter(ItemManager.Instance.GetitemData(0), 5));
            list.Add(new ItemManager.ItemCounter(ItemManager.Instance.GetitemData(1), 5));
        }
        return list;
    }
}
