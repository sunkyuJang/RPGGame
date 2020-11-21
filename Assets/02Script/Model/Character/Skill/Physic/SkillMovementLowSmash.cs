using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMovementLowSmash : SkillMovement
{
    // Start is called before the first frame update
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

        copy.transform.position = skillData.Model.transform.position + Vector3.up + skillData.Model.transform.forward * skillData.Length;

        yield return StartCoroutine(copy.CheckObjCollideInTime());

        if (copy.isCollide)
        {
            var target = copy.GetSingleTargetColiders(skillData.Model.transform.position);
            copy.transform.position = target.transform.position;
            copy.isImmediately = true;
            skillData.SetDamage(target);
        }

        yield return new WaitWhile(() => copy.isEffectTimeLeft);
        skillData.returnHitBox(copy);

        yield return null;
    }
}
