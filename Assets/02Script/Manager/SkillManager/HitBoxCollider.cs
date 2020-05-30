using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UIElements;
using GLip;
using Unity.Collections;
using UnityEditor;
using System.Runtime.Remoting.Messaging;

public class HitBoxCollider : MonoBehaviour
{
    public List<Collider> colliders { private set; get; } = new List<Collider>();
    Vector3 centerForward;
    float speed;
    private void FixedUpdate()
    {
        transform.position += centerForward * (speed * Time.fixedDeltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster") colliders.Add(other);
    }

    public void MoveHitBox()
    {

    }
    public static HitBoxCollider StartHitBox(GameObject gameObject, Transform center, float speed)
    {
        HitBoxCollider hitBoxCollider = Instantiate(gameObject).GetComponent<HitBoxCollider>();
        Debug.Log("isIn");
        hitBoxCollider.transform.forward = center.forward;
        hitBoxCollider.speed = speed;
        return hitBoxCollider;
    }
    public Collider GetColsedCollider(Vector3 center) 
    {
        Collider beforeCollider = null;
        foreach(Collider nowCollider in colliders)
        {
            beforeCollider = beforeCollider == null ? nowCollider
                            : GPosition.IsAClosedThanBFromCenter(center, beforeCollider.transform.position, nowCollider.transform.position)
                            ? beforeCollider : nowCollider;
        }
        return beforeCollider;
    }
    public bool IsEnteredTrigger { get { return colliders.Count > 0; } }
}
