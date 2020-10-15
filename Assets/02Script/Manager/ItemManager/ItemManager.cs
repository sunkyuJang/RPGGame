using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;
using UnityEngine.PlayerLoop;

public partial class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { set; get; }
    public ItemSheet sheet;
    public List<ItemSheet.Param> Data { set; get; }

    public GameObject ItemViewPrefab;
    public ObjPooler ItemViewerPooler { set; get; }
    public bool IsItemViewPoolerReady { get { return ItemViewerPooler != null; } }

    public GameObject itemDescriptionPrefab;
    public ItemDescriptionBox ItemDescriptionBox { set; get; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Data = sheet.sheets[0].list;
            ItemDescriptionBox = Instantiate(itemDescriptionPrefab, transform.root).GetComponent<ItemDescriptionBox>();
        }
        else 
            Destroy(gameObject);
    }

    private void Start()
    {
        ItemViewerPooler = ObjPoolerManager.instance.ReqeuestObjPooler(ItemViewPrefab);
        GPosition.GetRectTransformWithReset(gameObject.GetComponent<RectTransform>(), ItemViewerPooler.gameObject.AddComponent<RectTransform>());
    }

    public ItemSheet.Param GetitemData(int index) => Data[index];

    public ItemView GetNewItemView(ItemCounter itemCounter)
    {
        var nowItemView = ItemViewerPooler.GetObj<ItemView>();
        return nowItemView.SetItemCounter(itemCounter, null);
    }
    public ItemView GetNewItemView(ItemCounter itemCounter, Inventory inventory)
    {
        var nowItemView = ItemViewerPooler.GetObj<ItemView>();
        return nowItemView.SetItemCounter(itemCounter, inventory);
    }

    public void ReturnItemView(ItemView itemView)
    {
        itemView.ItemCounter = null;
        ItemViewerPooler.returnObj(itemView.gameObject);
    }

    public void ShowItemDescription(ItemView itemView) => ItemDescriptionBox.Show(itemView);
    public void HideItemDescription() => ItemDescriptionBox.Hide();

    public class ItemCounter
    {
        public ItemSheet.Param Data { private set; get; }
        public int count { private set; get; }
        public float Probablilty { private set; get; }
        public ItemCounter(ItemSheet.Param data) => Data = data;
        ItemCounter(ItemSheet.Param data, int count) { Data = data; this.count = count; }
        public ItemCounter(ItemSheet.Param data, int count, float probablility) { Data = data; this.count = count; Probablilty = probablility; }
        public bool isWaitingNewItemView{ set; get; } = true;
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

        public ItemCounter CopyThis()
        {
            return new ItemCounter(Data, count);
        }
    }
}