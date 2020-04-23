﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ComfimBox : MonoBehaviour
{
    private RectTransform rectTransform;
    private Text script;
    public enum State { Yes, No, Waiting }
    public State NowState { private set; get; }
    public bool isPressed { set; get; }
    public bool isYes { set; get; }
    private void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();

        script = gameObject.transform.GetChild(0).GetComponent<Text>();
        isPressed = false;
        isYes = false;
        gameObject.SetActive(false);
    }
    public void ShowComfirmBox(string _script) 
    { 
        script.text = _script; 
        gameObject.SetActive(true);
        rectTransform.SetAsLastSibling();
        NowState = State.Waiting;
    }

    public void PreseedYes() { NowState = State.Yes; gameObject.SetActive(false); }
    public void PressedNo() { NowState = State.No; gameObject.SetActive(false); }

    public static ComfimBox GetNew { get { return Instantiate(Resources.Load<GameObject>("ComfirmBox"), GameObject.Find("Canvas").transform).GetComponent<ComfimBox>(); } }
}