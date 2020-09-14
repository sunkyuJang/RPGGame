using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;

public class SkillMovementHeavyFireBall : SkillMovement, ISkillMovement
{
    public float upperDegree = 75f;
    public float totalSpeed = 0.45f;
    new private void Start()
    {
        base.Start();
        skillData.skillMovement = (ISkillMovement)this;
    }
    public void StartMove() => StartCoroutine(StartHitBoxMovement());

    public IEnumerator StartHitBoxMovement()
    {
        yield return base.StartHitBoxMovement();
        var copy = skillData.GetHitBox();
        copy.isImmediately = true;
        var copyTranform = copy.transform;
        copyTranform.position = skillData.Model.transform.position;

        var ratio = GMath.DegreeToRatio(upperDegree);
        Vector3 firstShotDirction = (copyTranform.forward * (1 - ratio) + Vector3.up * ratio) * totalSpeed;
        Vector3 downAcceleration = (Physics.gravity * Time.fixedDeltaTime);

        while (copyTranform.position.y >= 0f)
        {
            copyTranform.position = copyTranform.position + firstShotDirction + downAcceleration;

            yield return new WaitForFixedUpdate();
            downAcceleration += downAcceleration * Time.fixedDeltaTime;

            if (copy.isCollide)
            {
                skillData.SetDamage(copy.GetTarget(skillData.Model.transform.position));
            }
        }

        skillData.returnHitBox(copy);
        yield return null;
    }

}
