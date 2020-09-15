﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UIElements;

namespace GLip
{
    class GPosition
    {
        public static Vector2 nonPosition { get { return new Vector2(-5, -5); } }
        public static Collider GetClosedCollider(Transform centerTransform, float castRad, float castFOVRad, string targetTag)
        {
            var list = GetSelectedColliderInFOV(centerTransform, castRad, castFOVRad, targetTag);
            var dist = 10000f;
            Collider closeOne = null;
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var nowCollider = list[i];
                    var nowDist = Vector3.Distance(centerTransform.position, nowCollider.transform.position);
                    if(nowDist < dist)
                    {
                        dist = nowDist;
                        closeOne = nowCollider;
                    }
                }
            }
            return closeOne;
        }

        public static List<Collider> GetAllCollidersInFOV(Transform centerTransform, float castRad, float castFOVRad)
        {
            var list = GetNearByObj(centerTransform.position, castRad);
            List<Collider> newList = new List<Collider>();
            foreach (Collider nowCollider in list)
            {
                    Vector3 directionFromCenterToCollider = nowCollider.transform.position - centerTransform.position;
                    float angleFromCenterToCollider = Vector3.Angle(centerTransform.forward, directionFromCenterToCollider);
                    if (angleFromCenterToCollider <= castFOVRad)
                    {
                        newList.Add(nowCollider);
                    }
            }
            return newList;
        }

        public static List<Collider> GetSelectedColliderInFOV(Transform centerTransform, float castRad, float castFOVRad, string targetTag)
        {
            var list = GetNearByObj(centerTransform.position, castRad);
            return SelectColliderInFOV(list, targetTag, centerTransform, castFOVRad);
        }
        public static List<Collider> GetNearByObj(Vector3 startPosition, float castRad)
        {
            return Physics.OverlapSphere(startPosition, castRad).ToList<Collider>();
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

        public static bool IsContainTouch(RectTransform rectTransform, out int index)
        {
            index = 0;
            Rect rect = GMath.GetRect(rectTransform);
            for(int i = 0; i < Input.touches.Length; i++)
            {
                if (rect.Contains(Input.touches[i].position))
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }
        public static bool IsContainTouch(Rect rect, out int index)
        {
            index = 0;
            for(int i = 0; i < Input.touches.Length; i++)
            {
                if (rect.Contains(Input.touches[i].position))
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }
        public static bool IsContainMousePosition(RectTransform rectTransform)
        {
            Rect rect = GMath.GetRect(rectTransform);
            return rect.Contains(Input.mousePosition);
        } 
        public static bool IsContainMousePosition(Rect rect)
        {
            return rect.Contains(Input.mousePosition);
        }
        public static bool IsTouchStillPressed(int touchId)
        {
            return Input.GetTouch(touchId).phase != TouchPhase.Ended;
        }

        public static bool IsMouseStillPressed()
        {
            return Input.GetKey(KeyCode.Mouse0);
        }

        public static bool IsContainInput(RectTransform rectTransform, out bool isTouch, out int touchId, out bool isMouse)
        {
            isTouch = false;
            touchId = 0;
            isMouse = false;

            if(IsContainTouch(rectTransform, out touchId)) 
            {
                isTouch = true;
                return true; 
            }
            else if (IsContainMousePosition(rectTransform))
            {
                isMouse = true;
                return true;
            }
            return false;
        }
        public static bool IsContainInput(Rect rect, out bool isTouch, out int touchId, out bool isMouse)
        {
            isTouch = false;
            touchId = 0;
            isMouse = false;

            if (IsContainTouch(rect, out touchId))
            {
                isTouch = true;
                return true;
            }
            else if (IsContainMousePosition(rect))
            {
                isMouse = true;
                return true;
            }
            return false;
        }
        public static bool IsContainInput(Rect rect, bool isTouch, int touchId, bool isMouse)
        {
            isTouch = false;
            touchId = 0;
            isMouse = false;

            if (IsContainTouch(rect, out touchId))
            {
                return true;
            }
            else if (IsContainMousePosition(rect))
            {
                return true;
            }
            return false;
        }

        public static Vector2 GetInputPosition(bool isTouch, int touchId, bool isMouse)
        {
            if (isTouch) return Input.touches[touchId].position;
            else if (isMouse) return Input.mousePosition;
            else return Vector2.zero;
        }

        public static bool IsHoldPressedInput(bool isTouched, int touchID, bool isMouse)
        {
            if (isTouched) return IsTouchStillPressed(touchID);
            else if (isMouse) return IsMouseStillPressed();
            else { return false; }
        }
        public static Vector3 ReverceLeft(Vector3 vector3) { return new Vector3(vector3.x * -1, vector3.y, vector3.z); }
        public static Vector3 ReverceUp(Vector3 vector3) { return new Vector3(vector3.x, vector3.y * -1, vector3.z); }
        public static Vector3 ReverceFront(Vector3 vector3) { return new Vector3(vector3.x, vector3.y, vector3.z * -1); }
    }

}