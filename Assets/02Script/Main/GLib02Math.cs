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

        public static Rect GetRect(Vector2 startPoint, Vector2 endPoint, Vector2 centerPosition)
        {
            startPoint= centerPosition - startPoint;
            endPoint = centerPosition - endPoint;
            return new Rect(startPoint, endPoint);
        }

        public static bool IsInArea(List<Vector2> points, Rect area, ref Vector2 point)
        {
            for(int i = 0; i < points.Count; i++)
            {
                if (area.Contains(points[i])){ point = points[i]; points.RemoveAt(i); return true; }
            }
            return false;
        }

        public static Vector2 ConvertV3xzToV2(Vector3 vector3) { return new Vector2(vector3.x, vector3.z); }
        public static Vector3 ConvertV2ToV3xz(Vector2 vector2) { return new Vector3(vector2.x, 0f, vector2.y); }
    }
}
