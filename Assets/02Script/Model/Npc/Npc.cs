using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : Model
{
    protected new void Awake()
    {
        isPlayer = false;
        base.Awake();
    }
    new void Start()
    {
        base.Start();
    }
    public void DoTrade() { ShowInventory(); }
    public void DoQuest(int _questIndex){ }
    public bool HasItems { get { return Inventory.HasItemCounters; } }
}
