using GLip;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public List<int> itemKinds = new List<int>();
    public List<int> itemCounts = new List<int>();

    public List<int> WearingItem;
    
    public int level;
    public PlayerData(string id, string pw, string nickName)
    {
        this.id = id;
        this.pw = pw;
        NickName = nickName;
        isFirstStart = true;
        LastPosition = Vector3.zero;
        LastScene = GameManager.pathOfScenes + "IngameScene";

        itemKinds.Add(0);
        itemCounts.Add(10);
        itemKinds.Add(1);
        itemCounts.Add(5);
    }
}
