﻿using System.Collections.Generic;
using UnityEngine;
using GLip;
public partial class Inventory : MonoBehaviour
{
    Model Model { set; get; }
    public GameObject inventoryFrame;
    public RectTransform rectTransform { private set; get; }
    Inventory TargetInventory { set; get; }
    public Transform itemViewGroup;
    public Rect Area { get { return GMath.GetRect(inventoryFrame.GetComponent<RectTransform>()); } }
    public bool isPlayer { private set; get; }

    public GameObject itemViewPrefab;
    public List<ItemView> itemViews { private set; get; } = new List<ItemView>();
    public bool HasItem { get { return itemViews.Count > 0; } }
    public int length { set; get; }

    public ItemCounterTable table { private set; get; }
    private void Awake()
    {
        table = new ItemCounterTable();
        rectTransform = inventoryFrame.GetComponent<RectTransform>();
    }

    public void SetTransformParent()
    {
        transform.SetParent(GetInventoryGroup);
        var rectTransform = transform.GetComponent<RectTransform>();
        transform.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        transform.GetComponent<RectTransform>().anchorMax = Vector2.one;
    }

    Transform GetInventoryGroup
    {
        get
        {
            var InventoryGroupTransform = GameManager.mainCanvas.Find("InventoryGroup");
            if (InventoryGroupTransform == null)
            {
                var inventoryGroupRectTransform = new GameObject("InventoryGroup", typeof(RectTransform)).GetComponent<RectTransform>();
                inventoryGroupRectTransform.SetParent(GameManager.mainCanvas, true);

                InventoryGroupTransform = inventoryGroupRectTransform;
            }
            return InventoryGroupTransform;
        }
    }

    public void SetDefault(Model model)
    {
        Model = model;
        isPlayer = model is Character ? true : false;
        SetViewPosition();
        gameObject.name = model.CharacterName + "inventory";
        inventoryFrame.SetActive(false);
    }

    public void ShowInventoryForTrade(Inventory targetInventory)
    {
        TargetInventory = targetInventory;
        ShowInventory();
    }
    public class ItemCounterTable
    {
        public List<List<ItemManager.ItemCounter>> itemCounters { set; get; } = new List<List<ItemManager.ItemCounter>>();
        public List<ItemManager.ItemCounter> GetSameKind(ItemSheet.Param item)
        {
            if (itemCounters.Count > 0)
            {
                for (int kindIndex = 0; kindIndex < itemCounters.Count; kindIndex++)
                {
                    List<ItemManager.ItemCounter> kinds = itemCounters[kindIndex];
                    if (item.Index == kinds[0].Data.Index)
                    {
                        return kinds;
                    }
                }
            }
            return null;
        }
        public int GetSameKindTotalCount(ItemSheet.Param item)
        {
            List<ItemManager.ItemCounter> kind = GetSameKind(item);
            return kind == null ? 0 : (kind[0].Data.Limit * (kind.Count - 1)) + kind[kind.Count - 1].count; // carry + lest
        }
        public int GetSameKindTotalCount(ItemSheet.Param item, out List<ItemManager.ItemCounter> kind)
        {
            kind = GetSameKind(item);
            return kind == null ? 0 : (kind[0].Data.Limit * (kind.Count - 1)) + kind[kind.Count - 1].count; // carry + lest
        }
        public ItemManager.ItemCounter GetLastPositionItemCounter(ItemSheet.Param item)
        {
            List<ItemManager.ItemCounter> kinds = GetSameKind(item);
            return kinds == null ? null : kinds[kinds.Count - 1];
        }
        public void AddItemCounter(ItemManager.ItemCounter nowCounter)
        {
            var kind = GetSameKind(nowCounter.Data);
            if (kind == null) { itemCounters.Add(AddNewKind(nowCounter)); }
            else kind.Add(nowCounter);
        }
        List<ItemManager.ItemCounter> AddNewKind(ItemManager.ItemCounter counter)
        {
            var list = new List<ItemManager.ItemCounter>();
            list.Add(counter);
            return list;
        }
        public void InsertItemCounter(ItemManager.ItemCounter nowCounter, int insertNum)
        {
            var kind = GetSameKind(nowCounter.Data);
            if (kind == null) AddItemCounter(nowCounter);
            else kind.Insert(insertNum, nowCounter);
        }
        public void RemoveItemCounter(ItemManager.ItemCounter nowCounter)
        {
            var kind = GetSameKind(nowCounter.Data);
            kind.Remove(nowCounter);
            if (kind.Count == 0) { itemCounters.Remove(kind); }
        }
    }
}
