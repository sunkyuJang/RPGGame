using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillListHandler : MonoBehaviour
{
    public List<SkillData> skillDatas { private set; get; } = new List<SkillData>();
    public SkillData GetSkillData(string skillDataName)
    {
        foreach (SkillData data in skillDatas)
            if (data.skillName_eng.Equals(skillDataName))
                return data;
        return null;
    }
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
