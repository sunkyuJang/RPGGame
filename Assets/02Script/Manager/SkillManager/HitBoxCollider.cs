using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UIElements;
using GLip;
using Unity.Collections;
using UnityEditor;
using System.Runtime.Remoting.Messaging;
using System.Runtime.CompilerServices;

public class HitBoxCollider : MonoBehaviour
{
    public List<Collider> colliders { private set; get; } = new List<Collider>();
    float speed;

    Transform HitBoxFXTransform { set; get; }
    SkillManager.Skill skill { set; get; }

    bool isAlive { set; get; } = true;
    private void FixedUpdate()
    {
        if (isAlive)
        {
            transform.position += transform.forward * (speed * Time.fixedDeltaTime);
            if (HitBoxFXTransform != null)
                HitBoxFXTransform.position += transform.forward * (speed * Time.fixedDeltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster") || other.CompareTag("Character")) colliders.Add(other);
    }
    public static HitBoxCollider StartHitBox(GameObject gameObject, Vector3 center, Vector3 direction, SkillManager.Skill skill)
    {
        HitBoxCollider hitBoxCollider = Instantiate(gameObject).GetComponent<HitBoxCollider>();
        hitBoxCollider.transform.position += new Vector3(center.x, 0, center.z);
        hitBoxCollider.transform.forward = direction;
        var duration = skill.data.Duration == 0f ? 0.1f : skill.data.Duration;
        hitBoxCollider.speed = (skill.data.Length + 1f) / duration;
        hitBoxCollider.skill = skill;

        if (skill.HitBoxFX != null)
        {
            hitBoxCollider.HitBoxFXTransform = skill.data.AttackType == "Line" ?
                Instantiate(skill.HitBoxFX).GetComponent<Transform>() : skill.HitBoxFX.GetComponent<Transform>();
            hitBoxCollider.HitBoxFXTransform.position = new Vector3(hitBoxCollider.transform.position.x, hitBoxCollider.HitBoxFXTransform.position.y, hitBoxCollider.transform.position.z);
        }

        return hitBoxCollider;
    }
    public Collider GetColsedCollider(Vector3 center, string targetTo) 
    {
        Collider beforeCollider = null;
        foreach(Collider nowCollider in colliders)
        {
            if (nowCollider.CompareTag(targetTo))
            {
                beforeCollider = beforeCollider == null ? nowCollider
                                : GPosition.IsAClosedThanBFromCenter(center, beforeCollider.transform.position, nowCollider.transform.position)
                                ? beforeCollider : nowCollider;
            }
        }

        return beforeCollider;
    }
    public bool IsEnteredTrigger { get { return colliders.Count > 0; } }

    public void DestroyObj()
    {
        isAlive = false;
        if (HitBoxFXTransform != null)
        {
            if (skill.data.AttackType == "Stamp")
                HitBoxFXTransform = Instantiate(HitBoxFXTransform).GetComponent<Transform>();

            StartCoroutine(DeleteParticleObj(HitBoxFXTransform.GetComponent<ParticleSystem>()));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DeleteParticleObj(ParticleSystem ps)
    {
        if(ps.main.loop) ps.Stop();
        while (ps.IsAlive())
        {
            yield return new WaitForFixedUpdate();
        }
        Destroy(ps.gameObject);
        Destroy(gameObject);
    }
}

/*    public static HitBoxCollider StartHitBox(GameObject gameObject, Vector3 startPosition, Vector3 , SkillManager.Skill skill)
    {
        HitBoxCollider hitBoxCollider = Instantiate(gameObject).GetComponent<HitBoxCollider>();
        hitBoxCollider.transform.position += new Vector3(character.position.x, 0, character.position.z);
        hitBoxCollider.transform.forward = character.forward;
        print(hitBoxCollider.speed);
        hitBoxCollider.skill = skill;

        if (skill.HitBoxFX != null)
        {
            hitBoxCollider.HitBoxFXTransform = skill.data.AttackType == "Line" ?
                Instantiate(skill.HitBoxFX).GetComponent<Transform>() : skill.HitBoxFX.GetComponent<Transform>();
            hitBoxCollider.HitBoxFXTransform.position = new Vector3(hitBoxCollider.transform.position.x, hitBoxCollider.HitBoxFXTransform.position.y, hitBoxCollider.transform.position.z);
        }

        return hitBoxCollider;
    }*/