using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
using UnityEditor;
using UnityEngine.UIElements;
using JetBrains.Annotations;
using System.Runtime.Serialization;

public class Monster : Model
{
    Character Character { set; get; }
    MonsterHPBarViewer HPViewer { set; get; }
    public Transform HPBarPositionGuide { private set; get; }
    bool CanShowingHPBar 
    { 
        get 
        {
            Vector3 HPBarPositionToScreen = Camera.main.WorldToScreenPoint(HPBarPositionGuide.position);
            if(HPViewer.MinDist <= HPBarPositionToScreen.z && HPBarPositionToScreen.z < HPViewer.MaxDist)
            {
                return true;
            }
            return false;
        } 
    }
    protected Rect RoamingArea { set; get; }
    protected enum State { roaming, following, battle, attack, getHit, dead, idle}
    protected State NowState { set; get; }
    protected State BeforeState { set; get; }

    public const float sightRadius = 5f;
    public const float SigthLimitRad = 30f * Mathf.Deg2Rad ;
    float GetNowAngle { get { return GMath.Get360DegToRad(transform.eulerAngles.y); } }
    
    float attackDelayTimer = 0f;
    bool canAttack = true;
    public Transform FXStartPoint { private set; get; }

    new protected void Awake()
    {
        base.Awake();
        NowState = State.roaming;
        BeforeState = State.idle;
        HPViewer = MonsterHPBarViewer.GetNew(this, GameObject.Find("Canvas").GetComponent<Transform>());
        HPBarPositionGuide = transform.Find("HPBarPositionGuide");
        FXStartPoint = transform.Find("FXStartPoint");
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
        if(CanShowingHPBar != HPViewer.gameObject.activeSelf)
        {
            HPViewer.gameObject.SetActive(CanShowingHPBar);
        }
    }

    protected void FixedUpdate()
    {
        if (NowState != BeforeState)
        {
            BeforeState = NowState;
            switch (NowState)
            {
                case State.idle: StartCoroutine(DoIdle()); break;
                case State.battle: StartCoroutine(DoBattle()); break;
                case State.roaming: StartCoroutine(DoRoaming()); break;
                case State.following: StartCoroutine(DoFollowing()); break;
                case State.attack: StartCoroutine(DoAttack()); break;
                //case State.getHit: StartCoroutine(DoGetHit()); break;
            }
        }
        Debug.DrawRay(Camera.main.transform.position, Camera.main.WorldToScreenPoint(HPBarPositionGuide.position));
    }

