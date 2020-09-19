using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public Controller controller;
    public CinemachineVirtualCamera followCamera { private set; get; }
    public void Awake()
    {
        instance = this;
        followCamera = transform.GetChild(0).GetComponent<CinemachineVirtualCamera>();
    }
    private void FixedUpdate()
    {
        transform.position = character.GetComponent<Transform>().position;
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
