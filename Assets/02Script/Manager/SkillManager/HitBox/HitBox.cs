using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

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

    public float duringTime;
    float nowDuringTime;
    public bool isTimeOver { get { return !(nowDuringTime > 0f); } }

    public void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(targetModel == TargetModel.All)
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
        for(int i = 0; i < colliderList.Count; i++)
        {
            var nowCollider = colliderList[i];
            var nowDist = Vector3.Distance(centerPosition, nowCollider.transform.position);
            if(nowDist < dist)
            {
                dist = nowDist;
                closeOne = nowCollider;
            }
        }
        return closeOne;
    }

    public List<Collider> GetTarget(Vector3 centerPosition)
    {
        List<Collider> target;
        if (isSingleTarget)
        {
            target = new List<Collider>();
            target.Add(GetSingleTargetColiders(centerPosition));
        }
        else
            target = GetMultiTargetColiders();
        return target;
    }

    public IEnumerator CheckObjCollideInTime()
    {
        nowDuringTime = duringTime;
        while (!isTimeOver)
        {
            yield return new WaitForFixedUpdate();
            nowDuringTime -= Time.fixedDeltaTime;
            if (isCollide) 
            {
                print(targetModel);
                break; 
            }
        }
    }

    public bool isWorks { get { return isCollide && !isTimeOver; } }
}
