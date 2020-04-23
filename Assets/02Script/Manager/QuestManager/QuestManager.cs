﻿using System.Collections;
using System.Collections.Generic;
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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

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
            ItemManager.Kinds kinds = ItemManager.Kinds.Null;
            string[] temp = splitKinds[i].Split(',');
            switch (temp[0])
            {
                case "key": kinds = ItemManager.Kinds.keyItemList; break;
                case "active": kinds = ItemManager.Kinds.activeItemList; break;
            }
            ItemManager.ItemIndexer indexer = new ItemManager.ItemIndexer(kinds, int.Parse(temp[1]));
            itemList.Add(new ItemManager.ItemCounter(indexer, int.Parse(splitCounts[i])));
        }
        return itemList;
    }
    class QuestTable
    {
        public string Name { private set; get; }
        public int StartIndex { private set; get; }
        public int EndIndex { private set; get; }
        public int NowQuestIndex { set; get; }
        public List<ItemManager.ItemCounter> RequireList { set; get; }
        public List<ItemManager.ItemCounter> RewardList { set; get; }

        public QuestTable(string _name, int _startIndex, int _endIndex)
        {
            Name = _name; StartIndex = _startIndex; EndIndex = _endIndex; NowQuestIndex = 0; RequireList = new List<ItemManager.ItemCounter>();
        }
        public bool ComfareQuest(string _name, int _index) { return Name.Equals(_name) && (NowQuestIndex == _index); }
        public bool ComfareItemList(Inventory inventory) 
        {
            List<bool> IsClear = new List<bool>();
            foreach(ItemManager.ItemCounter requireItem in RequireList)
            {
                foreach(ItemManager.ItemCounter inventoryItem in inventory.ItemCounters)
                {
                    if(requireItem.Indexer.IsSame(inventoryItem.Indexer))
                    {
                        if (requireItem.Count <= inventoryItem.Count) { IsClear.Add(true); }
                        else return false;
                    }
                }
            }

            if(IsClear.Count == RequireList.Count)
            {
                foreach(ItemManager.ItemCounter requireItem in RequireList) { inventory.RemoveItem(requireItem); }
                foreach(ItemManager.ItemCounter rewardItem in RewardList) { inventory.AddItem(rewardItem); }
                return true;
            }
            return false;
        }
        public int GetQuestIndex() { return NowQuestIndex + StartIndex; }
        public bool IsUnresistered() { return RequireList.Count == 0; }
    }
}