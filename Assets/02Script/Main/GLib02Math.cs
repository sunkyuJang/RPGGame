using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GLip
{
    class GMath
    {
        public static Rect GetRect(RectTransform rectTransform)
        {
            Vector2 now = new Vector2();
            now.x = rectTransform.position.x + rectTransform.rect.x;
            now.y = rectTransform.position.y + rectTransform.rect.y;
            return new Rect(now.x, now.y, rectTransform.rect.width, rectTransform.rect.height);
        }

        public static Rect GetRect(Vector2 centerPosition, Vector2 size)
        {
            Rect rect = new Rect(Vector2.zero, size);
            rect.center = centerPosition;
            return rect;
        }

        public static void PrintRectArea(Rect rect)
        {
            Debug.Log(rect.xMin + "//" + rect.yMin + "//" + rect.xMax + "//" + rect.yMax);
        }
        public static bool IsInArea(List<Vector2> points, Rect area, ref Vector2 point)
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (area.Contains(points[i])) { point = points[i]; points.RemoveAt(i); return true; }
            }
            return false;
        }

        public static float Get360DegToDeg180(float radius360)
        {
            return radius360 <= 180 ? radius360 : radius360 - 360;
        }
        public static float Get360DegToRad(float radius360)
        {
            return Get360DegToDeg180(radius360) * Mathf.Deg2Rad;
        }
        public static Vector2[] MoveToRad(float centerRadian, float moveToRadian, float dist)
        {
            Vector2[] vectors = new Vector2[2];
            for (int i = 0; i < 2; i++)
            {
                moveToRadian *= -1;
                vectors[i].x = dist * Mathf.Sin(moveToRadian + centerRadian);
                vectors[i].y = dist * Mathf.Cos(moveToRadian + centerRadian);
            }
            return vectors;
        }
        public static Vector2 DegreeToVector2(float rad)
        {
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }

        public static Vector2 ConvertV3xzToV2(Vector3 vector3) { return new Vector2(vector3.x, vector3.z); }
        public static Vector3 ConvertV2ToV3xz(Vector2 vector2) { return new Vector3(vector2.x, 0f, vector2.y); }
    }
}
