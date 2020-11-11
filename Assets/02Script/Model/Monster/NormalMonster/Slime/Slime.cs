using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
public class Slime : NormalMonster
{
    new private void Awake()
    {
        SetInfo("허접한 슬라임", 100, 0, 15, 10, 5);
        base.Awake();
    }
    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    new protected void OnEnable()
    {
        base.OnEnable();
        AddItem(0, 1, 0.8f, 100);
        AddItem(2, 1, 0.8f, 100);
    }

    new private void OnDisable()
    {
        base.OnDisable();
    }

    new private void FixedUpdate()
    {
        base.FixedUpdate();
        SelectedNextAction();
    }
    new private void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    protected override IEnumerator DoAttack()
    {
        canAttack = false;
        canGetHit = false;
        DoAnimator(ActionState.attack);

        yield return StartCoroutine(WaitTillAnimator("NomalAttack", true));

        var nowSkill = GetSkillData(SkillDataName + "NormalAttack");
        nowSkill.ActivateSkill();

        while (BeforeState == ActionState.attack)
        {
            yield return new WaitForFixedUpdate();
            transform.LookAt(Character.transform.position);

            if (NowAnimatorInfo.normalizedTime >= 0.9f)
            {
                NowState = ActionState.battle;
                canGetHit = true;
                StartCoroutine(StartAttackDelayTimer(2f));
                break;
            }
        }
    }
}
