using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class QuestViewer : MonoBehaviour
{
    public Character Character { set; get; }
    public GameObject viewerFrame;
    public Transform questProcessListTransform;
    List<QuestNameBtn> questNameBtns { set; get; } = new List<QuestNameBtn>();

    public QuestViewerDescriptionBox descriptionBox;

    private void Awake()
    {
        foreach (Transform nowTransform in questProcessListTransform)
            questNameBtns.Add(nowTransform.GetComponent<QuestNameBtn>());
    }

    private void Start()
    {
        HideQuestList();
    }

    public void ShowQuestList(List<QuestManager.QuestTable> questTables) 
    {
        gameObject.SetActive(true);
        if (questTables.Count > 0)
        {
            for(int i = 0; i < questTables.Count; i++)
            {
                var nowQuest = questTables[i];
                var viewBtn = questNameBtns[i];
                viewBtn.SetQuestBtn(nowQuest);
                viewBtn.gameObject.SetActive(true);
            }
        }
    }

    public void HideQuestList()
    {
        descriptionBox.HideDescription();
        gameObject.SetActive(false);
        Character.IntoNormalUI();
    }

    public void PressecNameBtn(int num)
    {
        try
        {
            descriptionBox.ShowDescription(questNameBtns[num]);
        }
        catch
        {
            print("something Worng in questVIewer");
        }
    }
}