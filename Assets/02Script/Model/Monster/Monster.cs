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
            if (HPViewer.MinDist <= HPBarPositionToScreen.z && HPBarPositionToScreen.z < HPViewer.MaxDist)
            {
                return true;
            }
            return false;
        }
    }

    protected Rect RoamingArea { set; get; }
    protected enum ActionState { roaming, following, battle, attack, getHit, dead, idle, skill }
    protected bool isAttacking { set; get; }
    protected ActionState NowState { set; get; }
    protected ActionState BeforeState { set; get; }

    public const float sightRadius = 5f;
    public const float SigthLimitRad = 30f * Mathf.Deg2Rad;
    float GetNowAngle { get { return GMath.Get360DegToRad(transform.eulerAngles.y); } }

    bool canAttack = true;
    public Transform FXStartPoint { private set; get; }

    new protected void Awake()
    {
        base.Awake();
        NowState = ActionState.roaming;
        BeforeState = ActionState.idle;
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
        if (CanShowingHPBar != HPViewer.gameObject.activeSelf)
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
                case ActionState.idle: StartCoroutine(DoIdle()); break;
                case ActionState.battle: StartCoroutine(DoBattle()); break;
                case ActionState.roaming: StartCoroutine(DoRoaming()); break;
                case ActionState.following: StartCoroutine(DoFollowing()); break;
                case ActionState.attack: StartCoroutine(DoAttack()); break;
                    //case State.getHit: StartCoroutine(DoGetHit()); break;
            }
        }
        Debug.DrawRay(Camera.main.transform.position, Camera.main.WorldToScreenPoint(HPBarPositionGuide.position));
    }

    protected void SelectedNextAction()
    {
        if (NowState != BeforeState)
        {
            BeforeState = NowState;
            switch (NowState)
            {
                case ActionState.idle: StartCoroutine(DoIdle()); break;
                case ActionState.battle: StartCoroutine(DoBattle()); break;
                case ActionState.roaming: StartCoroutine(DoRoaming()); break;
                case ActionState.following: StartCoroutine(DoFollowing()); break;
                case ActionState.attack: StartCoroutine(DoAttack()); break;
                    //case State.getHit: StartCoroutine(DoGetHit()); break;
            }
        }
    }

    protected IEnumerator DoIdle()
    {
        DoAnimator(ActionState.idle);
        float pauseTime = 0f;
        float pauseTimeLimit = 3f;
        while (BeforeState == ActionState.idle)
        {
            if (pauseTime < pauseTimeLimit)
            {
                pauseTime += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();

                if (IsDetectedCharacter)
                {
                    NowState = ActionState.following;
                    yield break;
                }
            }
            else
            {
                NowState = ActionState.roaming;
                yield break;
            }
        }
    }

    protected IEnumerator DoRoaming()
    {
        DoAnimator(ActionState.following);
        float roamingDistance = 0f;
        float roamingDistanceLimit = 5f;
        transform.eulerAngles = new Vector3(0f, Random.Range(-180f, 180f), 0f);
        Rigidbody.velocity = transform.forward * SPD;

        while (BeforeState == ActionState.roaming)
        {
            if (roamingDistance < roamingDistanceLimit)
            {
                roamingDistance += SPD * Time.fixedDeltaTime;
            }
            else
            {
                Rigidbody.velocity = Vector3.zero;
                NowState = ActionState.idle;
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
                NowState = ActionState.following;
                yield break;
            }
        }
    }

    protected IEnumerator DoBattle()
    {
        DoAnimator(ActionState.battle);
        while (BeforeState == ActionState.battle)
        {
            transform.LookAt(Character.transform.position);
            yield return new WaitForFixedUpdate();

            if (!IsCloseEnoughWithChracter)
            {
                NowState = ActionState.following;
                break;
            }

            if(canAttack)
                NowState = ActionState.attack;
        }
    }

    protected IEnumerator DoFollowing()
    {
        DoAnimator(ActionState.following);
        while (BeforeState == ActionState.following)
        {
            transform.LookAt(Character.transform.position);
            Rigidbody.velocity = transform.forward * SPD;
            yield return new WaitForFixedUpdate();

            if (IsOutRoamingArea)
            {
                NowState = ActionState.roaming;
                break;
            }

            if (IsCloseEnoughWithChracter)
            {
                DoAnimator(ActionState.battle);
                Rigidbody.velocity = Vector3.zero;
                NowState = ActionState.attack;
                break;
            }
            Debug.DrawRay(transform.position, Character.transform.position - transform.position);
        }
    }

    protected IEnumerator DoAttack()
    {
        canAttack = false;
        DoAnimator(ActionState.attack);
        while (!NowAnimatorInfo.IsName("NomalAttack"))
            yield return new WaitForFixedUpdate();

        Character.GetHit(ATK);
        while (BeforeState == ActionState.attack)
        {
            yield return new WaitForFixedUpdate();
            transform.LookAt(Character.transform.position);

            if (NowAnimatorInfo.normalizedTime >= 0.9f)
            {
                NowState = ActionState.battle;
                StartCoroutine(StartAttackDelayTimer(2f));
                break;
            }
        }
    }

    IEnumerator StartAttackDelayTimer(float limit)
    {
        float attackDelayTimer = 0f;

        while (attackDelayTimer <= limit)
        {
            attackDelayTimer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
          canAttack = true;
    }

    new public void GetHit(float damege, GameObject HitFX, bool isFXStartFromGround)
    {
        damege -= DEF;
        damege = (int)(damege > 0 ? damege : 0);
        //StaticManager.ShowAlert(((int)damege).ToString(), Color.red, Camera.main.WorldToScreenPoint(transform.position + (Vector3.up * 2)));
        //NowState = NowState == ActionState.attack ? NowState : ActionState.getHit;
        if (!IsActionStateAre(ActionState.attack))
        {
            NowState = ActionState.getHit;
            StartCoroutine(DoGetHit());
        }
        
        if(HitFX != null)
            StartCoroutine(ControllHitFX(HitFX, isFXStartFromGround));
    }

    protected IEnumerator ControllHitFX(GameObject HitFX, bool isFXStartFromGround)
    {
        Transform FXtransform = Instantiate(HitFX).transform;
        FXtransform.position = isFXStartFromGround ? transform.position : FXStartPoint.position;
        FXtransform.forward = transform.forward;

        ParticleSystem particle = FXtransform.GetComponent<ParticleSystem>();
        while (particle.IsAlive())
            yield return new WaitForFixedUpdate();

        Destroy(FXtransform.gameObject);
    }

    protected IEnumerator DoGetHit()
    {
        DoAnimator(ActionState.getHit);

        while (!NowAnimatorInfo.IsName("GetHit"))
            yield return new WaitForEndOfFrame();

        if(nowHP <= 0)
        {
            DoAnimator(ActionState.dead);
            Rigidbody.velocity = Vector3.zero;
            
            while (!NowAnimatorInfo.IsName("Dead"))
                yield return new WaitForEndOfFrame();

            float waitTIme = NowAnimatorInfo.length - (NowAnimatorInfo.normalizedTime * NowAnimatorInfo.length);
            yield return new WaitForSeconds(waitTIme);
            Destroy(HPViewer.gameObject);
            Destroy(gameObject);
            DropItem();
        }
        else
        {
            DoAnimator(ActionState.battle);
            while (NowAnimatorInfo.normalizedTime <= 0.9f)
                yield return new WaitForFixedUpdate();
        }

        NowState = ActionState.battle;
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
                Character.ShowAlert(kind.ItemCounter.Data.Name + "을 획득했습니다", Color.green);
            }
        }
    }

    bool IsActionStateAre(ActionState actionState) { return BeforeState == actionState; }

    void DoAnimator(ActionState state)
    {
        ResetAnimatorState();
        switch (state)
        {
            case ActionState.idle: Animator.SetBool("NomalIdle", true); break;
            case ActionState.following: Animator.SetBool("Walking", true); break;
            case ActionState.battle: Animator.SetBool("BattleIdle", true); break;
            case ActionState.attack: Animator.SetBool("Attack", true); break;
            case ActionState.getHit: Animator.SetBool("GetHit", true); break;
            case ActionState.dead: Animator.SetBool("Dead", true); break;
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
