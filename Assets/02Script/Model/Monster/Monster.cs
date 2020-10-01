using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
using UnityEditor;
using UnityEngine.UIElements;
using JetBrains.Annotations;
using System.Runtime.Serialization;
using UnityEngine.XR.WSA.Input;

public class Monster : Model
{
    public MonseterLocator MonseterLocator { set; get; }
    public float roamingHorizontal;
    public float roamingVertical;
    protected Character Character { set; get; }
    public Rect RoamingArea { set; get; }
    protected enum ActionState { roaming, following, battle, attack, getHit, dead, idle, skill, TimeLine, non }
    protected bool isAttacking { set; get; }
    protected ActionState NowState { set; get; }
    public void SetIdleState() { NowState = ActionState.idle; }
    protected ActionState BeforeState { set; get; }

    protected float sightRadius = 5f;
    protected float SigthLimitRad = 30f * Mathf.Deg2Rad;
    protected float closeEnough = 2f;
    protected float GetNowAngle { get { return GMath.Get360DegToRad(transform.eulerAngles.y); } }
    protected bool canAttack = true;
    public GameObject HitFXStartPoint;
    public Transform FXStartPoint { private set; get; }
/*    public Transform skillPullingGroup;
    public Transform skillListGroup;
    protected List<SkillData> skillsMovements { set; get; } = new List<SkillData>();*/

    public bool isCreating { private set; get; } = true;
    new protected void Awake()
    {
        base.Awake();
        NowState = ActionState.roaming;
        BeforeState = ActionState.idle;
        FXStartPoint = HitFXStartPoint.transform;
 //       skillPullingGroup = new GameObject(gameObject.name).GetComponent<Transform>();
    }
    // Start is called before the first frame update
    new protected void Start()
    {
        base.Start();
/*        skillPullingGroup.parent = StaticManager.MonsterSkillPulling;
        foreach (Transform skillTransform in skillListGroup)
        {
            var nowSkillData = skillTransform.GetComponent<SkillData>();
            nowSkillData.Model = this;
            nowSkillData.skillPooling.parent = skillPullingGroup;
            skillsMovements.Add(nowSkillData);
        }*/

        var position = GMath.ConvertV3xzToV2(transform.position);
        var positionX = position.x - roamingVertical / 2;
        var positionY = position.y - roamingHorizontal / 2;
        RoamingArea = new Rect(new Vector2(positionX, positionY), new Vector2(roamingVertical, roamingHorizontal));
    }

    protected void OnEnable()
    {
        NowState = ActionState.idle;
    }

    new protected void OnDisable()
    {
        BeforeState = ActionState.non;
        base.OnDisable();
    }

    protected SkillData GetSkillData(string name)
    {
        foreach (SkillData skillData in skillsMovements)
            if (skillData.gameObject.name.Equals(name))
                return skillData;

        return null;
    }
    // Update is called once per frame
    protected void Update()
    {
        if (IsRunningTimeLine)
            NowState = ActionState.TimeLine;
        SelectedNextAction();
    }

