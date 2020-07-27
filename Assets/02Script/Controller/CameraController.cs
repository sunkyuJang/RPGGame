using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public GameObject character;
    public CinemachineVirtualCamera followCamera { private set; get; }
    public void Awake()
    {
        followCamera = transform.Find("CMFollowCam").GetComponent<CinemachineVirtualCamera>();
    }
    private void FixedUpdate()
    {
        transform.position = character.GetComponent<Transform>().position;
    }

    public void RotateCamera(float radian)
    {
        transform.Rotate(Vector3.up, radian);
    }
}
