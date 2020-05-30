using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;
using System.Linq;
using System;
using UnityScript.Steps;
using System.Runtime.Remoting.Messaging;

public class SkillManager : MonoBehaviour
{
    public SkillSheet skillSheet;
    public static SkillManager StaticSkillManager;
    public enum SkillsType { Physic, Magic }
    public static List<Skill> PhysicSkills { private set; get; } = new List<Skill>();
    public static List<Skill> MagicSkills { private set; get; } = new List<Skill>();
    public static List<Skill> RunningSkills { private set; get; } = new List<Skill>();
    const float FOVDeg = Mathf.PI / 4;
    public class Skill
    {
        public SkillSheet.Param data { private set; get; }
        public Sprite Icon { private set; get; }
        public GameObject HitBoxObj{ private set; get; }
        public HitBoxCollider HitBoxCollider { private set; get; }
        public Skill(SkillSheet.Param sheet)
        {
            data = sheet;
            string folderPath = "Character/Animation/Skills/" + sheet.InfluencedBy + "/" + sheet.Name_Eng + "/" + sheet.Name_Eng;
            Icon = Resources.Load<Sprite>(folderPath + "Icon");
            HitBoxObj = Resources.Load<GameObject>(folderPath + "HitBox");
            if (Icon == null) print("null in " + data.Name + "//" + folderPath);
        }

        public IEnumerator ActivateSkill(Character chracter)
        {
            float endFrom = data.EndFrom + 1f;
            var NearbyCharacter = GPosition.GetSelectedColliderInFOV(chracter.transform, endFrom, FOVDeg, "Monster");
            print(NearbyCharacter.Count);

            if (NearbyCharacter.Count > 0) {
                RunningSkills.Add(this);

                //Activate Hit
                while (!chracter.isHitTriggerActivate) 
                    yield return new WaitForEndOfFrame();
                chracter.HitTrigger(0);


                float during = data.During == 0f ? 0.5f : data.During;
                float speed = endFrom / during;
                float hitTime = during / data.HitCount;
                for (int i = 0; i < data.HitCount; i++)
                {
                    HitBoxCollider hitBoxScrip = HitBoxCollider.StartHitBox(HitBoxObj, chracter.transform, speed);
                    print("isin Scrup");

                    float aliveTime= 0;
                    while (aliveTime < hitTime)
                    {
                        aliveTime += Time.fixedDeltaTime;
                        yield return new WaitForFixedUpdate();

                        if (hitBoxScrip.IsEnteredTrigger)
                        {
                            Collider targetCollider = hitBoxScrip.GetColsedCollider(chracter.transform.position);
                            if (targetCollider == null) { print("somting worng in skillManager ActivateSkill"); }
                            
                            int damage = (data.InfluencedBy == "Physic" ? chracter.ATK + chracter.HP : chracter.ATK + (chracter.MP * 10)) + (data.Damage_Percentage + 100);
                            
                            if (data.IsSingleTarget)
                            {
                                Monster targetMonster = targetCollider.GetComponent<Monster>();
                                targetMonster.GetHit(damage);
                                Destroy(hitBoxScrip.gameObject);
                            }
                            else
                            {
                                foreach(Collider collider in hitBoxScrip.colliders)
                                {
                                    collider.GetComponent<Monster>().GetHit(damage);
                                    hitBoxScrip.colliders.Remove(collider);
                                }
                            }
                        }
                    }

                    Destroy(hitBoxScrip.gameObject);
                }
                RunningSkills.Remove(this); 
            }
        }
    }
    public static Skill GetSkill(bool isPhysic, int skillTier, int index) 
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
    private void Awake()
    {
        StaticSkillManager = this;
        var sheet = skillSheet.sheets[0].list;
        for (int i = 0; i < sheet.Count; i++)
        {
            if(sheet[i].Index != 0)
            {
                var nowList = sheet[i].InfluencedBy == "Physic" ? PhysicSkills : MagicSkills;
                nowList.Add(new Skill(sheet[i]));
            }
            else break;
        }
    }


}
