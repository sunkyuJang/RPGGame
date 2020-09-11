using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMovementOverDriveForBossMonster : SkillMovement, ISkillMovement
{
    new void Start()
    {
        base.Start();
        skillData.skillMovement = (ISkillMovement)this;
    }
    public void StartMove() => StartCoroutine(StartHitBoxMovement());
    public IEnumerator StartHitBoxMovement()
    {
        yield return StartCoroutine(model.WaitTillInterrupt(1));

        var copy = skillData.GetHitBox();

        copy.transform.position = skillData.Model.transform.position + Vector3.up + skillData.Model.transform.forward;
        copy.Rigidbody.velocity = copy.transform.forward * 10f;
        var StartPosition = copy.transform.position;
        var nowLength = 0f;
        while(nowLength < skillData.Length)
        {
            yield return new WaitForFixedUpdate();
            nowLength = Vector3.Distance(StartPosition, copy.transform.position);
            if (copy.isCollide) 
                skillData.SetDamage(copy.GetTarget(model.transform.position));
        }

        copy.Collider.enabled = false;
        copy.gameObject.SetActive(false);
        skillData.hitBoxes.Enqueue(copy);

        yield return null;
    }
}
