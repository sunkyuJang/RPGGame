﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : Model
{
    public QuestSheet questSheet;
    public int ClearQuestCount = 0;
    protected new void Awake()
    {
        base.Awake();
    }
    new void Start()
    {
        base.Start();
    }
    public bool HasQuest 
    { 
        get 
        {
            if (questSheet != null)
            {
                var questList = questSheet.sheets[0].list;
                if (ClearQuestCount < questList.Count)
                    return true;
            }
            return false; 
        } 
    }
}
