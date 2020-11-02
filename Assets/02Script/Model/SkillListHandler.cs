using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillListHandler : MonoBehaviour
{
    public List<SkillData> skillDatas { private set; get; } = new List<SkillData>();

    public Transform physicalGroup;
    public Transform magicalGroup;

    public bool StartPass = false;
    private void Start()
    {
        var model = transform.parent.GetComponent<Model>();
        var group = physicalGroup;
        
        for (int i = 0; i < 2; i++)
        {
            foreach (Transform nowTransform in group)
            {
                var nowSkillData = nowTransform.GetComponent<SkillData>();
                nowSkillData.Model = model;
                nowSkillData.SetPooler();
                //yield return new WaitUntil(() => nowSkillData.hitBoxPooler != null);
                SkillDataGrouper.instance.SetSkillDataParent(nowSkillData);
                skillDatas.Add(nowSkillData);
            }
            if (group.Equals(physicalGroup))
                group = magicalGroup;
            else
                break;
        }

        StartPass = true;
    }
}
