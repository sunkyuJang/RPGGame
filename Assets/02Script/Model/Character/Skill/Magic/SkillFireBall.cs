using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillFireBall : SkillData, ISkillActivator
{
    new public void Awake()
    {
        base.Awake();
    }
    public void SetActivateSkill()
    {
        ActivateSkill();
    }

    protected override IEnumerator StartHitBoxMove()
    {
        var copy = GetHitBox();

        print(copy.gameObject.name);

        copy.transform.position = Model.position + Model.forward;
        copy.Rigidbody.velocity = Model.forward * 1f;
        copy.isImmediately = true;

        yield return StartCoroutine(copy.CheckObjCollideInDist(Model.transform.position, Length));
        print(true);

        if (copy.isWorks)
        {
            var target = copy.GetTarget(Model.position);
            SetDamage(target);
        }

        copy.Collider.enabled = false;
        copy.gameObject.SetActive(false);
        hitBoxes.Enqueue(copy);

        yield return null;
    }
}
