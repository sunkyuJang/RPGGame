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
        public bool isCoolDownNow { set; get; } = false;
        public GameObject HitBoxFX { private set; get; }
        public GameObject HitFX { private set; get; }
        public void SetSkillData(SkillSheet.Param sheet, SkillManager skillManager, Transform viewerTransform)
        {
            data = sheet;
            string folderPath = "Character/Animation/Skills/" + sheet.InfluencedBy + "/" + sheet.Name_Eng + "/" + sheet.Name_Eng;
            Icon = Resources.Load<Sprite>(folderPath + "Icon");
            HitBoxObj = Resources.Load<GameObject>(folderPath + "HitBox");

            HitBoxFX = Resources.Load<GameObject>(folderPath + "HitBoxFX");
            HitFX = Resources.Load<GameObject>(folderPath + "HitFX");

            viewerTransform.GetComponent<Image>().sprite = Icon;
            viewerTransform.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            List<EventTrigger.Entry> entry = viewerTransform.GetComponent<EventTrigger>().triggers;
            entry[0].callback.AddListener((data) => { skillManager.SelectedIcon(this); });
        }

        public IEnumerator ActivateSkill(Character chracter)
        {
            if(data.Cooldown > 0f) StaticManager.coroutineStart(CountCoolDown());
            float endFrom = data.Duration + 1f;

            //Activate Hit
            while (!chracter.isHitTriggerActivate)
                yield return new WaitForEndOfFrame();

            float during = data.Duration == 0f ? 0.5f : data.Duration;
            float speed = endFrom / during;
            float hitTime = during / data.HitCount;
            float originalDamage = (data.InfluencedBy == "Physic" ? chracter.ATK + chracter.HP : chracter.ATK + (chracter.MP * 10));
            float damage = originalDamage + (originalDamage * (0.01f * data.Damage_Percentage));
            bool isFXStartFromGround = data.FXStartPoint == "Ground" ? true : false;

            Vector3 startPosition = chracter.transform.position;

            for (int i = 0; i < data.HitCount; i++)
            {
                HitBoxCollider hitBoxScrip = HitBoxCollider.StartHitBox(HitBoxObj, startPosition, chracter.transform.forward, speed, this);

                float aliveTime = 0;
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
                    if (colliders.Count > 0)
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

        IEnumerator CountCoolDown()
        {
            isCoolDownNow = true;
            var time = 0f;
            while(time < data.Cooldown)
            {
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            isCoolDownNow = false;
        }

        public bool IsThereMonsterAround(Transform characterTransform)
        {
            return GPosition.GetSelectedColliderInFOV(characterTransform.transform, data.Length + 1, FOVDeg, "Monster").Count > 0;
        }

        void SetDamageToTarget(Collider target, float damage, GameObject hitFX, bool isFXStartFromGround)
        {
            if (target != null) target.GetComponent<Monster>().GetHit(StaticSkillManager.character, damage, hitFX, isFXStartFromGround);
        }
    }

    public void SelectedIcon(Skill viewer)
    {
        bool isTouch;
        int touchId = 0;
        bool isMouse;
        if (GPosition.IsContainInput(viewer.GetComponent<RectTransform>(), out isTouch, out touchId, out isMouse))
        {
            StartCoroutine(TraceInput(viewer, isTouch, touchId, isMouse));
        }
        else
            print("somting Wrong in SkillManager_SelectedIcon");
    }

    IEnumerator TraceInput(Skill viewer, bool isTouch, int touchId, bool isMouse)
    {
        Transform copy = Instantiate(viewer.transform, transform.root);
        while (GPosition.IsHoldPressedInput(isTouch, touchId, isMouse))
        {
            copy.position = isTouch ? (Vector3)Input.touches[touchId].position : Input.mousePosition;
            yield return new WaitForEndOfFrame();
        }

        if (GMath.GetRect(viewer.GetComponent<RectTransform>()).Contains(copy.transform.position))
        {
            discriptionBox.ShowDiscription(viewer);
        }
        else if (character.QuickSlot.gameObject.activeSelf)
        {
            QuickSlot quickSlot = character.QuickSlot;
            int num = quickSlot.IsIn((Vector2)copy.position);
            if (viewer.isLearned)
            {
                if (num >= 0)
                {
                    quickSlot.SetSlot(viewer.transform, num);
                }
            }
            else
            {
                StaticManager.ShowAlert("아직 배우지 않은 스킬입니다", Color.red);
            }
        }

        Destroy(copy.gameObject);
    }
}
