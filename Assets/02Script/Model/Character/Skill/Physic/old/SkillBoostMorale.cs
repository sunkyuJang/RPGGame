/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBoostMorale : SkillData, ISkillActivator
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

        copy.transform.position = Model.transform.position;
        copy.isImmediately = true;

        //yield return copy.CheckObjCollideInTime();

        copy.StartEffectTimeCountDown();
        while (copy.isEffectTimeLeft)
        {
            yield return new WaitForFixedUpdate();
            if (copy.isWorks)
            {
                print(true);
                var target = copy.GetTarget(Model.transform.position);
                SetDamage(target);
            }
        }

        copy.Collider.enabled = false;
        copy.gameObject.SetActive(false);
        hitBoxes.Enqueue(copy);

        yield return null;
    }
}
*/