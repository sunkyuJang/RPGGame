using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class HitBox : MonoBehaviour
{
    public GameObject HitBoxEffectObj;
    public Rigidbody Rigidbody { private set; get; }
    public Collider Collider { private set; get; }
    List<Collider> enteredColliders { set; get; } = new List<Collider>();
    public bool isCollide { get { return GetMultiTargetColiders().Count > 0; } }
    public bool isStop { get { return Rigidbody.velocity == Vector3.zero; } }
    public bool isImmediately { set; get; }
    public enum TargetModel { Monster, Character, All }
    public TargetModel targetModel;
    public string GetTargetModelTag
    { get { return targetModel == TargetModel.Monster ? "Monster" : "Character"; } }
    public bool isSingleTarget;

    public enum CheckedType { inTime, inDist }
    public CheckedType nowCheckedType;
    public float duringTime;
    public float nowDuringTime;
    public bool isTimeLeft { get { return nowDuringTime > 0f; } }

    public float effectTime;
    public float nowEffectTime;
    public bool isEffectTimeLeft { get { return nowEffectTime > 0f; } }
    //public bool isDistOver { get { return nowDist > dist; } }

    public void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (targetModel == TargetModel.All)
            enteredColliders.Add(other);
        else
        {
            if (other.CompareTag(GetTargetModelTag))
            {
                enteredColliders.Add(other);
            }
        }
    }

    public List<Collider> GetMultiTargetColiders()
    {
        return enteredColliders;
    }
    public Collider GetSingleTargetColiders(Vector3 centerPosition)
    {
        var colliderList = GetMultiTargetColiders();
        var dist = 10000f;
        Collider closeOne = null;
        for (int i = 0; i < colliderList.Count; i++)
        {
            var nowCollider = colliderList[i];
            var nowDist = Vector3.Distance(centerPosition, nowCollider.transform.position);
            if (nowDist < dist)
            {
                dist = nowDist;
                closeOne = nowCollider;
            }
        }
        return closeOne;
    }

    public List<Collider> GetTarget(Vector3 centerPosition)
    {
        List<Collider> target = new List<Collider>();

        if (isSingleTarget)
            target.Add(GetSingleTargetColiders(centerPosition));
        else
            foreach (Collider collider in GetMultiTargetColiders())
                target.Add(collider);

        enteredColliders.Clear();
        return target;
    }

    public IEnumerator CheckObjCollideInTime()
    {
        StartCoroutine(StartCountingEffectTime());
        nowCheckedType = CheckedType.inTime;
        nowDuringTime = duringTime;
        while (isTimeLeft)
        {
            yield return new WaitForFixedUpdate();
            nowDuringTime -= Time.fixedDeltaTime;
            if (isCollide)
            {
                break;
            }
        }
        nowDuringTime = 0;
    }

    public IEnumerator StartCountingEffectTime()
    {
        nowEffectTime = effectTime;
        while (isEffectTimeLeft)
        {
            yield return new WaitForFixedUpdate();
            nowEffectTime -= Time.fixedDeltaTime;
        }
        nowEffectTime = 0;
    }

    // public IEnumerator CheckObjCollideInDist(Vector3 center, float dist)
    // {
    //     this.dist = dist;
    //     nowDist = Vector3.Distance(transform.position, center);
    //     while (nowDist < dist)
    //     {
    //         yield return new WaitForFixedUpdate();
    //         if (isCollide)
    //         {
    //             break;
    //         }
    //     }
    // }

    // public bool isWorks
    // {
    //     get
    //     {
    //         if (nowCheckedType == CheckedType.inTime)
    //             return isCollide && !isTimeLeft;
    //         else
    //             return isCollide && !isDistOver;
    //     }
    // }

    public void StartCountDown()
    {
        StartCoroutine(ReduceTime());
    }
    IEnumerator ReduceTime()
    {
        nowDuringTime = duringTime;
        while (isTimeLeft)
        {
            yield return new WaitForFixedUpdate();
            nowDuringTime -= Time.fixedDeltaTime;
        }
    }

    public void StartEffectTimeCountDown()
    {
        StartCoroutine(WaitForEffectTime());
    }
    public IEnumerator WaitForEffectTime()
    {
        nowEffectTime = effectTime;
        while (isEffectTimeLeft)
        {
            yield return new WaitForFixedUpdate();
            nowEffectTime -= Time.fixedDeltaTime;
        }
    }
}
