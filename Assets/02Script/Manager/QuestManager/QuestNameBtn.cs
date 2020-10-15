using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestNameBtn : MonoBehaviour
{
    public QuestManager.QuestTable questTable { set; get; }
    public Text QuestTitle;
    public Transform requireItemGroup;
    public Transform rewardItemGroup;

    public void SetQuestBtn(QuestManager.QuestTable questTable)
    {
        this.questTable = questTable;
        QuestTitle.text = questTable.data.Title;
        for (int i = 0; i < 2; i++) {
            var nowList = i == 0 ? questTable.RequireList : questTable.RewardList;
            
            foreach (ItemManager.ItemCounter itemCount in nowList)
                itemCount.View.transform.SetParent(i == 0 ? requireItemGroup : rewardItemGroup);
        }
    }
}
