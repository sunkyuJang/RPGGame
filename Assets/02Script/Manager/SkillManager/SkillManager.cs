using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;
using System.Linq;
using System;
using UnityScript.Steps;
using System.Runtime.Remoting.Messaging;
using UnityEditorInternal;
using UnityEngine.EventSystems;
using UnityEditor;
using System.Runtime.CompilerServices;
using System.ComponentModel;

public partial class SkillManager : MonoBehaviour
{
    public GameObject CharacterObj;
    public GameObject SkillViewerObj;

    DiscriptionBox discriptionBox { set; get; }

    public Character character { private set; get; }
    
    public Text SkillCount;
    public SkillSheet skillSheet;
    public int skillPoint;

    public static SkillManager StaticSkillManager;
    public static List<Skill> skillList { private set; get; } = new List<Skill>();
    const float FOVDeg = Mathf.PI / 4 + Mathf.Rad2Deg;    
    public static Skill GetSkill(int index) { return skillList[index]; }

    private void Awake()
    {
        CreatViewer();
        character = CharacterObj.GetComponent<Character>();
        discriptionBox = DiscriptionBox.AddDiscriptionScript(SkillViewerObj.transform.Find("DescriptionBox"), character);
        StaticSkillManager = this;
        var sheet = skillSheet.sheets[0].list;
        Transform skillViewerTransform = SkillViewerObj.transform.GetChild(0).GetChild(0);

        for (int i = 0; i < sheet.Count; i++)
        {
            if(sheet[i].Name != "")
            {
                GameObject nowObj = skillViewerTransform.Find(sheet[i].Name_Eng).gameObject;
                Skill skill = nowObj.AddComponent<Skill>();
                skill.SetSkillData(sheet[i], this, nowObj.transform);

                skillList.Add(skill);
            }
            else break;
        }
        SkillViewerObj.SetActive(false);
    }

    private void Start()
    {
        skillPoint = character.level;
        LearnSkill(GetSkill(0), true);
    }

    public static void ActivateSkiil(Skill skill, Character character)
    {
        StaticSkillManager.StartCoroutine(skill.ActivateSkill(character));
    }
    public void CreatViewer()
    {
        SkillViewerObj = Instantiate(SkillViewerObj, transform.parent);
        SkillViewerObj.transform.Find("CloseButton").GetComponent<Button>().onClick.AddListener(HideSkillViewer);
        SkillCount = SkillViewerObj.transform.Find("SkillPointText").GetChild(0).GetComponent<Text>();
    }
    public void LearnSkill(Skill skill, bool isLearn)
    {
        skill.IsLearn(isLearn);
        if (isLearn) skillPoint--;
        else skillPoint++;
        RefreshSkillCount();
    }
    void RefreshSkillCount()
    {
        SkillCount.text = skillPoint.ToString();
    }

    public void ShowSkillViewer() => SkillViewerObj.SetActive(true);
    public void HideSkillViewer()
    {
        SkillViewerObj.SetActive(false);
        discriptionBox.gameObject.SetActive(false);
        character.ShowGameUI(true);
    }
    
    class DiscriptionBox : MonoBehaviour
    {
        public Image icon { private set; get; }
        public Text text { private set; get; }
        public Button button { private set; get; }
        public Character character { private set; get; }
        Skill skill { set; get; }
        public static DiscriptionBox AddDiscriptionScript(Transform transform, Character character)
        {
            var discription = transform.gameObject.AddComponent<DiscriptionBox>();
            discription.character = character;
            discription.gameObject.SetActive(false);
            return discription;
        }
        void Awake()
        {
            icon = transform.Find("Icon").GetComponent<Image>();
            text = transform.Find("DescriptionText").GetComponent<Text>();
            button = transform.Find("Button").GetComponent<Button>();
            button.onClick.AddListener(ClickLearnButton);
        }
        public void ShowDiscription(Skill data)
        {
            skill = data;
            icon.sprite = data.Icon;
            text.text = data.data.Description;
            gameObject.SetActive(true);
            if (!data.isLearned) { button.gameObject.SetActive(true); }
            else { button.gameObject.SetActive(false); }
        }
        public void ClickLearnButton() 
        {
            if (StaticSkillManager.skillPoint > 0)
            {
                if (GetSkill(skill.data.ParentIndex).isLearned)
                {
                    StaticSkillManager.LearnSkill(skill, true);
                    button.gameObject.SetActive(false);
                }
                else
                {
                    character.ShowAlert("선행 스킬이 습득되지 않았습니다.", Color.red);
                }
            }
            else
                character.ShowAlert("스킬포인트가 부족합니다", Color.red);
        }
    }
}

    //public static List<Skill> RunningSkills { private set; get; } = new List<Skill>();
/*    public static bool IsDeActivateSkill (Skill skill) 
    { 
        foreach(Skill nowSkill in RunningSkills)
            if (skill.data.Index == nowSkill.data.Index) 
                return false;
        return true;
    }
*//*    public static void ActivateSkiil(Skill skill, Character character)
    {
        StaticSkillManager.StartCoroutine(skill.ActivateSkill(character));
        RunningSkills.Add(skill);
    }
    public static void DeActivateSkill(Skill skill) 
    { 
        RunningSkills.Remove(skill);
    }*/