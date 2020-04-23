using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;

public class TouchManager : MonoBehaviour
{
    public static Vector2 GetTouch(RectTransform _rectTransform)
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touches.Length; i++)
            {
                if (IsTouched(_rectTransform, Input.GetTouch(i).position)) { return Input.GetTouch(i).position; }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (IsTouched(_rectTransform, Input.mousePosition)) { return Input.mousePosition; }
        }
        return GPosition.nonPosition;
    }
    private static bool IsTouched(RectTransform _rectTransform, Vector2 touchPosition)
    {
        Rect rect = GMath.GetRect(_rectTransform);
        return rect.Contains(touchPosition);
    }
}