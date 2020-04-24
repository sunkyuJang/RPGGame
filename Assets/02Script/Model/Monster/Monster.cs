using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
public class Monster : Model
{
    protected Rect RoamingArea { set; get; }
    protected enum State { roaming, following, battle, attack, getHit, non }
    protected State NowState { set; get; }
    protected State BeForeState { set; get; }
    Coroutine stateProcess;
    protected void Awake()
    {
        base.Awake();
        NowState = State.roaming;
        BeForeState = State.non;
        print(RoamingArea);
    }

    protected void MonsterSetInfo(Rect roamingArea) 
    { 
        RoamingArea = roamingArea; 
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
        if (NowState != BeForeState)
        {
            BeForeState = NowState;
            switch (NowState)
            {
                case State.roaming: StartCoroutine(DoRoaming()); break;

            }
        }
    }

    IEnumerator DoRoaming()
    {
        float roamingDistance = 5f;
        float nowRoamingDistance = 0f;
        bool isOut = false;
        while(BeForeState == State.roaming)
        {
            nowRoamingDistance = 0f;
            transform.eulerAngles = new Vector3(0f, Random.Range(-180f, 180f), 0f);

            while(nowRoamingDistance < roamingDistance)
            {
                nowRoamingDistance += SPD * Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();

                if (IsOutRoamingArea)
                {
                    transform.LookAt(GMath.ConvertV2ToV3xz( RoamingArea.center));
                }
                Rigidbody.velocity = transform.forward * SPD;
                print(nowRoamingDistance);
            }
            Rigidbody.velocity = Vector3.zero;
            yield return new WaitForSeconds(2f);
        }
    }
    bool IsOutRoamingArea { get { return !RoamingArea.Contains(GMath.ConvertV3xzToV2(transform.position)); } }
    protected void OnDrawGizmos()
    {
    }
}
