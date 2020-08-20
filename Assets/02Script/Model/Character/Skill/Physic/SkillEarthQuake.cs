using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEarthQuake : SkillData
{
    new public void Awake()
    {
        base.Awake();
    }
    protected override IEnumerator StartHitBoxMove()
    {
        for (int i = 0, max = 5; i < max; i++)
        {
            var copy = GetHitBox();

            StartCoroutine(EachBoxMove(copy, i, Length / max));
            yield return new WaitForSeconds(0.2f);
        }
        yield return null;
    }

    IEnumerator EachBoxMove(HitBox copy, int count, float dist)
    {
        copy.transform.position = Model.position + Model.forward * count * dist;
        copy.isImmediately = true;

        while (copy.isTimeOver)
        {
            yield return copy.CheckObjCollideInTime();

            if (copy.isWorks)
            {
                print(true);
                var target = copy.GetTarget(Model.position);
                SetDamage(target);
            }
        }

        copy.Collider.enabled = false;
        copy.gameObject.SetActive(false);
        hitBoxes.Enqueue(copy);

        yield return null;
    }
}
