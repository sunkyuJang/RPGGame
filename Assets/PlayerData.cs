using GLip;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class PlayerData
{
    public string ID { set; get; }
    public string NickName { set; get; }
    public Vector3 LastStandPosition { set; get; }
    public List<ItemManager.ItemCounter> InventoryItemList { set; get; }
        = new List<ItemManager.ItemCounter>();

    public List<ItemManager.ItemCounter> EquipmentItems { set; get; }
        = new List<ItemManager.ItemCounter>();

    public PlayerData(string id, string nickName)
    {
        ID = id;
        NickName = nickName;
    }

    public void CreateJsonFile()
    {
        var fileStream = new FileStream(string.Format("{0}/{1}.json", Application.dataPath, ID), FileMode.Create);
    }
}
