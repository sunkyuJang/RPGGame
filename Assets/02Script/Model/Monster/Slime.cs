using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster
{
    private void Awake()
    {
        SetInfo("허접한 슬라임", 100, 0, 5, 1, 5);
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(RoamingArea.size.x, 0.5f, RoamingArea.size.y));
        print(RoamingArea);
    }
}
