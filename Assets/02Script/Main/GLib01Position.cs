using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace GLip
{
    class GPosition
    {
        public static Vector2 nonPosition { get { return new Vector2(-5, -5); } }
        public static Vector2 GetItemViewRect { get { return new Vector2(100, 100); } }
        public static Vector2 InventoryViewPosition { get { return new Vector2(331, 0); } }

        public static List<Collider> GetSelectedColliderInFOV(Transform centerTransform, float castRad, float castFOVRad, string targetTag)
        {
            var list = GetNearByObj(centerTransform.position, castRad);
            return SelectColliderInFOV(list, targetTag, centerTransform, castFOVRad);
        }
        public static List<Collider> GetNearByObj(Vector3 startPosition, float castRad)
        {
            return Physics.OverlapSphere(startPosition, castRad, (int)GGameInfo.LayerMasksList.Floor, QueryTriggerInteraction.Ignore).ToList<Collider>();
        }
        public static List<Collider> SelectColliderInFOV(List<Collider> colliders, string targetTag, Transform centerTransform, float castFOVRad)
        {
            List<Collider> newList = new List<Collider>();
            foreach(Collider nowCollider in colliders)
            {
                if (nowCollider.CompareTag(targetTag))
                {
                    Vector3 directionFromCenterToCollider = nowCollider.transform.position - centerTransform.position;
                    float angleFromCenterToCollider = Vector3.Angle(centerTransform.forward, directionFromCenterToCollider);
                    if (angleFromCenterToCollider <= castFOVRad)
                    {
                        newList.Add(nowCollider);
                    }
                }
            }
            return newList;
        }
        public static bool IsAClosedThanBFromCenter(Vector3 center, Vector3 positionA, Vector3 positionB) 
        {
            return Vector3.Distance(center, positionA) < Vector3.Distance(center, positionB); 
        }
    }
}
