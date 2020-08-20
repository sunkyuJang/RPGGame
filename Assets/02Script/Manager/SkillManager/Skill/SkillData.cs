using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SkillData : MonoBehaviour
{
    public Transform Model;
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
        if (!isCoolDown)
        {
            StartCoroutine(StartCoolDown());
            StartCoroutine(StartHitBoxMove());
        }
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

    protected void SetDamage(List<Collider> colliders)
    {
        foreach(Collider collider in colliders)
        {
            StateEffecterManager.EffectToModelBySkill(this, collider.GetComponent<Model>(), DamagePercentage);
        }
    }

    protected void SetHitBox()
    {
        var nowHitBox = Instantiate(hitBox.gameObject, skillpulling).GetComponent<HitBox>();
        nowHitBox.gameObject.SetActive(false);
        hitBoxes.Enqueue(nowHitBox);
    }
    protected HitBox GetHitBox()
    {
        if(hitBoxes.Count == 0) { SetHitBox(); }
        var copy = hitBoxes.Dequeue();
        copy.gameObject.SetActive(true);
        copy.transform.forward = Model.forward;
        return copy;
    }
}
