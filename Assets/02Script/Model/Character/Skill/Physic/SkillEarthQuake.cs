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
        for (int i = 0; i < 5; i++)
        {
            var copy = GetHitBox();

            copy.transform.position = Model.position;

            yield return copy.CheckObjCollideInTime();

            if (copy.isWorks)
            {
                print(true);
                var target = copy.GetTarget(Model.position);
                SetDamage(target);
            }

            copy.Collider.enabled = false;
            copy.gameObject.SetActive(false);
            hitBoxes.Enqueue(copy);
        }
        yield return null;
    }
}
