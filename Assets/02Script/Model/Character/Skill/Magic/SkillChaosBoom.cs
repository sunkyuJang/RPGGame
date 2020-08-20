using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillChaosBoom : SkillData, ISkillActivator
{
    public float downSpeed = 1f;
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
        var copy = GetHitBox();

        copy.transform.position = targetModel.transform.position + Vector3.up * 5f;
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
                    var target = copy.GetTarget(Model.position);
                    SetDamage(target);
                }
                print(true);
            }

            colliderTurnOn = !colliderTurnOn;
            copy.Collider.enabled = colliderTurnOn;
        }

        copy.gameObject.SetActive(false);
        hitBoxes.Enqueue(copy);

        yield return null;
    }
}
