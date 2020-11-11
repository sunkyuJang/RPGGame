using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillMovementFireBall : SkillMovement, ISkillMovement
{
    public float Speed = 3f;
    new void Start()
    {
        base.Start();
        skillData.skillMovement = (ISkillMovement)this;
    }
    public void StartMove() => StartCoroutine(StartHitBoxMovement());
    new public IEnumerator StartHitBoxMovement()
    {
        var copy = skillData.GetHitBox();

        copy.transform.position = skillData.Model.transform.position + Vector3.up;
        copy.transform.forward = skillData.Model.transform.forward;
        copy.Rigidbody.velocity = copy.transform.forward * Speed;
        copy.isImmediately = true;

        yield return StartCoroutine(copy.CheckObjCollideInTime());

        if (copy.isCollide)
        {
            var target = copy.GetTarget(skillData.Model.transform.position);
            skillData.SetDamage(target);
        }
        // while (copy.isEffectTimeLeft)
        // {
        //     yield return new WaitForFixedUpdate();
        //     if (copy.isCollide)
        //     {
        //         var target = copy.GetTarget(skillData.Model.transform.position);
        //         skillData.SetDamage(target);
        //     }
        // }

        skillData.returnHitBox(copy);

        yield return null;
    }
}
