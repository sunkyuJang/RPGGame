using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
using System.Runtime.CompilerServices;

public class SkillMovementSeedBoomForBossMonster : SkillMovement, ISkillMovement
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

        yield return StartCoroutine(model.WaitTillInterrupt(1));

        Vector2[] dir = GMath.MoveToRad(model.transform.eulerAngles.y, 1f, 1f);
        for (int i = 0; i < 3; i++)
        {
            var startDir = i == 0 ? GMath.ConvertV2ToV3xz(dir[0])
                : i == 1 ? model.transform.forward : GMath.ConvertV2ToV3xz(dir[1]);
            var hitBox = skillData.GetHitBox();// Instantiate(SeedBoomHitBoxObj).GetComponent<HitBoxCollider>();
            hitBox.transform.position = model.transform.position;
            StartCoroutine(EachBallMovement(hitBox, (i + 1) * 0.1f + 0.3f, startDir));
        }

        yield return null;
    }

    public IEnumerator EachBallMovement(HitBox hitBox, float force, Vector3 startDir)
    {
        hitBox.transform.forward = startDir;
        float upperDegree = 75f;
        var ratio = GMath.DegreeToRatio(upperDegree);
        Vector3 firstShotDirction = (hitBox.transform.forward * (1 - ratio) + Vector3.up * ratio) * force;
        Vector3 downAcceleration = (Physics.gravity * Time.fixedDeltaTime);

        hitBox.StartCountDown();
        bool canMaintainState = true;
        while (canMaintainState || !hitBox.isCollide)
        {
            hitBox.transform.position = hitBox.transform.position + firstShotDirction + downAcceleration;
            downAcceleration += downAcceleration * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();

            if (firstShotDirction.magnitude < downAcceleration.magnitude)
                if (hitBox.transform.position.y <= 2f)
                    break;
        }

        canMaintainState = true;
        float degree = 0f;
        while (canMaintainState || !hitBox.isCollide)
        {
            yield return new WaitForFixedUpdate();
            hitBox.transform.RotateAround(model.transform.position, Vector3.up, degree);
            degree = 50f * Time.fixedDeltaTime;

            if (!hitBox.isTimeLeft)
                break;
        }

        if (hitBox.isCollide)
            skillData.SetDamage(hitBox.GetTarget(hitBox.transform.position));

        skillData.returnHitBox(hitBox);
        yield break;
    }
}
