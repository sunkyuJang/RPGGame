﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFireBall : SkillData
{
    new public void Awake()
    {
        base.Awake();
    }
    protected override IEnumerator StartHitBoxMove()
    {
        var copy = GetHitBox();

        copy.transform.position = Model.position + Model.forward;
        copy.Rigidbody.velocity = Model.forward * 10f;

        yield return StartCoroutine(copy.CheckObjCollideInDist(transform.position, Length));

        if (copy.isWorks)
        {
            print(true);
            var target = copy.GetTarget(Model.position);
            SetDamage(target);
        }

        copy.Collider.enabled = false;
        copy.gameObject.SetActive(false);
        hitBoxes.Enqueue(copy);

        yield return null;
    }
}
