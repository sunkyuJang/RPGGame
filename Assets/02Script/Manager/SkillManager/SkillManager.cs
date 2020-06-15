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

public class SkillManager : MonoBehaviour
{
    public GameObject CharacterObj;
    public GameObject SkillViewerObj;
    public GameObject DescriptionBoxObj;

    private Character character;
    
    public Text SkillCount;
    public SkillSheet skillSheet;
    int skillPoint;

    public static SkillManager StaticSkillManager;
    public enum SkillsType { Physic, Magic }
    public static List<Skill> PhysicSkills { private set; get; } = new List<Skill>();
    public static List<Skill> MagicSkills { private set; get; } = new List<Skill>();
    public static List<Skill> RunningSkills { private set; get; } = new List<Skill>();
    const float FOVDeg = Mathf.PI / 4 + Mathf.Rad2Deg;

    public string ClickedSkillName { private set; get; }
    public class Skill : MonoBehaviour
    {
        public SkillSheet.Param data { private set; get; }
        public Sprite Icon { private set; get; }
        public GameObject HitBoxObj { private set; get; }
        public HitBoxCollider HitBoxCollider { private set; get; }
        public bool isLearned { private set; get; }
        public void IsLearn(bool isLearn)
        {
            isLearned = isLearn;
            transform.GetComponent<Image>().color = Color.white;
        }
        public GameObject HitBoxFX { private set; get; }
        public GameObject HitFX { private set; get; }
        public void SetSkillData(SkillSheet.Param sheet, SkillManager skillManager , Transform viewerTransform)
        {
            data = sheet;
            string folderPath = "Character/Animation/Skills/" + sheet.InfluencedBy + "/" + sheet.Name_Eng + "/" + sheet.Name_Eng;
            Icon = Resources.Load<Sprite>(folderPath + "Icon");
            HitBoxObj = Resources.Load<GameObject>(folderPath + "HitBox");

            HitBoxFX = Resources.Load<GameObject>(folderPath + "HitBoxFX");
            HitFX = Resources.Load<GameObject>(folderPath + "HitFX");

            if(HitBoxFX == null) print(data.Name);
            viewerTransform.GetComponent<Image>().sprite = Icon;
            viewerTransform.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            List<EventTrigger.Entry> entry = viewerTransform.GetComponent<EventTrigger>().triggers;
            entry[0].callback.AddListener((data) => { skillManager.SelectedIcon(viewerTransform); });
        }

        public IEnumerator ActivateSkill(Character chracter)
        {
            float endFrom = data.EndFrom + 1f;

            //Activate Hit
            while (!chracter.isHitTriggerActivate)
                yield return new WaitForEndOfFrame();

            float during = data.During == 0f ? 0.5f : data.During;
            float speed = endFrom / during;
            float hitTime = during / data.HitCount;
            float originalDamage = (data.InfluencedBy == "Physic" ? chracter.ATK + chracter.HP : chracter.ATK + (chracter.MP * 10));
            float damage = originalDamage + (originalDamage * (0.01f * data.Damage_Percentage));
            bool isFXStartFromGround = data.FXStartPoint == "Ground" ? true : false;

            Vector3 startPosition = chracter.transform.position;
            
            for (int i = 0; i < data.HitCount; i++)
            {
                HitBoxCollider hitBoxScrip = HitBoxCollider.StartHitBox(HitBoxObj, startPosition, chracter.transform.forward, speed, this);

                float aliveTime= 0;
                while (aliveTime < hitTime)
                {
                    aliveTime += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();

                    if (data.IsDetectCollision)
                    {
                        if (hitBoxScrip.IsEnteredTrigger)
                        {
                            Collider targetCollider = hitBoxScrip.GetColsedCollider(chracter.transform.position);

                            if (targetCollider == null) { print("somting wrong in skillManager ActivateSkill"); }

                            if (data.IsSingleTarget)
                            {
                                SetDamageToTarget(targetCollider, damage, HitFX, isFXStartFromGround);
                                break;
                            }
                            else
                            {
                                var colliders = hitBoxScrip.colliders;
                                for (int colliderCount = 0; colliderCount < colliders.Count; colliderCount++)
                                {

                                    SetDamageToTarget(colliders[colliderCount], damage, HitFX, isFXStartFromGround);
                                    colliders.RemoveAt(colliderCount);
                                }
                            }
                        }
                    }
                }

                if (!data.IsDetectCollision)
                {
                    startPosition = hitBoxScrip.transform.position;

                    var colliders = hitBoxScrip.colliders;
                    if(colliders.Count > 0)
                    for (int colliderCount = 0; colliderCount < colliders.Count; colliderCount++)
                    {
                        SetDamageToTarget(colliders[colliderCount], damage, HitFX, isFXStartFromGround);
                        colliders.RemoveAt(colliderCount);
                    }
                }
                hitBoxScrip.DestroyObj();
            }
            while (chracter.NowAnimatorInfo.IsName(data.Name_Eng))
                yield return new WaitForFixedUpdate();
            DeActivateSkill(this);
        }

        public bool IsThereMonsterAround(Transform characterTransform)
        {
            return GPosition.GetSelectedColliderInFOV(characterTransform.transform, data.EndFrom + 1, FOVDeg, "Monster").Count > 0;
        }

        void SetDamageToTarget(Collider target, float damage, GameObject hitFX, bool isFXStartFromGround) 
        { 
            if (target != null) target.GetComponent<Monster>().GetHit(damage, hitFX, isFXStartFromGround); 
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
    public static void DeActivateSkill(Skill skill) 
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
        Transform skillViewerTransform = SkillViewerObj.transform.GetChild(0).GetChild(0);

        for (int i = 0; i < sheet.Count; i++)
        {
            if(sheet[i].Index != 0)
            {
                GameObject nowObj = skillViewerTransform.Find(sheet[i].Name_Eng).gameObject;
                Skill skill = nowObj.AddComponent<Skill>();
                skill.SetSkillData(sheet[i], this, nowObj.transform);

                var nowList = skill.data.InfluencedBy == "Physic" ? PhysicSkills : MagicSkills;
                nowList.Add(skill);
            }
            else break;
        }
        SkillViewerObj.SetActive(false);
    }

    private void Start()
    {
        skillPoint = character.level;
        LearnSkill(GetSkill(true, 1), true);
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
        DescriptionBoxObj.SetActive(false);
        character.IntoNomalUI();
    }
    public void SelectedIcon(Transform viewer)
    {
        bool isTouch;
        int touchId = 0;
        bool isMouse;
        if (GPosition.IsContainInput(viewer.GetComponent<RectTransform>(), out isTouch ,out touchId, out isMouse))
        {
            StartCoroutine(TraceInput(viewer, isTouch, touchId, isMouse));
        }
        else
            print("somting Wrong in SkillManager_SelectedIcon");
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
            DescriptionBoxObj.transform.GetChild(0).GetComponent<Image>().sprite = viewer.GetComponent<Skill>().Icon;
            DescriptionBoxObj.transform.GetChild(1).GetComponent<Text>().text = viewer.GetComponent<Skill>().data.Description;
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
