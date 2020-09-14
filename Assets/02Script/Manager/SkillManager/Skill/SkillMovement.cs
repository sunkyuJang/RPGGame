using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
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
        yield return StartCoroutine(skillData.hitBoxPullingController.CheckCanUseObj(skillData.hitBoxNum));
    }
}
