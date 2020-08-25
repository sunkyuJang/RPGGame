using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMovement : MonoBehaviour
{
    protected SkillData skillData;

    protected void Start()
    {
        skillData = GetComponent<SkillData>();
    }

    protected void StartMovement() 
    {
        skillData.ActivateSkill();
        StartCoroutine(StartHitBoxMove());
    }

    protected virtual IEnumerator StartHitBoxMove() { yield return null; }
}
