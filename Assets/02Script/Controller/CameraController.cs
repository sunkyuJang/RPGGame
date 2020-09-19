using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera followCamera { private set; get; }
    public void Awake()
    {
        followCamera = transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
    }

    public void RotateCamera(float radian)
    {
        transform.Rotate(Vector3.up, radian);
    }

    public float GetRadianFromFront()
    {
         return Mathf.Deg2Rad * transform.eulerAngles.y;
    }
}
