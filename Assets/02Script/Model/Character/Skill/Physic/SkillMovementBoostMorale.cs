using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMovementBoostMorale : SkillMovement
{
    new void Start()
    {
        base.Start();
        // skillData.skillMovement = (ISkillMovement)this;
        skillData.skillMovement = this;
    }
    public override void StartMove() => StartCoroutine(StartHitBoxMovement());
    new public IEnumerator StartHitBoxMovement()
    {
        yield return base.StartHitBoxMovement();

        var copy = skillData.GetHitBox();

        copy.transform.position = skillData.Model.transform.position;
        copy.isImmediately = true;

        //yield return copy.CheckObjCollideInTime();

        copy.StartEffectTimeCountDown();
        while (copy.isEffectTimeLeft)
        {
            yield return new WaitForFixedUpdate();
            if (copy.isCollide)
            {
                var target = copy.GetTarget(skillData.Model.transform.position);
                skillData.SetDamage(target);
            }
        }

        skillData.returnHitBox(copy);

        yield return null;
    }
}
