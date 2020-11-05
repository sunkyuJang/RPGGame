﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    public string nextSceneName;
    public Vector3 nextScenePosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            var Character = other.GetComponent<Character>();
            Character.controller.ResetJoystick();
            LoadSceneManager.LoadScene(nextSceneName, other.transform.GetComponent<Character>(), nextScenePosition);
        }
    }
}