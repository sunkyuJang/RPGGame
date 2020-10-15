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

        AddItem(0, 10);
        AddItem(0, 5);
        AddItem(0, 3);
        AddItem(0, 5);
        AddItem(0, 17);
        AddItem(1, 10);
        AddItem(1, 10);
        AddItem(0, 10);
        AddItem(0, 10);
        AddItem(0, 10);
    }
}
