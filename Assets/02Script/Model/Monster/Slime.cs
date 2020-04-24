using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
public class Slime : Monster
{
    Vector3 nowPosition { set; get; }
    private void Awake()
    {
        SetInfo("허접한 슬라임", 100, 0, 5, 1, 5);
        MonsterSetInfo(GMath.GetRect(GMath.ConvertV3xzToV2(transform.position), new Vector2(8, 8)));
        base.Awake();
        nowPosition = transform.position;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    new private void FixedUpdate()
    {
        base.FixedUpdate();
    }
    new private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 0, 0.5f);
        Gizmos.DrawCube(nowPosition, new Vector3(RoamingArea.width, 0.5f, RoamingArea.height));
    }
}
