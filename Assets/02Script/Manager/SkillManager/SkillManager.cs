using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;
using System.Linq;
using System;
using UnityScript.Steps;

public class SkillManager : MonoBehaviour
{
    public SkillSheet skillSheet;
    public enum SkillsType { Physic, Magic }
    public List<Skill> PhysicSkills { private set; get; } = new List<Skill>();
    public List<Skill> MagicSkills { private set; get; } = new List<Skill>();
    public List<Skill> RunningSkills { private set; get; }
    const float FOVDeg = Mathf.PI / 4;
    public class Skill
    {
        public SkillSheet.Param data { private set; get; }
        public Sprite Icon { private set; get; }
        public GameObject HitBoxObj{ private set; get; }
        public HitBoxCollider HitBoxCollider { private set; get; }
        public bool IsRunning { private set; get; }
        public Skill(SkillSheet.Param sheet)
        {
            data = sheet;
            string folderPath = "Character/Animation/Skills/" + sheet.InfluencedBy + "/" + sheet.Name_Eng + "/" + sheet.Name_Eng;
            Icon = Resources.Load<Sprite>(folderPath + "Icon");
            HitBoxObj = Resources.Load<GameObject>(folderPath + "HitBox");
            HitBoxCollider = HitBoxObj.GetComponent<HitBoxCollider>();
            if (Icon == null) print("null in " + data.Name + "//" + folderPath);
        }

        public IEnumerator ActivateSkill(Character chracter)
        {
            IsRunning = true;
            
            chracter.ActivateSkill(data.InfluencedBy == "Physic" ? true : false, data.SkillTier, data.Index);

            var NearbyCharacter = GPosition.GetSelectedColliderInFOV(chracter.transform, data.EndFrom, FOVDeg, "Monster");
            //Physics.OverlapSphere(chracter.transform.position, data.EndFrom, (int)GGameInfo.LayerMasksList.Floor, QueryTriggerInteraction.Ignore).ToList<Collider>();
            /*GPosition.GetNearByObj(chracter.transform.position, data.EndFrom);
            NearbyCharacter = GPosition.SelectColliderInFOV(NearbyCharacter, "Monster", chracter.transform, FOVDeg);*/


            /*if (reachedObj.Count > 0)
            {
                double rad = Math.PI / 16;
                switch (data.StartRange_Rad)
                {
                    case "Straigth": rad *= 2; break;
                    case "Wide": rad *= 4; break;
                    case "Foward": rad *= 8; break;
                    case "Back": rad *= -8; break;
                    case "AllAround": rad *= 16; break;
                }
            }*/
            if (NearbyCharacter.Count > 0) {
                //Activate Hit
                while (!chracter.isHitTriggerActivate) yield return new WaitForEndOfFrame();
                chracter.HitTrigger(0);

                bool isStartAttack = false;
                float hitTime = data.During / data.HitCount;
                for (int i = 0; i < data.HitCount; i++)
                {
                    HitBoxCollider.StartMove(chracter.transform.position, )
                    float during = 0;
                    while (during < hitTime) yield return during += Time.fixedDeltaTime;


                }
            }
            IsRunning = false;
        }
    }

    private void Awake()
    {
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
