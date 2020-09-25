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
/*        Inventory.AddItem(0, 20);
        Inventory.AddItem(1, 5);*/
    }
}
