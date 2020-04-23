using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
public class Monster : Model
{
    protected Rect RoamingArea { set; get; }
    protected enum State { roaming, following, battle, attack, getHit }
    protected State nowState { set; get; }
    protected void Awake()
    {
        RoamingArea = GMath.GetRect(new Vector2(2, 2), new Vector2(2, 2), GMath.ConvertV3xzToV2(transform.position));
        print(RoamingArea);
        nowState = State.roaming;
    }
    // Start is called before the first frame update
    protected void Start()
    {
        
    }

    // Update is called once per frame
    protected void Update()
    {
        
    }

    protected void FixedUpdate()
    {
        
    }
    protected void OnDrawGizmos()
    {
    }
}