    new protected void FixedUpdate()
    {
        base.FixedUpdate();
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
                case ActionState.getHit: StartCoroutine(DoGetHit()); break;
                case ActionState.dead: StartCoroutine(DoDead()); break;
                case ActionState.TimeLine: StartCoroutine(DoIdle()); break;
            }
        }
    }

    protected virtual IEnumerator DoIdle()
    {
        DoAnimator(ActionState.idle);
        if (IsRunningTimeLine)
        {
            while(IsRunningTimeLine)
                yield return new WaitForFixedUpdate();

            NowState = ActionState.idle;
        }
        else
        {
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
    }

    protected virtual IEnumerator DoRoaming()
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

    protected virtual IEnumerator DoBattle()
    {
        DoAnimator(ActionState.battle);
        while (BeforeState == ActionState.battle)
        {
            transform.LookAt(Character.transform.position);
            yield return new WaitForFixedUpdate();

            if (!IsCloseEnoughWithChracter)
                NowState = ActionState.following;

            if(canAttack)
                NowState = ActionState.attack;
        }
    }

    protected virtual IEnumerator DoFollowing()
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
                Rigidbody.velocity = Vector3.zero;
                NowState = ActionState.battle;
                break;
            }
        }
    }

    protected virtual IEnumerator DoAttack() { yield break; }

    protected virtual IEnumerator StartAttackDelayTimer(float limit)
    {
        float attackDelayTimer = 0f;

        while (attackDelayTimer <= limit)
        {
            attackDelayTimer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        canAttack = true;
    }

    public void GetHit(GameObject HitFX, bool isFXStartFromGround)
    {
        if (nowHP <= 0f && !IsAlreadyDead)
            NowState = ActionState.dead;
        else if (!IsActionStateAre(ActionState.attack) && canGetHit)
            NowState = ActionState.getHit;
        
        if(HitFX != null)
            StartCoroutine(ControllHitFX(HitFX, isFXStartFromGround));
    }

    protected virtual IEnumerator ControllHitFX(GameObject HitFX, bool isFXStartFromGround)
    {
        Transform FXtransform = Instantiate(HitFX).transform;
        FXtransform.position = isFXStartFromGround ? transform.position : FXStartPoint.position;
        FXtransform.forward = transform.forward;

        ParticleSystem particle = FXtransform.GetComponent<ParticleSystem>();
        while (particle.IsAlive())
            yield return new WaitForFixedUpdate();

        Destroy(FXtransform.gameObject);
    }

    protected bool canGetHit { set; get; } = true;
    protected virtual IEnumerator DoGetHit()
    {
        canGetHit = false;
        DoAnimator(ActionState.getHit);

        while (!NowAnimatorInfo.IsName("GetHit"))
            yield return new WaitForEndOfFrame();
        
        NowState = ActionState.battle;
        canGetHit = true;
        yield break;
    }

    bool IsAlreadyDead { set; get; }
    protected virtual IEnumerator DoDead()
    {
        IsAlreadyDead = true;
        DoAnimator(ActionState.getHit);

        while (!NowAnimatorInfo.IsName("GetHit"))
            yield return new WaitForEndOfFrame();
        
        DoAnimator(ActionState.dead);
        Rigidbody.velocity = Vector3.zero;

        while (!NowAnimatorInfo.IsName("Dead"))
            yield return new WaitForEndOfFrame();

        float waitTIme = NowAnimatorInfo.length - (NowAnimatorInfo.normalizedTime * NowAnimatorInfo.length);
        yield return new WaitForSeconds(waitTIme);
        Destroy(gameObject);
        DropItem();
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

    protected void DoAnimator(ActionState state)
    {
        ResetAnimatorState();
        switch (state)
        {
            case ActionState.idle: Animator.SetBool("NormalIdle", true); break;
            case ActionState.following: Animator.SetBool("Walking", true); break;
            case ActionState.battle: Animator.SetBool("BattleIdle", true); break;
            case ActionState.attack: Animator.SetBool("Attack", true); break;
            case ActionState.getHit: Animator.SetBool("GetHit", true); break;
            case ActionState.dead: Animator.SetBool("Dead", true); break;
        }
    }

    void ResetAnimatorState()
    {
        Animator.SetBool("NormalIdle", false);
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
    protected virtual bool IsCloseEnoughWithChracter { get {
            return Vector3.Distance(Character.transform.position, transform.position) <= closeEnough;
        } }
    protected void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,1,1, 0.5f);
        Gizmos.DrawCube(transform.position + Vector3.up * 0.5f, new Vector3(roamingVertical, 1, roamingHorizontal));
        Gizmos.color = new Color(1,0,0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, sightRadius);

        Vector2[] vectors = GMath.MoveToRad(GetNowAngle, SigthLimitRad, sightRadius);
        Debug.DrawRay(transform.position, GMath.ConvertV2ToV3xz(vectors[0]), Color.red);
        Debug.DrawRay(transform.position, GMath.ConvertV2ToV3xz(vectors[1]), Color.red);
    }
}
