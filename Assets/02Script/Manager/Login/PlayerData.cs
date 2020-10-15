using GLip;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.WSA.Input;

public class PlayerData
{
    public string id;
    public string pw;
    public string NickName;

    public static string path = Application.dataPath + "/Resources/Managers/SaveData/";
    public string GetJsonPathWithAcc { get { return PlayerData.path + id + ".json"; } }

    public bool isFirstStart;
    public string LastScene;
    public Vector3 LastPosition;
    
    public int level;
    
    //item
    public List<int> itemKinds = new List<int>();
    public List<int> itemCounts = new List<int>();
    
    //Equipment
    public List<int> WearingItem;
    
    //skills
    public List<string> skillNames = new List<string>();
    public List<bool> isLearnSkill = new List<bool>();

    //TalkingWith
    public List<string> namesTalkingwith = new List<string>();
    public List<int> lastTalkingIndex = new List<int>();

    //QuestList
    public List<int> QuestIndexList = new List<int>();
    PlayerData() { }
    public PlayerData(string id, string pw, string nickName)
    {
        this.id = id;
        this.pw = pw;
        NickName = nickName;
        isFirstStart = true;
        LastPosition = Vector3.zero;
        LastScene = "IngameScene";
    }
    public void SetPlayerDataFromCharacter(Character character)
    {
        isFirstStart = false;
        LastScene = SceneManager.GetActiveScene().name;
        LastPosition = character.transform.position;
        level = character.level;

        itemKinds.Clear();
        itemCounts.Clear();
        var inventory = character.Inventory;
        foreach (ItemView view in inventory.itemViews)
        {
            itemKinds.Add(view.ItemCounter.Data.Index);
            itemCounts.Add(view.ItemCounter.count);
        }

        WearingItem.Clear();
        var equipmentView = character.EquipmentView;
        if (equipmentView.IsWearing)
        {
            foreach (ItemManager.ItemCounter item in equipmentView.EquipmentItems)
            {
                itemKinds.Add(item.Data.Index);
                itemCounts.Add(item.count);
            }
        }

        skillNames.Clear();
        isLearnSkill.Clear();
        var skillLists = character.skillListHandler.skillDatas;
        foreach (SkillData skillData in skillLists)
        {
            skillNames.Add(skillData.skillName_eng);
            isLearnSkill.Add(skillData.isLearn);
        }

        namesTalkingwith.Clear();
        lastTalkingIndex.Clear();
        var talkingWith = character.LastTimeTalkingWith;
        if (talkingWith.Count > 0)
            foreach (KeyValuePair<string, int> model in talkingWith)
            {
                namesTalkingwith.Add(model.Key);
                lastTalkingIndex.Add(model.Value);
            }

        QuestIndexList.Clear();
        var questIndexList = character.ProcessingQuestList;
        if(questIndexList.Count > 0)
        {
            foreach (QuestManager.QuestTable questTable in questIndexList)
                QuestIndexList.Add(questTable.data.Index);
        }
    }

    /*public static PlayerData ConvertChatacterToPlayerData(Character character)
    {
        var playerData = new PlayerData();
        playerData.isFirstStart = false;
        playerData.LastScene = SceneManager.GetActiveScene().name;
        playerData.LastPosition = character.transform.position;
        playerData.level = character.level;

        var inventory = character.Inventory;
        foreach (ItemView view in inventory.itemViews)
        {
            playerData.itemKinds.Add(view.ItemCounter.Data.Index);
            playerData.itemCounts.Add(view.ItemCounter.count);
        }

        var equipmentItems = character.EquipmentView.EquipmentItems;
        foreach (ItemManager.ItemCounter item in equipmentItems)
        {
            playerData.itemKinds.Add(item.Data.Index);
            playerData.itemCounts.Add(item.count);
        }

        var skillLists = character.skillListHandler.skillDatas;
        foreach(SkillData skillData in skillLists)
        {
            playerData.skillNames.Add(skillData.skillName_eng);
            playerData.isLearnSkill.Add(skillData.isLearn);
        }

  

        return playerData;
    }*/
}
