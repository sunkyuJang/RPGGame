/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;

public class SkillCurse : SkillData, ISkillActivator
{
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

        copy.transform.position = targetModel.transform.position;

        yield return copy.CheckObjCollideInTime();

        if (copy.isCollide)
        {
            var target = copy.GetTarget(Model.transform.position);
            for (int i = 0; i < target.Count; i++) 
            {
                StartCoroutine(MoveEachHitBox(GetHitBoxWithOutSetUp(), target[i]));
            }
        }

        copy.Collider.enabled = false;
        copy.gameObject.SetActive(false);
        hitBoxes.Enqueue(copy);

        yield return null;
    }

    protected IEnumerator MoveEachHitBox(HitBox nowHitbox, Collider nowCollider)
    {
        nowHitbox.transform.position = nowCollider.transform.position;
        nowHitbox.Collider.enabled = false;
        nowHitbox.isImmediately = true;
        nowHitbox.gameObject.SetActive(true);
        SetDamage(nowCollider);
        nowHitbox.StartEffectTimeCountDown();

        while (nowHitbox.isEffectTimeLeft)
            yield return new WaitForFixedUpdate();

        nowHitbox.gameObject.SetActive(false);
        hitBoxes.Enqueue(nowHitbox);
        yield return null;
    }
}
*/