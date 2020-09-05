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
    public IEnumerator StartHitBoxMovement()
    {
        for (int i = 0, max = 5; i < max; i++)
        {
            var copy = skillData.GetHitBox();

            StartCoroutine(EachBoxMove(copy, i, skillData.Length / max));
            yield return new WaitForSeconds(0.3f);
        }
        yield return null;
    }
    IEnumerator EachBoxMove(HitBox copy, int count, float dist)
    {
        copy.transform.position = skillData.Model.transform.position + skillData.Model.transform.forward * count * dist;
        copy.isImmediately = true;

        copy.StartCountDown();
        while (copy.isTimeLeft)
        {
            if (copy.isWorks)
            {
                var target = copy.GetTarget(skillData.Model.transform.position);
                skillData.SetDamage(target);
            }
            yield return new WaitForFixedUpdate();
        }

        copy.Collider.enabled = false;
        copy.gameObject.SetActive(false);
        skillData.hitBoxes.Enqueue(copy);

        yield return null;
    }
}
