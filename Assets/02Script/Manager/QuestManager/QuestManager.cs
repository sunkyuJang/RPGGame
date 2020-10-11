using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Xml.Serialization;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public QuestSheet questSheet;
    public static List<QuestSheet.Param> QuestList { private set; get; }
    private static List<QuestTable> QuestTableList { set; get; }
    private static List<QuestTable> ProcessingQuestList { set; get; }
    void Awake()
    {
        QuestTableList = new List<QuestTable>();
        ProcessingQuestList = new List<QuestTable>();
    }
    public static QuestTable GetQuest(Npc npc)
    {
        QuestTable nowQuest = null;
        if (npc.HasQuest)
        {
            int processingIndex = 0;

            if (isAlreadyAccept(npc, out processingIndex))
                nowQuest = ProcessingQuestList[processingIndex];
            else
                nowQuest = new QuestTable(npc, npc.lastDialog);
        }

        return nowQuest;
    }
    public static bool isAlreadyAccept(Npc npc, out int processingIndex)
    {
        if (ProcessingQuestList != null)
        {
            for (int i = 0; i < ProcessingQuestList.Count; i++)
            {
                var processingQuest = ProcessingQuestList[i];
                if(processingQuest.npc == npc)
                {
                    processingIndex = i;
                    return true;
                }
            }
        }
        processingIndex = 0;
        return false;
    }
    public static void AcceptQuest(QuestTable quest) 
    {
        quest.isAccept = true;
        ProcessingQuestList.Add(quest); 
    }
    
    public static bool CanClearQuest(Inventory requestPlayerInventory, QuestTable quest)
    {
        if (quest.ComfareItemList(requestPlayerInventory))
            return true;

        return false;
    }

    private static List<ItemManager.ItemCounter> GetItemList(string requireItem, string itemCount)
    {
        List<ItemManager.ItemCounter> itemList = new List<ItemManager.ItemCounter>();
        string[] splitKinds = requireItem.Split(';');
        string[] splitCounts = itemCount.Split(';');
        for (int i = 0; i < splitKinds.Length -1; i++)
        {
            var counter = new ItemManager.ItemCounter(ItemManager.Instance.GetitemData(int.Parse(splitKinds[i])));
            counter.GetExcessCount(int.Parse(splitCounts[i]));
            itemList.Add(counter);
        }
        return itemList;
    }

    public class QuestTable
    {
        public Npc npc { private set; get; }
        public QuestSheet.Param data { private set; get; }
        public int Index { private set; get; }
        public bool isAccept { set; get; } = false;
        public List<ItemManager.ItemCounter> RequireList { set; get; }
        public List<ItemManager.ItemCounter> RewardList { set; get; }

        public QuestTable(Npc npc, int dialogueIndex)
        {
            this.npc = npc;
            data = GetMatchedQuestData(npc, dialogueIndex);
            if (data != null)
            {
                RequireList = GetItemList(data.NeedItem, data.NeedCount);
                RewardList = GetItemList(data.RewardItem, data.RewardCount);
            }
            else
            {
                print("Something worng in QuestTable");
            }
        }

        QuestSheet.Param GetMatchedQuestData(Npc npc, int dialogueIndex)
        {
            foreach (QuestSheet.Param data in npc.questSheet.sheets[0].list)
            {
                if (data.DialogueIndex == dialogueIndex)
                {
                    return data;
                }
            }
            return null;
        }
        public bool ComfareItemList(Inventory inventory) 
        {
            int clearCount = 0;
            foreach(ItemManager.ItemCounter requireItem in RequireList)
            {
                if (inventory.table.GetSameKindTotalCount(requireItem.Data) >= requireItem.count)
                {
                    clearCount++;
                }
                else
                {
                    return false;
                }
            }

            if(clearCount == RequireList.Count)
            {
                foreach(ItemManager.ItemCounter requireItem in RequireList) { inventory.RemoveItem(requireItem); }
                foreach(ItemManager.ItemCounter rewardItem in RewardList) { inventory.AddItem(rewardItem); }
                npc.ClearQuestCount++;
                return true;
            }
            return false;
        }
    }
}
