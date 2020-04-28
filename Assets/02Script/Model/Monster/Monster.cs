using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
using UnityEditor;

public class Monster : Model
{
    Character Character { set; get; }
    RaycastHit raycastHit = new RaycastHit();
    protected Rect RoamingArea { set; get; }
    protected enum State { roaming, following, battle, attack, getHit, Dead, non }
    protected State NowState { set; get; }
    protected State BeForeState { set; get; }
    Coroutine stateProcess;

    public const float sightRadius = 5f;
    public const float SigthLimitRad = 30f * Mathf.Deg2Rad ;
    float GetNowAngle { get { return GMath.Get360DegToRad(transform.eulerAngles.y); } }


    new protected void Awake()
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
    new protected void Start()
    {
        base.Start();
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
        float nowRoamingDistance = 0f;
        float roamingDistance = 5f;
        float pauseTime = 0f;
        float pauseTimeLimit = 3f;
        while(BeForeState == State.roaming)
        {
            transform.eulerAngles = new Vector3(0f, Random.Range(-180f, 180f), 0f);
            
            while (pauseTime < pauseTimeLimit) 
            {
                if (IsOutRoamingArea)
                {
                    transform.LookAt(GMath.ConvertV2ToV3xz(RoamingArea.center));
                }
                else if (IsDetectedCharacter)
                {
                    NowState = State.following;
                    yield break;
                }

                nowRoamingDistance += SPD * Time.fixedDeltaTime;

                Rigidbody.velocity = nowRoamingDistance < roamingDistance
                    ? transform.forward * SPD : Vector3.zero;

                pauseTime += Rigidbody.velocity != Vector3.zero
                    ? 0f : Time.fixedDeltaTime;

                yield return new WaitForFixedUpdate();
            }
            pauseTime = 0f;
            nowRoamingDistance = 0f;
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
            Debug.DrawRay(transform.position, Character.transform.position - transform.position);
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
    void DoDead()
    {
        Destroy(gameObject);
    }

    public void GetHit(int Damege)
    {
        nowHP -= Damege - DEF;
        if(nowHP <= 0)
        {
            NowState = State.Dead;
        }
    }

    bool IsOutRoamingArea { get { return !RoamingArea.Contains(GMath.ConvertV3xzToV2(transform.position)); } }
    bool IsDetectedCharacter 
    { 
        get 
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, sightRadius, (int)GGameInfo.LayerMasksList.Floor, QueryTriggerInteraction.Ignore);
            foreach(Collider collider in colliders)
            {
                if (collider.CompareTag("Character"))
                {
                    Character = collider.GetComponent<Character>();
                    Vector3 CharacterDirection = Character.transform.position - transform.position;
                    float fowardToCharacterRad = Vector3.Angle(transform.forward, CharacterDirection) * Mathf.Deg2Rad;
                    if(fowardToCharacterRad <= SigthLimitRad)
                    {
                        RaycastHit hit = new RaycastHit();
                        if (Physics.Raycast(transform.position + Vector3.up, CharacterDirection, out hit, Vector3.Distance(transform.position, Character.transform.position)))
                        {
                            print("isIn");
                            if (hit.collider.CompareTag("Character"))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false; 
        } 
    }
    bool IsCloseEnoughWithChracter { get {
            return Vector3.Distance(Character.transform.position, transform.position) <= 1f;
        } }

/*    public void DoAnimation(State state) 
    {
        switch (state)
        {
            case State.roaming:  break;
            case State.following: break;
            case State.roaming:break;
            case State.roaming:break;
        }
    }*/
    protected void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,1,1, 0.5f);
        Gizmos.DrawCube(GMath.ConvertV2ToV3xz(RoamingArea.center), GMath.ConvertV2ToV3xz(RoamingArea.size) + Vector3.up);
        Gizmos.DrawSphere(transform.position, sightRadius);

        Vector2[] vectors = GMath.MoveToRad(GetNowAngle, SigthLimitRad, sightRadius);
        Debug.DrawRay(transform.position, GMath.ConvertV2ToV3xz(vectors[0]), Color.red);
        Debug.DrawRay(transform.position, GMath.ConvertV2ToV3xz(vectors[1]), Color.red);
      }
}
