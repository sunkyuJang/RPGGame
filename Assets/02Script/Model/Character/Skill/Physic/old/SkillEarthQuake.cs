/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEarthQuake : SkillData, ISkillActivator
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
        for (int i = 0, max = 5; i < max; i++)
        {
            var copy = GetHitBox();

            StartCoroutine(EachBoxMove(copy, i, Length / max));
            yield return new WaitForSeconds(0.3f);
        }
        yield return null;
    }

    IEnumerator EachBoxMove(HitBox copy, int count, float dist)
    {
        copy.transform.position = Model.transform.position + Model.transform.forward * count * dist;
        copy.isImmediately = true;

        copy.StartCountDown();
        while (copy.isTimeLeft)
        {
            if (copy.isWorks)
            {
                var target = copy.GetTarget(Model.transform.position);
                SetDamage(target);
            }
            yield return new WaitForFixedUpdate();
        }

        copy.Collider.enabled = false;
        copy.gameObject.SetActive(false);
        hitBoxes.Enqueue(copy);

        yield return null;
    }
}
*/