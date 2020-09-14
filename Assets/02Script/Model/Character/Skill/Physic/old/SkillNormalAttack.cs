/*using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class SkillNormalAttack : SkillData, ISkillActivator
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

        copy.transform.position = Model.transform.position + Model.transform.forward * Length;

        yield return copy.CheckObjCollideInTime();

        if (copy.isWorks)
        {
            print(true);
            var target = copy.GetTarget(Model.transform.position);
            SetDamage(target);
        }

        copy.Collider.enabled = false;
        copy.gameObject.SetActive(false);
        hitBoxes.Enqueue(copy);

        yield return null;
    }
}
*/