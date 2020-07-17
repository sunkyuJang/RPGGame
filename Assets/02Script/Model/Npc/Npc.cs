using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : Model
{
    public int lastQuestNum { set; get; }
    public QuestSheet questList;

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
    public bool HasItems { get { return Inventory.HasItem; } }
    public bool HasQuest 
    { 
        get 
        {
            if (questList != null)
                if (lastQuestNum < questList.sheets[0].list.Count)
                    return true;
            
            return false; 
        } 
    }
}
