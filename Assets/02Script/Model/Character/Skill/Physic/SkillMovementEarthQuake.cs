using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMovementEarthQuake : SkillMovement, ISkillMovement
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

        var startPosition = skillData.Model.transform.position;
        var startDirection = skillData.Model.transform.forward;

        for (int i = 0, max = 5; i < max; i++)
        {
            var copy = skillData.GetHitBox();
            StartCoroutine(copy.StartCountingEffectTime());
            copy.transform.position = startPosition + startDirection * i * skillData.Length / max;
            StartCoroutine(EachBoxMove(copy));
            yield return new WaitForSeconds(0.3f);
        }
        yield return null;
    }
    IEnumerator EachBoxMove(HitBox copy)
    {
        // copy.transform.position = skillData.Model.transform.position + skillData.Model.transform.forward * count * dist;
        copy.isImmediately = true;

        copy.StartCountDown();
        while (copy.isTimeLeft)
        {
            if (copy.isCollide)
            {
                var target = copy.GetTarget(skillData.Model.transform.position);
                skillData.SetDamage(target);
                break;
            }
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitWhile(() => copy.isEffectTimeLeft);
        skillData.returnHitBox(copy);

        yield return null;
    }
}
