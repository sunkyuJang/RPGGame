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
        ItemView itemView = Instantiate(ItemViewObj).GetComponent<ItemView>();
        itemView.SetItemCounter(itemCount, inventory);
        itemCount.View = itemView;
        return itemView;
    }
    public class ItemCounter
    {
        public ItemSheet.Param Data { private set; get; }
        public int count { private set; get; }
        public ItemCounter(ItemSheet.Param data) => Data = data;
        public ItemView View { set; get; }
        public int GetExcessCount(int addCount) 
        {
            var nowCount = count + addCount;

            if (nowCount > Data.Limit)
            {
                count = Data.Limit;
                nowCount -= Data.Limit;
            }
            else
            {
                count = nowCount;
                nowCount = 0;
            }

            ViewRefreash();
            return nowCount;
        }
        public int RemoveCountWithOverFlow(int removeCount)
        {
            count -= removeCount;

            ViewRefreash();
            return count;
        }

        void ViewRefreash()
        {
            if(View != null)
            {
                View.RefreshText();
            }
        }
    }
}