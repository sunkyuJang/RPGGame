using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosBoomSkillMovement : SkillMovement, ISkillActivator
{
    public float downSpeed = 1f;
    public void SetActivateSkill()
    {
        StartMovement();
    }

    new public void Start()
    {
        base.Start();
    }

    protected override IEnumerator StartHitBoxMove()
    {
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
                    var target = copy.GetTarget(skillData.Model.position);
                    skillData.SetDamage(target);
                }
            }

            colliderTurnOn = !colliderTurnOn;
            copy.Collider.enabled = colliderTurnOn;
        }

        copy.gameObject.SetActive(false);
        skillData.hitBoxes.Enqueue(copy);

        yield return null;
    }
}
