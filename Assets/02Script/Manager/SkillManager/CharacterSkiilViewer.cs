using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkiilViewer : MonoBehaviour
{
    public Character Character { set; get; }
    public GameObject skillViewGroup;
    public List<SkillData> skillDatas { private set; get; } = new List<SkillData>();
    List<SkillViewer> skillViewers { set; get; } = new List<SkillViewer>();
    public GameObject skillTreeViewer;
    public CharacterSkillDescriptionBox characterSkillDescriptionBox;

    public SkillData SkillNormalAttack { set; get; }

    SkillViewer nowSkillViewer;

    private void Awake()
    {
        foreach (Transform transform in skillViewGroup.transform)
            skillViewers.Add(transform.GetComponent<SkillViewer>());
    }

    private void Start()
    {
//        SkillTreeSet();

        skillTreeViewer.SetActive(false);
        characterSkillDescriptionBox.gameObject.SetActive(false);
    }

    public void SetCharater(Character character)
    {
        Character = character;
        StartCoroutine(SetSkillListHandler(character.skillListHandler));
    }

/*    void SkillTreeSet()
    {
        for (int i = 0; i < 2; i++)
        {
            var group = i == 0 ? physicSkillGroup.transform : MagicSkillGroup.transform;
            var count = group.childCount;

            for (int j = 0; j < count; j++)
            {
                var skillData = group.GetChild(j).GetComponent<SkillData>();
                skillData.Model = character;
                SkillDataGrouper.instance.SetSkillDataParent(skillData);

                for (int k = 0; k < skillViewers.Count; k++)
                {
                    var nowSkillViewer = skillViewers[k];
                    if (skillData.skillName_eng == nowSkillViewer.gameObject.name)
                    {
                        nowSkillViewer.skillData = skillData;
                        nowSkillViewer.characterSkiilViewer = this;
                        nowSkillViewer.MakeImage();
                        skillViewers.RemoveAt(k);
                        break;
                    }
                }
            }
        }
    }*/
    IEnumerator SetSkillListHandler(SkillListHandler listHandler)
    {
        yield return new WaitWhile(() => !listHandler.StartPass);
        var skillList = listHandler.skillDatas;
        foreach (SkillData nowSkillData in skillList)
            for (int i = 0; i < skillViewers.Count; i++)
            {
                var skillViewer = skillViewers[i];
                if (skillViewer.name == nowSkillData.skillName_eng)
                {
                    skillViewer.SetSkillData(nowSkillData, this);
                    skillViewers.RemoveAt(i);

                    if (nowSkillData.skillName_eng.Equals("NormalAttack"))
                        SkillNormalAttack = nowSkillData;
                }
            }
    }

    public SkillData GetSkillData(string name)
    {
        foreach (SkillData skillData in skillDatas)
            if (skillData.skillName_eng == name)
                return skillData;

        return null;
    }

    public void ShowDescription(SkillViewer viewer)
    {
        if (!characterSkillDescriptionBox.gameObject.activeSelf)
            characterSkillDescriptionBox.gameObject.SetActive(true);
        
        nowSkillViewer = viewer;
        characterSkillDescriptionBox.ShowDescription(viewer.skillData);
    }
    public void ShowSKillTree()
    {
        skillTreeViewer.gameObject.SetActive(true);
    }
    public void HideSkillTree()
    {
        Character.IntoNormalUI();
    }

    public void PressedLearnBtn()
    {
        if (Character.SkillPoint > 0)
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
        => Character.ShowAlert(text, color);
}