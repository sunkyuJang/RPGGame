using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMovementNormalAttack : SkillMovement, ISkillMovement
{
    new void Start()
    {
        base.Start();
        skillData.skillMovement = (ISkillMovement)this;
    }
    public void StartMove() => StartCoroutine(StartHitBoxMovement());
    new public IEnumerator StartHitBoxMovement()
    {
        yield return base.StartHitBoxMovement();

        var copy = skillData.GetHitBox();

        copy.transform.position = skillData.Model.transform.position + Vector3.up + skillData.Model.transform.forward * skillData.Length;

        yield return copy.CheckObjCollideInTime();

        if (copy.isCollide)
        {
            var target = copy.GetTarget(skillData.Model.transform.position);
            skillData.SetDamage(target);
        }

        skillData.returnHitBox(copy);

        yield return null;
    }
}
