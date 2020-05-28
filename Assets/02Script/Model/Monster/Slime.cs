using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
public class Slime : Monster
{
    new private void Awake()
    {
        SetInfo("허접한 슬라임", 100, 0, 15, 1, 5);
        MonsterSetInfo(GMath.GetRect(GMath.ConvertV3xzToV2(transform.position), new Vector2(8, 8)));
        base.Awake();
    }
    // Start is called before the first frame update
    new void Start()
    {
        
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    new private void FixedUpdate()
    {
        base.FixedUpdate();
    }
    new private void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}
