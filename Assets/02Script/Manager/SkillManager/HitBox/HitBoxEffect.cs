using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxEffect : MonoBehaviour
{
    public HitBox hitBox;
    public GameObject HitBoxEffectObj;
    public enum EffectorStartTime { Immediately, Hit, EndMove }
    public EffectorStartTime effectorStartTime;
    bool isBeforeEffectDrawing { get { return !HitBoxEffectObj.activeSelf; } }

    private void Start()
    {
        HitBoxEffectObj.SetActive(false);
    }
    private void FixedUpdate()
    {
        if (isBeforeEffectDrawing)
        {
            HitBoxEffectObj.SetActive(ShouldDrawEffect());
        }
    }

    bool ShouldDrawEffect()
    {
        switch (effectorStartTime)
        {
            case EffectorStartTime.Hit:
                return hitBox.isCollide;
            case EffectorStartTime.EndMove:
                return hitBox.isStop;
            case EffectorStartTime.Immediately:
                return hitBox.isImmediately;
        }
        return false;
    }

}
