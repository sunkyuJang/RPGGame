﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkiilViewer : MonoBehaviour
{
    public Character character { private set; get; }
    Transform skillPulling;
    public GameObject physicSkillGroup;
    public GameObject MagicSkillGroup;
    public GameObject skillTreeViewer;
    public CharacterSkillDescriptionBox characterSkillDescriptionBox;

    public SkillNormalAttack SkillNormalAttack;

    SkillViewer nowSkillViewer;
    private void Awake()
    {
        skillPulling = new GameObject("CharacterSkillPulling").GetComponent<Transform>();
    }

    private void Start()
    {
        character = StaticManager.Character;
        SkillTreeSet();

        SkillNormalAttack = physicSkillGroup.transform.Find("SkillNormalAttack").GetComponent<SkillNormalAttack>();
    }

    void SkillTreeSet()
    {
        for (int i = 0; i < 2; i++)
        {
            var group = i == 0 ? physicSkillGroup.transform : MagicSkillGroup.transform;
            var count = group.childCount;

            for (int j = 0; j < count; j++)
            {
                var skillData = group.GetChild(j).GetComponent<SkillData>();
                skillData.Model = character.transform;
                try
                {
                    skillData.skillpulling.parent = skillPulling;
                }
                catch 
                {
                    if(!skillData.gameObject.activeSelf)
                        print(skillData.skillName_kor); 
                }
            }
        }
    }

    public void ShowDescription(SkillViewer viewer)
    {
        if (!characterSkillDescriptionBox.gameObject.activeSelf)
            characterSkillDescriptionBox.gameObject.SetActive(true);
        
        nowSkillViewer = viewer;
        characterSkillDescriptionBox.ShowDescription(viewer.skillData);
    }

    public void PressedLearnBtn()
    {
        if (character.SkillPoint > 0)
        {
            if (nowSkillViewer.skillData.parentSkillData.isLearn)
            {
                nowSkillViewer.skillData.isLearn = true;
                nowSkillViewer.SetLearnedIcon();
                ShowDescription(nowSkillViewer);
            }
            else
                UseCharacterAlert("상위스킬을 먼저 배우셔야 합니다.", Color.red);
        }
        else
            UseCharacterAlert("스킬포인트가 부족합니다.", Color.red);
    }

    public void UseCharacterAlert(string text, Color color)
        => character.ShowAlert(text, color);

    public void DoNormalAttack() 
    {
        SkillNormalAttack.ActivateSkill();
    }
}