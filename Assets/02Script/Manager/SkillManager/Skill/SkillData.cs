using GLip;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SkillData : MonoBehaviour
{
    public Model Model;
    string skillName_eng;
    public string skillName_kor;
    [TextArea]
    public string description;
    public Sprite icon;
    public bool isLearn;
    public float Length;
    public enum AttackType { Physic, Magic }
    public AttackType attackType;
    public float DamagePercentage;
    public float CoolDown;

    [SerializeField]
    private float nowCoolDown;
    
    public bool isCoolDown { get { return nowCoolDown != 0f; } }
    public HitBox hitBox;
    public bool shouldLookAtEffectSheet;
    public int EffectSheetNum;

    public SkillData parentSkillData;

    public Transform skillpulling;
    public int hitBoxNum;
    public Queue<HitBox> hitBoxes = new Queue<HitBox>();

    public Model targetModel;

    public ISkillMovement skillMovement;
    public bool IsReachedTarget 
    { 
        get 
        {
            var closeCollider = GPosition.GetClosedCollider(Model.transform, Length, 30f, hitBox.GetTargetModelTag);
            if (closeCollider != null)
            {
                targetModel = closeCollider.transform.GetComponent<Model>();
                return true;
            }
            else
                return false;
        } 
    }
    public void Awake()
    {
        skillName_eng = gameObject.name;
        skillpulling = new GameObject(skillName_eng).GetComponent<Transform>();
        for (int i = 0; i < hitBoxNum; i++)
        {
            SetHitBox();
        }
    }

    public void ActivateSkill()
    {
        StartCoroutine(StartCoolDown());
        skillMovement.StartMove();
    }

    protected virtual IEnumerator StartHitBoxMove() { yield return null; }

    IEnumerator StartCoolDown()
    {
        nowCoolDown = CoolDown;
        while(nowCoolDown >= 0f)
        {
            yield return new WaitForFixedUpdate();
            nowCoolDown -= Time.fixedDeltaTime;
        }
        nowCoolDown = 0f;
    }
    int setDamageCount;
    public void SetDamage(List<Collider> colliders)
    {
        foreach(Collider collider in colliders)
        {
            SetDamage(collider);
        }
    }
    public void SetDamage(Collider collider)
    {
        StateEffecterManager.EffectToModelBySkill(this, collider.GetComponent<Model>(), Model.ATK + (DamagePercentage * (attackType == AttackType.Physic ? Model.ATK : Model.MP * 10) * 0.01f));
    }

    public void SetHitBox()
    {
        var nowHitBox = Instantiate(hitBox.gameObject, skillpulling).GetComponent<HitBox>();
        nowHitBox.gameObject.SetActive(false);
        hitBoxes.Enqueue(nowHitBox);
    }
    public HitBox GetHitBox()
    {
        if(hitBoxes.Count == 0) { SetHitBox(); }
        var copy = hitBoxes.Dequeue();
        copy.gameObject.SetActive(true);
        copy.transform.forward = Model.transform.forward;
        copy.Collider.enabled = true;
        return copy;
    }

    public HitBox GetHitBoxWithOutSetUp()
    {
        if (hitBoxes.Count == 0) { SetHitBox(); }
        return hitBoxes.Dequeue();
    }
}
