﻿using System.Collections;
using UnityEngine;

public class SkillMovement : MonoBehaviour
{
    protected SkillData skillData;
    protected Model model { get { return skillData.Model; } }
    protected void Start()
    {
        skillData = GetComponent<SkillData>();
    }

    protected IEnumerator StartHitBoxMovement()
    {
        yield return StartCoroutine(skillData.hitBoxPooler.CheckCanUseObj(skillData.hitBoxNum));
    }
}
