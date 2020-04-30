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
    protected enum State { roaming, following, battle, attack, getHit, dead, idle}
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
        BeForeState = State.idle;
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
                case State.idle: StartCoroutine(DoIdle(true)); break;
                case State.battle: StartCoroutine(DoIdle(false)); break;
                case State.roaming: StartCoroutine(DoRoaming()); break;
                case State.following: StartCoroutine(DoFollowing()); break;
                case State.attack: StartCoroutine(DoAttack()); break;
            }
        }
    }
    IEnumerator DoRoaming()
    {
        DoAnimator(State.following);
        float roamingDistance = 0f;
        float roamingDistanceLimit = 5f;
/*        float pauseTime = 0f;
        float pauseTimeLimit = 3f;*/
        transform.eulerAngles = new Vector3(0f, Random.Range(-180f, 180f), 0f);
        Rigidbody.velocity = transform.forward * SPD;
        
        while(BeForeState == State.roaming)
        {
            if(roamingDistance < roamingDistanceLimit)
            {
                roamingDistance += SPD * Time.fixedDeltaTime;
            }
            else
            {
                Rigidbody.velocity = Vector3.zero;
                NowState = State.idle;
                yield break;
            }

            yield return new WaitForFixedUpdate();
            
            if (IsOutRoamingArea)
            {
                transform.LookAt(GMath.ConvertV2ToV3xz(RoamingArea.center));
                Rigidbody.velocity = transform.forward * SPD;
            }
            else if (IsDetectedCharacter)
            {
                NowState = State.following;
                yield break;
            }

            /*while (pauseTime < pauseTimeLimit) 
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

                DoAnimator(Rigidbody.velocity != Vector3.zero ? State.following : State.idle);

                pauseTime += Rigidbody.velocity != Vector3.zero
                    ? 0f : Time.fixedDeltaTime;

                yield return new WaitForFixedUpdate();
            }*/
        }
    }
    IEnumerator DoIdle(bool isNomalIdle)
    {
        DoAnimator(isNomalIdle ? State.idle : State.battle);
        float pauseTime = 0f;
        float pauseTimeLimit = isNomalIdle ? 3f : 1f;
        while(BeForeState == State.idle)
        {
            if (pauseTime < pauseTimeLimit)
            {
                pauseTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();

                if (IsDetectedCharacter)
                {
                    NowState = State.following;
                    yield break;
                }
            }
            else
            {
                NowState = State.roaming;
                yield break;
            }
        }
    }
    IEnumerator DoFollowing()
    {
        DoAnimator(State.following);
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
                DoAnimator(State.battle);
                Rigidbody.velocity = Vector3.zero;
                NowState = State.attack;
                break;
            }
            Debug.DrawRay(transform.position, Character.transform.position - transform.position);
        }
    }

    IEnumerator DoAttack()
    {
        DoAnimator(State.attack);
        float pauseTime = 0f;
        float pauseTimeLimit = 1f;
        while (BeForeState == State.attack)
        {
            yield return new WaitForFixedUpdate();

            transform.LookAt(Character.transform.position);

            if (!IsCloseEnoughWithChracter)
            {
                NowState = State.following;
                break;
            }
            
            if (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f)
            {

            }
        }
        print("isOut");
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
            NowState = State.dead;
        }
    }

    void DoAnimator(State state)
    {
        switch (state)
        {
            case State.idle: Animator.SetTrigger("NomalIdle"); break;
            case State.following: Animator.SetTrigger("Walking"); break;
            case State.battle: Animator.SetTrigger("BattleIdle"); break;
            case State.attack: Animator.SetTrigger("Attack"); break;
            case State.getHit: Animator.SetTrigger("GetHit"); break;
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
