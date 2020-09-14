using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMovementNormalAttackForBossMonster : SkillMovement, ISkillMovement
{
    new void Start()
    {
        base.Start();
        skillData.skillMovement = (ISkillMovement)this;
    }
    public void StartMove() => StartCoroutine(StartHitBoxMovement());
    public IEnumerator StartHitBoxMovement()
    {
        yield return base.StartHitBoxMovement();

        var copy = skillData.GetHitBox();

        copy.transform.position = skillData.Model.transform.position + Vector3.up + skillData.Model.transform.forward * skillData.Length;
        print(true);
        for(int i = 1, max = 3; i <= max; i++)
        {
            yield return StartCoroutine(model.WaitTillInterrupt(1));
            copy.Collider.enabled = true;
            
            yield return copy.CheckObjCollideInTime();
            if (copy.isCollide)
            {
                var target = copy.GetTarget(skillData.Model.transform.position);
                skillData.SetDamage(target);
                copy.Collider.enabled = false;
            }
        }

        skillData.returnHitBox(copy);

        yield return null;
    }
}
