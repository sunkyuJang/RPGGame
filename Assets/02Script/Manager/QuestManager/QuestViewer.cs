using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestViewer : MonoBehaviour
{
    public static QuestViewer instance { set; get; }
    public List<QuestManager.QuestTable> processingQuestList { set; get; } = new List<QuestManager.QuestTable>();
    public GameObject viewerFrame;
    public Transform questProcessListTransform;
    public GameObject questNamePrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
    }

    public void AddQuest(QuestManager.QuestTable questTable)
    {
        var copy = Instantiate(questNamePrefab, questProcessListTransform);
    }


}
