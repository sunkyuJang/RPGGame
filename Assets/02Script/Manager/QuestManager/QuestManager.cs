using System.Collections;
using System.Collections.Generic;
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
        QuestList = questSheet.sheets[0].list;
        QuestTableList = new List<QuestTable>();
        ProcessingQuestList = new List<QuestTable>();
        LoadQuestList();
    }

    private void LoadQuestList()
    {
        var name = QuestList[0].name;
        var startIndex = 0;

        for (int i = 1; i < QuestList.Count; i++)
        {
            if (name != QuestList[i].name)
            {
                QuestTableList.Add(new QuestTable(name, startIndex, i - 1));
                name = QuestList[i].name;
                startIndex = i;
            }
        }

        //add last line
        QuestTableList.Add(new QuestTable(name, startIndex, QuestList.Count - 1));
    }

    public static QuestTable GetLastQuest(Npc npc)
    {
        QuestTable nowQuest = null;
        if (npc.HasQuest)
        {
            return new QuestTable(npc, npc.lastQuestNum);
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
                if (processingQuest.ComfareQuest(npc, npc.lastQuestNum))
                {
                    processingIndex = i;
                    return true;
                }
            }
        }
        processingIndex = 0;
        return false;
    }
    public static void AcceptQuest(QuestTable quest) => ProcessingQuestList.Add(quest);
    public static QuestTable GetRunningQuest(Npc npc, int processingIndex) { return processingIndex}
    
    public static bool CanClearQuest(Inventory inventory, string npcName, int index)
    {
        QuestTable nowTable = null;
        foreach(QuestTable table in ProcessingQuestList)
        {
            if (table.ComfareQuest(npcName, index))
            {
                nowTable = table;
            }
        }
        if (nowTable == null)
        {
            foreach (QuestTable table in QuestTableList)
            {
                if (table.ComfareQuest(npcName, index))
                {
                    table.NowQuestIndex = index;
                    QuestSheet.Param nowQuest = QuestList[table.GetQuestIndex()];
                    table.RequireList = GetItemList(nowQuest.needItem, nowQuest.needCount);
                    table.RewardList = GetItemList(nowQuest.rewardItem, nowQuest.rewardCount);
                    ProcessingQuestList.Add(table);
                    nowTable = table;
                }
            }
        }

        if(nowTable != null)
        {
            return nowTable.ComfareItemList(inventory);
        }
        return false;
    }

    private static List<ItemManager.ItemCounter> GetItemList(string requireItem, string itemCount)
    {
        List<ItemManager.ItemCounter> itemList = new List<ItemManager.ItemCounter>();
        string[] splitKinds = requireItem.Split(';');
        string[] splitCounts = itemCount.Split(';');
        for (int i = 0; i < splitKinds.Length -1; i++)
        {
            var counter = new ItemManager.ItemCounter(ItemManager.GetitemData(int.Parse(splitKinds[i])));
            counter.GetExcessCount(int.Parse(splitCounts[i]));
            itemList.Add(counter);
        }
        return itemList;
    }
    public class QuestTable
    {
        public Npc npc { private set; get; }
        public int Index { private set; get; }
        public int NowQuestIndex { set; get; }
        public List<ItemManager.ItemCounter> RequireList { set; get; }
        public List<ItemManager.ItemCounter> RewardList { set; get; }

        public QuestTable(Npc npc, int index)
        {
            this.npc = npc; 
            Index = index;
            var data = npc.questList.sheets[0].list;
            RequireList = GetItemList(data[index].needItem, data[index].needCount);
            RewardList = GetItemList(data[index].rewardItem, data[index].rewardCount);
        }
        public bool ComfareQuest(Npc npc, int _index) { return this.npc.Equals(npc) && (NowQuestIndex == _index); }
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
                return true;
            }
            return false;
        }
        public bool IsUnresistered() { return RequireList.Count == 0; }
    }
}
