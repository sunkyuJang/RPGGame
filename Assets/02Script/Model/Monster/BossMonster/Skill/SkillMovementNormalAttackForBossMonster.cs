using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMovementNormalAttackForBossMonster : SkillMovement
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


        for (int i = 1, max = 3; i <= max; i++)
        {
            yield return StartCoroutine(model.WaitTillInterrupt(1));
            var copy = skillData.GetHitBox();
            copy.transform.position = skillData.Model.transform.position + Vector3.up + skillData.Model.transform.forward * skillData.Length;

            StartCoroutine(ProcessEachHitBox(copy));

            // copy.isImmediately = true;
            // copy.Collider.enabled = true;

            // yield return copy.CheckObjCollideInTime();
            // if (copy.isCollide)
            // {
            //     var target = copy.GetTarget(skillData.Model.transform.position);
            //     skillData.SetDamage(target);
            //     copy.Collider.enabled = false;
            // }
        }

        // skillData.returnHitBox(copy);

        yield return null;
    }

    IEnumerator ProcessEachHitBox(HitBox copy)
    {
        copy.isImmediately = true;
        yield return StartCoroutine(copy.CheckObjCollideInTime());
        if (copy.isCollide)
        {
            var target = copy.GetTarget(skillData.Model.transform.position);
            skillData.SetDamage(target);
        }

        yield return new WaitWhile(() => copy.isEffectTimeLeft);
        skillData.returnHitBox(copy);
    }
}
