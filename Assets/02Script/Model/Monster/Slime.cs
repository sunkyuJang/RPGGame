﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
public class Slime : Monster
{
    new private void Awake()
    {
        SetInfo("허접한 슬라임", 100, 0, 15, 100, 5);
        MonsterSetInfo(GMath.GetRect(GMath.ConvertV3xzToV2(transform.position), new Vector2(8, 8)));
        base.Awake();
    }
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        Inventory.AddItemForMonster(0, 1, 0.8f);
        Inventory.AddItemForMonster(2, 1, 1f);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    new private void FixedUpdate()
    {
        base.FixedUpdate();
        //SelectedNextAction();
    }
    new private void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    new void SelectedNextAction()
    {
        if(BeforeState != NowState)
        {
            BeforeState = NowState;
            switch (BeforeState)
            {
                case ActionState.idle: StartCoroutine(DoIdle()); break;
                case ActionState.roaming: StartCoroutine(DoRoaming()); break;
            }
        }
    }
}
