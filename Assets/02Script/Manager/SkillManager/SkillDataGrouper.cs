﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDataGrouper : MonoBehaviour
{
    public static SkillDataGrouper instance;
    public Transform CharacterGroup;
    public Transform MonsterGroup;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.position = Vector3.zero;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void SetSkillDataParent(SkillData skillData)
    {
        Transform group = skillData.Model is Character ? CharacterGroup : MonsterGroup;
        Transform nowTransform = group; // temp value

        if (group.childCount > 0)
        {
            foreach (Transform sameName in group)
                if (sameName.name == skillData.Model.CharacterName)
                    nowTransform = sameName;
        }
        else
            nowTransform = new GameObject(skillData.Model.CharacterName).transform;
        
        nowTransform.position = Vector3.zero;
        skillData.hitBoxPooler.transform.SetParent(nowTransform);
        //skillData.skillPooling.SetParent(nowTransform.Find("PoolerObj"));

        nowTransform.SetParent(group);
    }
}