    IEnumerator DoIdle()
    {
        DoAnimator(State.idle);
        float pauseTime = 0f;
        float pauseTimeLimit = 3f;
        while(BeforeState == State.idle)
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
    IEnumerator DoRoaming()
    {
        DoAnimator(State.following);
        float roamingDistance = 0f;
        float roamingDistanceLimit = 5f;
        transform.eulerAngles = new Vector3(0f, Random.Range(-180f, 180f), 0f);
        Rigidbody.velocity = transform.forward * SPD;

        while (BeforeState == State.roaming)
        {
            if (roamingDistance < roamingDistanceLimit)
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
        }
    }
    IEnumerator DoBattle()
    {
        DoAnimator(State.battle);
        while(BeforeState == State.battle)
        {
            transform.LookAt(Character.transform.position);
            yield return new WaitForFixedUpdate();

            if (!IsCloseEnoughWithChracter)
            {
                NowState = State.following;
                break;
            }
            
            if (canAttack)
            {
                NowState = State.attack;
                break;
            }
        }
    }
    IEnumerator DoFollowing()
    {
        DoAnimator(State.following);
        while (BeforeState == State.following) 
        {
            transform.LookAt(Character.transform.position);
            Rigidbody.velocity = transform.forward * SPD;
            yield return new WaitForFixedUpdate();

            if (IsOutRoamingArea)
            {
                NowState = State.roaming;
                break;
            }
            
            if(IsCloseEnoughWithChracter)
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
        while (!NowAnimatorInfo.IsName("Attack01"))
            yield return new WaitForFixedUpdate();
        
        Character.GetHit(ATK);
        while (BeforeState == State.attack)
        {
            yield return new WaitForFixedUpdate();
            transform.LookAt(Character.transform.position);

            if (NowAnimatorInfo.normalizedTime >= 0.9f)
            {
                NowState = State.battle;
                StartCoroutine(StartAttackDelayTimer(1.5f));
                break;
            }
        }
    }
    IEnumerator StartAttackDelayTimer(float limit)
    {
        canAttack = false;
        while (attackDelayTimer <= limit) 
        {
            attackDelayTimer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        attackDelayTimer = 0f;
        canAttack = true;
    }
    public void GetHit(Character from, float damege, GameObject HitFX, bool isFXStartFromGround)
    {
        damege -= DEF;
        damege = (int)(damege > 0 ? damege : 0);
        StaticManager.ShowAlert(((int)damege).ToString(), Color.red, Camera.main.WorldToScreenPoint(transform.position + (Vector3.up * 2)));
        NowState = State.getHit;
        StartCoroutine(DoGetHit());
        if (HitFX != null)
            StartCoroutine(ControllHitFX(HitFX, isFXStartFromGround));
    }
    IEnumerator ControllHitFX(GameObject HitFX, bool isFXStartFromGround)
    {
        Transform FXtransform = Instantiate(HitFX).transform;
        FXtransform.position = isFXStartFromGround ? transform.position : FXStartPoint.position;
        FXtransform.forward = transform.forward;

        ParticleSystem particle = FXtransform.GetComponent<ParticleSystem>();
        while (particle.IsAlive())
            yield return new WaitForFixedUpdate();

        Destroy(FXtransform.gameObject);
    }
    IEnumerator DoGetHit()
    {
        DoAnimator(State.getHit);
        while (!NowAnimatorInfo.IsName("GetHit"))
            yield return new WaitForEndOfFrame();

        if(nowHP <= 0)
        {
            DoAnimator(State.dead);
            Rigidbody.velocity = Vector3.zero;
            while (!NowAnimatorInfo.IsName("Dead"))
                yield return new WaitForEndOfFrame();

            float waitTIme = NowAnimatorInfo.length - (NowAnimatorInfo.normalizedTime * NowAnimatorInfo.length);
            yield return new WaitForSeconds(waitTIme);
            Destroy(HPViewer.gameObject);
            Destroy(gameObject);
            DropItem();
        }

        NowState = State.battle;
        yield break;
    }

    void DropItem()
    {
        var probablility = Random.Range(0, 100) * 0.01f;
        print(probablility);
        foreach(ItemView kind in Inventory.itemViews)
        {
            if (probablility <= kind.ItemCounter.Probablilty)
            {
                Character.Inventory.AddItem(kind.ItemCounter);
                StaticManager.ShowAlert(kind.ItemCounter.Data.Name + "을 획득했습니다", Color.green);
            }
        }

    }

    void DoAnimator(State state)
    {
        ResetAnimatorState();
        switch (state)
        {
            case State.idle: Animator.SetBool("NomalIdle", true); break;
            case State.following: Animator.SetBool("Walking", true); break;
            case State.battle: Animator.SetBool("BattleIdle", true); break;
            case State.attack: Animator.SetBool("Attack", true); break;
            case State.getHit: Animator.SetBool("GetHit", true); break;
            case State.dead: Animator.SetBool("Dead", true); break;
        }
    }
    void ResetAnimatorState()
    {
        Animator.SetBool("NomalIdle", false);
        Animator.SetBool("Walking", false);
        Animator.SetBool("BattleIdle", false);
        Animator.SetBool("Attack", false);
        Animator.SetBool("GetHit", false);
        Animator.SetBool("Dead", false);
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
            return Vector3.Distance(Character.transform.position, transform.position) <= 2f;
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
