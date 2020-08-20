using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLowSmash : SkillData, ISkillActivator
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

        copy.transform.position = Model.position + Model.forward * Length;

        yield return copy.CheckObjCollideInTime();

        if (copy.isWorks)
        {
            print(true);
            var target = copy.GetTarget(Model.position);
            SetDamage(target);
            copy.Collider.enabled = false;
        }

        yield return StartCoroutine(copy.WaitForEffectTime());

        copy.gameObject.SetActive(false);
        hitBoxes.Enqueue(copy);

        yield return null;
    }
}
