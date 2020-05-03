using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject cameraKeyObj;
    private RectTransform cameraKeyRectTransform;
    public Vector2 centerPoint = Vector2.zero;
    private static Vector3 FirstCameraPosition { get { return new Vector3(0, 5, -5); } }
    private float firstRadian = Mathf.Atan2(FirstCameraPosition.z, FirstCameraPosition.x);
    private float lastRadian = Mathf.Atan2(FirstCameraPosition.z, FirstCameraPosition.x);
    public float Radian { 
        get 
        {
            float radian = firstRadian - lastRadian;
            return radian < (Mathf.PI * -1) ? radian + Mathf.PI * 2 : radian;
        } 
    }
    private Vector3 FirstRotation { get { return new Vector3(30, 0, 0); } }
    public Vector3 cameraPosition = Vector3.zero;
    private void Awake()
    {
        cameraKeyRectTransform = cameraKeyObj.GetComponent<RectTransform>();
        cameraPosition = FirstCameraPosition;
        transform.rotation = Quaternion.Euler(FirstRotation);
    }

    public void Follow(Vector3 position)
    {
        transform.position = position + cameraPosition;
    }
    public void SetCenterTouch()
    {
        centerPoint = TouchManager.GetTouch(cameraKeyRectTransform);
    }
    public void SetCamera(Vector3 position)
    {
        Vector2 touch = TouchManager.GetTouch(cameraKeyRectTransform);
        float moveWay = centerPoint.x - touch.x;
        float moveSpeed = 5f;
        if(Mathf.Abs(moveWay) > 10f)
        {
            if(moveWay < 0) { transform.RotateAround(position, Vector2.up, moveSpeed); }
            else { transform.RotateAround(position, Vector2.down, moveSpeed); }
        }
        cameraPosition = transform.position - position;
        lastRadian = Mathf.Atan2(cameraPosition.z, cameraPosition.x);
    }
}
