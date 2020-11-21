using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMovementNormalAttack : SkillMovement
{
    new void Start()
    {
        base.Start();
        skillData.skillMovement = this;//(ISkillMovement)this;
    }
    public override void StartMove() => StartCoroutine(StartHitBoxMovement());
    new public IEnumerator StartHitBoxMovement()
    {
        yield return base.StartHitBoxMovement();
        var copy = skillData.GetHitBox();

        copy.transform.position = skillData.Model.transform.position + Vector3.up + skillData.Model.transform.forward * skillData.Length;

        yield return StartCoroutine(copy.CheckObjCollideInTime());

        if (copy.isCollide)
        {
            copy.isImmediately = true;
            var target = copy.GetTarget(skillData.Model.transform.position);
            skillData.SetDamage(target);
        }

        yield return new WaitWhile(() => copy.isEffectTimeLeft);
        skillData.returnHitBox(copy);

        yield return null;
    }
}
