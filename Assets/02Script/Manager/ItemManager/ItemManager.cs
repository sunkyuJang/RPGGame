using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using JetBrains.Annotations;
using GLip;
using UnityEditor;
using System.Runtime.Remoting.Messaging;

public partial class ItemManager : MonoBehaviour
{
    public ItemSheet sheet;
    public static List<ItemSheet.Param> Data { set; get; }
    public static GameObject ItemViewObj { set; get; }

    private void Awake()
    {
        Data = sheet.sheets[0].list;
        ItemViewObj = Resources.Load<GameObject>("ItemView");
    }

    public static ItemSheet.Param GetitemData(int index) => Data[index];
    public static ItemView GetNewItemView(ItemCounter itemCount, Inventory inventory)
    {
        ItemView itemView = Instantiate(ItemViewObj, inventory.transform).GetComponent<ItemView>();
        itemView.ItemCounter = itemCount;
        itemView.inventory = inventory;
        List<EventTrigger.Entry> entry = itemView.GetComponent<EventTrigger>().triggers;
        entry[0].callback.AddListener((data) => { itemView.SelectedIcon(); });
        itemCount.View = itemView;
        return itemView;
    }
    public class ItemCounter
    {
        public ItemSheet.Param Data { private set; get; }
        public int count { private set; get; }
        public ItemCounter(ItemSheet.Param data) => Data = data;
        public ItemView View { set; get; }
        public bool AddCount(int addCount, out int overNum) 
        {
            count += addCount;
            if(count > Data.Limit)
            {
                overNum = count - Data.Limit;
                count = Data.Limit;
                return true;
            }
            overNum = 0;
            return false;
        }
        public int RemoveCountWithOverFlow(int removeCount)
        {
            return (count -= removeCount) * -1;
        }
    }
}