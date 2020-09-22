using GLip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class PlayerData
{
    public string id;
    public string pw;
    public string NickName;

    public static string path = Application.dataPath + "/Resources/Managers/SaveData/";
    public string GetJsonPathWithAcc { get { return PlayerData.path + id + ".json"; } }

    public bool isFirstStart;
    public Vector3 LastPosition;
    Dictionary<int, int> inventoryItem = new Dictionary<int, int>();
    public PlayerData(string id, string pw, string nickName)
    {
        this.id = id;
        this.pw = pw;
        NickName = nickName;
        isFirstStart = true;
        LastPosition = Vector3.zero;
    }
}
