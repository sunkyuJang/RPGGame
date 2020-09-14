using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMovementChaosBoom : SkillMovement, ISkillMovement
{
    public float downSpeed = 1f;
    new public void Start()
    {
        base.Start();
        skillData.skillMovement = (ISkillMovement)this;
    }

    public void StartMove() => StartCoroutine(StartHitBoxMovement());
    public IEnumerator StartHitBoxMovement()
    {
        yield return base.StartHitBoxMovement();
        var copy = skillData.GetHitBox();

        copy.transform.position = skillData.targetModel.transform.position + Vector3.up * 5f;
        copy.Rigidbody.velocity = Vector3.down * downSpeed;

        bool colliderTurnOn = true;
        copy.StartCountDown();
        while (copy.isTimeLeft)
        {
            yield return new WaitForSeconds(0.5f);

            if (colliderTurnOn)
            {
                if (copy.isCollide)
                {
                    var target = copy.GetTarget(skillData.Model.transform.position);
                    skillData.SetDamage(target);
                }
            }

            colliderTurnOn = !colliderTurnOn;
            copy.Collider.enabled = colliderTurnOn;
        }

        skillData.returnHitBox(copy);

        yield return null;
    }
}
