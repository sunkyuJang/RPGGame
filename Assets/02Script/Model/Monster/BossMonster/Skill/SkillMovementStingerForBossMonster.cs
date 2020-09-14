using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

public class SkillMovementStingerForBossMonster : SkillMovement, ISkillMovement
{
    public float startSpeed = 15f;
    new void Start()
    {
        base.Start();
        skillData.skillMovement = (ISkillMovement)this;
    }
    public void StartMove() => StartCoroutine(StartHitBoxMovement());
    public IEnumerator StartHitBoxMovement()
    {
        yield return base.StartHitBoxMovement();
        yield return StartCoroutine(model.WaitTillInterrupt(1));

        var copy = skillData.GetHitBox();
        model.Rigidbody.velocity = model.transform.forward * startSpeed;
        var startPoint = model.transform.position;
        var nowLength = 0f;
        while (startSpeed > 0f)
        {
            yield return new WaitForFixedUpdate();
            copy.transform.position = model.transform.position + model.transform.forward + Vector3.up;
            nowLength = Vector3.Distance(startPoint, model.transform.position);
            if (nowLength > skillData.Length * 0.5f)
                model.Rigidbody.velocity = model.transform.forward * startSpeed--;

            if (copy.isCollide)
                skillData.SetDamage(copy.GetTarget(copy.transform.position));
        }

        startSpeed = 15f;
        skillData.returnHitBox(copy);
        yield return null;
    }
}
