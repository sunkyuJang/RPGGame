using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
public class Monster : Model
{
    Character Character { set; get; }
    RaycastHit raycastHit = new RaycastHit();
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
                case State.following: StartCoroutine(DoFollowing()); break;
                case State.attack: StartCoroutine(DoAttack()); break;
            }
        }
    }

    IEnumerator DoRoaming()
    {
        float roamingDistance = 5f;
        float nowRoamingDistance = 0f;
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
                if (IsDetectedCharacter)
                {
                    Character = raycastHit.transform.GetComponent<Character>();
                    NowState = State.following;
                    break;
                }
                print(nowRoamingDistance);
            }
            Rigidbody.velocity = Vector3.zero;
            yield return new WaitForSeconds(3f);
        }
    }
    IEnumerator DoFollowing()
    {
        while (BeForeState == State.following) 
        {
            yield return new WaitForFixedUpdate();
            transform.LookAt(Character.transform.position);
            Rigidbody.velocity = transform.forward * SPD;

            if (IsOutRoamingArea)
            {
                NowState = State.roaming;
                break;
            }
            else if(IsCloseEnoughWithChracter)
            {
                Rigidbody.velocity = Vector3.zero;
                NowState = State.attack;
                break;
            }
        }
    }

    IEnumerator DoAttack()
    {
        while(BeForeState == State.attack)
        {
            yield return new WaitForFixedUpdate();
            transform.LookAt(Character.transform.position);
            if (!IsCloseEnoughWithChracter)
            {
                NowState = State.following;
                break;
            }
        }
    }
    bool IsOutRoamingArea { get { return !RoamingArea.Contains(GMath.ConvertV3xzToV2(transform.position)); } }
    bool IsDetectedCharacter { get { 
            if (Physics.SphereCast(transform.position, 3f, transform.forward, out raycastHit, 5f)) 
            {
                if (raycastHit.transform.CompareTag("Character")) 
                {
                    return true;
                }
            } 
            return false; } }
    bool IsCloseEnoughWithChracter { get {
            return Vector3.Distance(Character.transform.position, transform.position) <= 1f;
        } }
    protected void OnDrawGizmos()
    {
        
    }
}
