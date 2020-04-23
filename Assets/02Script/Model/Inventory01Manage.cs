using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Inventory : MonoBehaviour
{
    public void AddItem(ItemManager.ItemCounter _addItem)
    {
        ItemCounterTable nowTableData = GetSameItemTable(_addItem.Indexer);

        ItemManager.ItemCounter lastCounter = nowTableData != null ? nowTableData.GetLastCounter : _addItem;
        int itemLimit = ItemManager.GetItem(_addItem.Indexer).Limit;

        lastCounter.Count += nowTableData != null ? _addItem.Count : 0;
        int carry = lastCounter.Count / itemLimit;
        int lest = lastCounter.Count % itemLimit;

        if(carry > 0)
        {
            lastCounter.Count = itemLimit;
            if (nowTableData == null) { inventoryInsert(lastCounter); }
            for(int i = 1; i < carry; i++)
            {
                inventoryInsert(new ItemManager.ItemCounter(_addItem.Indexer, itemLimit));
            }
        }

        if (lest > 0)
        {
            _addItem.Count = lest;
            if (carry > 0 || nowTableData == null)inventoryInsert(_addItem);
            RefreshItemViewer(lastCounter);
        }
    }
    void inventoryInsert(ItemManager.ItemCounter _newItem) 
    {
        ItemCounterTable nowTable = GetSameItemTable(_newItem.Indexer);
        if (nowTable == null) 
        { 
            itemTable.Add(new ItemCounterTable()); 
            nowTable = itemTable[itemTable.Count - 1]; 
        }
        nowTable.lists.Add(_newItem);
        ItemCounters.Add(_newItem);
        itemViews.Add(ItemView.GetNew(this, _newItem, itemViews.Count - 1));
    }

    ItemCounterTable GetSameItemTable(ItemManager.ItemIndexer _tarIndexer)
    {
        for(int i = 0; i < itemTable.Count; i++)
        {
            if (itemTable[i].GetIndexer.IsSame(_tarIndexer)) { return itemTable[i]; }
        }
        return null;
    }

    public ItemView GetHeadItemView(ItemView _nowVIew)
    {
        foreach(ItemView view in itemViews)
        {
            if (view.ItemCounter.IsSame(_nowVIew.ItemCounter)) { return view; }
        }
        return null;
    }

    public void RemoveItem(ItemManager.ItemCounter _itemCounter)
    {
        ItemCounterTable table = GetSameItemTable(_itemCounter.Indexer);
        ItemManager.ItemCounter lastCounter = table.GetLastCounter;
        lastCounter.Count--;
            
        RefreshItemViewer(lastCounter);
        if(lastCounter.Count <= 0)
        {
            ItemCounters.Remove(lastCounter);
            table.lists.Remove(lastCounter);
            if(table.lists.Count == 0) { itemTable.Remove(table); }
        }
    }

    public IEnumerator UsePlayerItem(ItemView _itemView, bool _useComfirmBox)
    {
        Character character = StaticManager.Player.Character;
        if (_useComfirmBox)
        {
            ComfimBox comfimBox = StaticManager.GetComfimBox;
            if (_itemView.ItemCounter.Indexer.Kinds == ItemManager.Kinds.activeItemList)
                comfimBox.ShowComfirmBox("이 아이템을 사용하시겠습니까?");
            else if (_itemView.ItemCounter.Indexer.Kinds == ItemManager.Kinds.EquipmentItemList)
                comfimBox.ShowComfirmBox("이 아이템을 장착하시겠습니까?");

            while (comfimBox.NowState == ComfimBox.State.Waiting)
            {
                yield return new WaitForFixedUpdate();
            }

            if (comfimBox.NowState == ComfimBox.State.Yes)
            {
                character.UseItem(GetitemUsingProcess(_itemView));
            }
        }
        else
        {
            character.UseItem(GetitemUsingProcess(_itemView));
        }
    }
    private ItemManager.ItemIndexer GetitemUsingProcess(ItemView _itemView)
    {
        ItemManager.ItemCounter lastCount = GetSameItemTable(_itemView.ItemCounter.Indexer).GetLastCounter;
        if (lastCount.Indexer.Kinds != ItemManager.Kinds.keyItemList)
        {
            RemoveItem(lastCount);
            ShowInventory();
        }
        return lastCount.Indexer;
    }

    private void RefreshItemViewer(ItemManager.ItemCounter _itemCounter)
    {
        ItemView nowView = itemViews[FindItemViewIndex(_itemCounter)];

        if(nowView != null)
        {
            if (nowView.ItemCounter.Count <= 0)
            {
                itemViews.RemoveAt(FindItemViewIndex(nowView.ItemCounter));
                Destroy(nowView.gameObject);
            }
            else
            {
                nowView.SetItemCounterInfo(nowView.ItemCounter);
            }
        }
    }
    private int FindItemViewIndex(ItemManager.ItemCounter _itemCounter)
    {
        for (int i = 0; i < itemViews.Count; i++) { if (_itemCounter.Equals(itemViews[i].ItemCounter)) return i; }
        return -1;
    }
}
