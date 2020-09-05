using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMovementCurse : SkillMovement, ISkillMovement
{
    public IEnumerator StartHitBoxMovement()
    {
        var copy = skillData.GetHitBox();

        copy.transform.position = skillData.targetModel.transform.position;

        yield return copy.CheckObjCollideInTime();

        if (copy.isCollide)
        {
            var target = copy.GetTarget(skillData.Model.transform.position);
            for (int i = 0; i < target.Count; i++)
            {
                StartCoroutine(MoveEachHitBox(skillData.GetHitBoxWithOutSetUp(), target[i]));
            }
        }

        copy.Collider.enabled = false;
        copy.gameObject.SetActive(false);
        skillData.hitBoxes.Enqueue(copy);

        yield return null;
    }

    protected IEnumerator MoveEachHitBox(HitBox nowHitbox, Collider nowCollider)
    {
        nowHitbox.transform.position = nowCollider.transform.position;
        nowHitbox.Collider.enabled = false;
        nowHitbox.isImmediately = true;
        nowHitbox.gameObject.SetActive(true);
        skillData.SetDamage(nowCollider);
        nowHitbox.StartEffectTimeCountDown();

        while (nowHitbox.isEffectTimeLeft)
        {
            nowHitbox.transform.position = nowCollider.transform.position;
            yield return new WaitForFixedUpdate();
        }

        nowHitbox.gameObject.SetActive(false);
        skillData.hitBoxes.Enqueue(nowHitbox);
        yield return null;
    }
    public void StartMove() => StartCoroutine(StartHitBoxMovement());

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        skillData.skillMovement = (ISkillMovement)this;
    }
}
