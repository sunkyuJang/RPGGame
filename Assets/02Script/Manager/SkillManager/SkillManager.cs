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

public class SkillManager : MonoBehaviour
{
    public GameObject CharacterObj;
    public GameObject SkillViewerObj;
    public GameObject DescriptionBoxObj;

    private Character character;
    
    public Text SkillCount;
    public SkillSheet skillSheet;

    public static SkillManager StaticSkillManager;
    public enum SkillsType { Physic, Magic }
    public static List<Skill> PhysicSkills { private set; get; } = new List<Skill>();
    public static List<Skill> MagicSkills { private set; get; } = new List<Skill>();
    public List<Skill> LearnedSkills { private set; get; } = new List<Skill>();
    public string SkillPoint { get { return (character.level - LearnedSkills.Count).ToString(); } }
    public static List<Skill> RunningSkills { private set; get; } = new List<Skill>();
    const float FOVDeg = Mathf.PI / 4 + Mathf.Rad2Deg;

    public string ClickedSkillName { private set; get; }
    public class Skill
    {
        public SkillSheet.Param data { private set; get; }
        public Sprite Icon { private set; get; }
        public GameObject HitBoxObj{ private set; get; }
        public HitBoxCollider HitBoxCollider { private set; get; }
        public Transform ViewerTransform { private set; get; }
        public class SkillViewer : MonoBehaviour
        { 
            public Skill Skill { set; get; }
        }
        public Skill(SkillSheet.Param sheet, SkillManager skillManager , Transform viewerTransform)
        {
            data = sheet;
            string folderPath = "Character/Animation/Skills/" + sheet.InfluencedBy + "/" + sheet.Name_Eng + "/" + sheet.Name_Eng;
            Icon = Resources.Load<Sprite>(folderPath + "Icon");
            HitBoxObj = Resources.Load<GameObject>(folderPath + "HitBox");

            ViewerTransform = viewerTransform.Find(data.Name_Eng);
            ViewerTransform.gameObject.AddComponent<SkillViewer>();
            ViewerTransform.GetComponent<SkillViewer>().Skill = this;
            ViewerTransform.GetComponent<Image>().sprite = Icon;
            ViewerTransform.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            List<EventTrigger.Entry> entry = ViewerTransform.GetComponent<EventTrigger>().triggers;
            entry[0].callback.AddListener((data) => { skillManager.SelectedIcon(ViewerTransform, this); });
        }

        public IEnumerator ActivateSkill(Character chracter)
        {
            float endFrom = data.EndFrom + 1f;
            var NearbyCharacter = GPosition.GetSelectedColliderInFOV(chracter.transform, endFrom, FOVDeg, "Monster");

            if (NearbyCharacter.Count > 0 && chracter.NowState == Character.ActionState.Attack) {

                //Activate Hit
                while (!chracter.isHitTriggerActivate)
                    yield return new WaitForEndOfFrame();

                float during = data.During == 0f ? 0.5f : data.During;
                float speed = endFrom / during;
                float hitTime = during / data.HitCount;
                for (int i = 0; i < data.HitCount; i++)
                {
                    HitBoxCollider hitBoxScrip = HitBoxCollider.StartHitBox(HitBoxObj, chracter.transform, speed);

                    float aliveTime= 0;
                    while (aliveTime < hitTime)
                    {
                        aliveTime += Time.fixedDeltaTime;
                        yield return new WaitForFixedUpdate();

                        if (hitBoxScrip.IsEnteredTrigger)
                        {
                            Collider targetCollider = hitBoxScrip.GetColsedCollider(chracter.transform.position);
                            
                            if (targetCollider == null) { print("somting wrong in skillManager ActivateSkill"); }
                            
                            int damage = (data.InfluencedBy == "Physic" ? chracter.ATK + chracter.HP : chracter.ATK + (chracter.MP * 10)) / (data.Damage_Percentage + 10);
                            
                            if (data.IsSingleTarget)
                            {
                                Monster targetMonster = targetCollider.GetComponent<Monster>();
                                targetMonster.GetHit(damage);
                                break;
                            }
                            else
                            {
                                var colliders = hitBoxScrip.colliders;
                                for (int colliderCount = 0; colliderCount < colliders.Count; colliderCount++)
                                {
                                    colliders[colliderCount].GetComponent<Monster>().GetHit(damage);
                                    colliders.RemoveAt(colliderCount);
                                }
                            }
                        }
                    }

                    Destroy(hitBoxScrip.gameObject);
                }
                while (chracter.NowAnimatorInfo.IsName(data.Name_Eng))
                    yield return new WaitForFixedUpdate();
                DeActivateSkill(this, chracter);
            }
        }
    }
    public static Skill GetSkill(bool isPhysic, int index) 
    {
        var list = isPhysic ? PhysicSkills : MagicSkills;
        foreach(Skill skill in list)
        {
            if(skill.data.Index == index) { return skill; }
        }
        return null;
    }
    public static bool IsDeActivateSkill (Skill skill) 
    { 
        foreach(Skill nowSkill in RunningSkills)
            if (skill.data.Index == nowSkill.data.Index) 
                return false;
        return true;
    }
    public static void ActivateSkiil(Skill skill, Character character)
    {
            StaticSkillManager.StartCoroutine(skill.ActivateSkill(character));
            RunningSkills.Add(skill);
    }
    public static void DeActivateSkill(Skill skill, Character character) 
    { 
        RunningSkills.Remove(skill);
    }
    private void Awake()
    {
        CreatViewer();
        DescriptionBoxObj = SkillViewerObj.transform.Find("DescriptionBox").gameObject;
        DescriptionBoxObj.SetActive(false);
        character = CharacterObj.GetComponent<Character>();
        StaticSkillManager = this;
        var sheet = skillSheet.sheets[0].list;
        for (int i = 0; i < sheet.Count; i++)
        {
            if(sheet[i].Index != 0)
            {
                Skill skill = new Skill(sheet[i], this, SkillViewerObj.transform.GetChild(0).GetChild(0));
                var nowList = skill.data.InfluencedBy == "Physic" ? PhysicSkills : MagicSkills;
                nowList.Add(skill);
            }
            else break;
        }

        LearnedSkills.Add(GetSkill(true, 1));
        SkillViewerObj.SetActive(false);
    }

