using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UIElements;

public class HitBoxCollider : MonoBehaviour
{
    public List<Collider> colliders { private set; get; } = new List<Collider>();
    float Speed { set; get; }
    bool isSingleTarget;
    Vector3 centerPosition;
    private void FixedUpdate()
    {
        if (isSingleTarget)
            if (colliders.Count >= 1)
            {
                foreach (Collider collider in colliders)
                {

                }
            }

        transform.position += transform.forward * (Speed * Time.fixedDeltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster") colliders.Add(other);
    }
    private void OnTriggerExit(Collider other)
    {
        colliders.Remove(other);
    }

    public void SetStartPosition(Vector3 centerPosition, Vector3 startPosition, float speed, bool isSingleTarget)
    {
        this.centerPosition = centerPosition;
        transform.position = startPosition;
        Speed = speed;
        this.isSingleTarget = isSingleTarget;   
    } 
    
    public void MoveHitBox()
    {

    }
}
