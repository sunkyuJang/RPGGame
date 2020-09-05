using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMovementBoostMorale : SkillMovement, ISkillMovement
{
    new void Start()
    {
        base.Start();
        skillData.skillMovement = (ISkillMovement)this;
    }
    public void StartMove() => StartCoroutine(StartHitBoxMovement());
    public IEnumerator StartHitBoxMovement()
    {
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

        copy.Collider.enabled = false;
        copy.gameObject.SetActive(false);
        skillData.hitBoxes.Enqueue(copy);

        yield return null;
    }
}