    private void Start()
    {
        SkillCount.text = SkillPoint;
        foreach(Skill skill in LearnedSkills) { skill.ViewerTransform.GetComponent<Image>().color = Color.white; }
    }

    public void CreatViewer()
    {
        SkillViewerObj = Instantiate(SkillViewerObj, transform.parent);
        SkillViewerObj.transform.Find("CloseButton").GetComponent<Button>().onClick.AddListener(HideSkillViewer);
        SkillCount = SkillViewerObj.transform.Find("SkillPointText").GetChild(0).GetComponent<Text>();
    }

    public void ShowSkillViewer() => SkillViewerObj.SetActive(true);
    public void HideSkillViewer()
    {
        SkillViewerObj.SetActive(false);
        DescriptionBoxObj.SetActive(false);
        character.IntoNomalUI();
    }
    public void SelectedIcon(Transform viewer, Skill skill)
    {
        bool isTouch;
        int touchId = 0;
        bool isMouse;
        if (GPosition.IsContainInput(viewer.GetComponent<RectTransform>(), out isTouch ,out touchId, out isMouse))
        {
            StartCoroutine(TraceInput(viewer, isTouch, touchId, isMouse));
        }
        else
        {
            print("somting Wrong in SkillManager_SelectedIcon");
        }
    }

    IEnumerator TraceInput(Transform viewer, bool isTouch, int touchId, bool isMouse)
    {
        Transform copy = Instantiate(viewer, transform.root);
        while(GPosition.IsHoldPressedInput(isTouch, touchId, isMouse))
        {
            copy.position = isTouch ? (Vector3)Input.touches[touchId].position : Input.mousePosition;
            yield return new WaitForEndOfFrame();
        }

        if(GMath.GetRect(viewer.GetComponent<RectTransform>()).Contains(copy.transform.position))
        {
            DescriptionBoxObj.SetActive(true);
            DescriptionBoxObj.transform.GetChild(0).GetComponent<Image>().sprite = viewer.GetComponent<Skill.SkillViewer>().Skill.Icon;
            DescriptionBoxObj.transform.GetChild(1).GetComponent<Text>().text = viewer.GetComponent<Skill.SkillViewer>().Skill.data.Description;
        }
        else if (character.QuickSlot.gameObject.activeSelf)
        {
            QuickSlot quickSlot = character.QuickSlot;
            int num = quickSlot.IsIn((Vector2)copy.position);
            if(num >= 0)
            {
                quickSlot.SetSlot(viewer, num);
            }
        }

        Destroy(copy.gameObject);
    }
}
