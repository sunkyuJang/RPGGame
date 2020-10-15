using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Services;
using System.Xml.Serialization;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance { set; get; }
    public QuestSheet questSheet;
    public static List<QuestSheet.Param> QuestList { private set; get; }

    public void Awake()
    {
        QuestList = questSheet.sheets[0].list;
    }

    public static bool NpcHasQuest(Npc npc, Character character)
    {
        foreach (QuestTable questTable in character.ProcessingQuestList)
            if (npc.CharacterName.Equals(questTable.data.Name))
                return true;
        
        for (int i = npc.lastDialog; i < npc.Dialogue.Count; i++)
            if (npc.Dialogue[i++].Type == "Quest")
                return true;

        return false;
    }
    public static QuestTable GetQuest(Npc npc, Character character)
    {
        QuestTable nowQuest = null;
        int processingIndex = 0;

        if (isAlreadyAccept(npc, character, out processingIndex))
            nowQuest = character.ProcessingQuestList[processingIndex];
        else
            nowQuest = new QuestTable(npc, npc.lastDialog);

        return nowQuest;
    }    
    
    public static QuestTable GetQuest(Npc npc, Character character, int processingIndex)
    {
        QuestTable nowQuest = null;

        if (processingIndex >= 0)
            nowQuest = character.ProcessingQuestList[processingIndex];
        else
            nowQuest = new QuestTable(npc, npc.lastDialog);

        return nowQuest;
    }

    public static List<QuestTable> LoadAllProgressQuestTable(List<int> questindexList, Character character)
    {
        List<QuestTable> questTables = new List<QuestTable>();
        for (int i = 0; i < questindexList.Count; i++)
        {
            var questTable = new QuestTable(QuestList[questindexList[i]]);
            AcceptQuest(questTable, character);
            questTables.Add(questTable);
        }
        return questTables;
    }
    public static bool isAlreadyAccept(Npc npc, Character character, out int processingIndex)
    {
        for (int i = 0; i < character.ProcessingQuestList.Count; i++) 
            if (character.ProcessingQuestList[i].data.Name.Equals(npc.CharacterName))
            {
                processingIndex = i;
                return true;
            }

        processingIndex = -1;
        return false;
    }
    public static void AcceptQuest(QuestTable quest, Character character) 
    {
        for(int start = 0, max = 2; start < max; start++)
        {
            var itemCountList = start == 0 ? quest.RequireList : quest.RewardList;
            
            for(int i = 0; i < itemCountList.Count; i++)
                ItemManager.Instance.GetNewItemView(itemCountList[i]);
        }
        
        character.ProcessingQuestList.Add(quest); 
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
        public QuestSheet.Param data { private set; get; }
        public List<ItemManager.ItemCounter> RequireList { set; get; }
        public List<ItemManager.ItemCounter> RewardList { set; get; }
        
        public QuestTable(Npc npc, int dialogueIndex)
        {
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

        public QuestTable(QuestSheet.Param data)
        {
            this.data = data;
            RequireList = GetItemList(data.NeedItem, data.NeedCount);
            RewardList = GetItemList(data.RewardItem, data.RewardCount);
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
                return true;
            }
            return false;
        }
    }
}
