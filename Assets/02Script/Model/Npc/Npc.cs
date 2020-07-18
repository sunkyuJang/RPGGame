using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : Model
{
    public QuestSheet questSheet;

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
            if (questSheet != null)
            {
                var questList = questSheet.sheets[0].list;
                if (questList[questList.Count - 1].DialogueIndex >= lastDialog)
                    return true;
            }
            return false; 
        } 
    }
}
