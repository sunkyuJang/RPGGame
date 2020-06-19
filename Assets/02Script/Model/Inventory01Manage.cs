using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public partial class Inventory : MonoBehaviour
{
    public void AddItem(int itemIndex, int addCounter)
    {
        ItemManager.ItemCounter newCounter = new ItemManager.ItemCounter(ItemManager.GetitemData(itemIndex));
        ItemManager.ItemCounter sameKind = table.GetLastPositionItemCounter(newCounter.Data);

        ItemManager.ItemCounter lastCounter = sameKind == null ? newCounter : sameKind;

        int overNum = 0;
        if(lastCounter.AddCount(sameKind == null ? 0 : addCounter, out overNum))
        {
            int carry = overNum / lastCounter.Data.Limit;
            int lest = overNum % lastCounter.Data.Limit;
            int count = newCounter.Data.Limit;
            for(int i = 0; i < carry; i++)
            {
                newCounter = new ItemManager.ItemCounter(newCounter.Data);
                newCounter.AddCount(count, out overNum);
                AddViewAndTableList(newCounter);
                
                //Repeat for lest
                if(lest > 0 && i == carry - 1) 
                { 
                    count = lest;
                    lest = 0;
                    i--; 
                }
            }
        }
        else
        {
            AddViewAndTableList(lastCounter);
        }
    }
    public void AddItem(ItemManager.ItemCounter counter) => AddItem(counter.Data.Index, counter.count);

    void AddViewAndTableList(ItemManager.ItemCounter newCounter)
    {
        table.AddItemCounter(newCounter);
        itemViews.Add(ItemManager.GetNewItemView(newCounter, this));
    }

    public bool RemoveItem(int itemIndex, int removeCount)
    {
        List<ItemManager.ItemCounter> kindList;
        if(table.GetSameKindTotalCount(ItemManager.GetitemData(itemIndex), out kindList) >= removeCount)
        {
            while(removeCount > 0)
            {
                var nowCounter = kindList[kindList.Count - 1];
                removeCount = nowCounter.RemoveCountWithOverFlow(removeCount);
                
                if(removeCount > 0)
                    table.RemoveItemCounter(nowCounter);
            }
            return true;
        }
        return false;
    }
    public bool RemoveItem(ItemManager.ItemCounter counter) => RemoveItem(counter.Data.Index, counter.count);

    public IEnumerator UseItem(ItemView itemView, bool useComfirmBox)
    {
        Character character = Model as Character;

        ItemSheet.Param.ItemTypeEnum nowType = itemView.ItemCounter.Data.GetItemType;
        if (nowType == ItemSheet.Param.ItemTypeEnum.Key)
        {
            StaticManager.ShowAlert("인위적으로 사용할 수 없는 아이템 입니다.", Color.red);
        }
        else
        {
            if (useComfirmBox)
            {
                string word = nowType == ItemSheet.Param.ItemTypeEnum.Active ? "사용" : "장착";
                StaticManager.ShowComfirmBox("이 아이템을 " + word + "하시겠습니까?");

                var comfirmBox = StaticManager.GetComfimBox;
                while (comfirmBox.NowState == ComfimBox.State.Waiting)
                    yield return new WaitForFixedUpdate();

                if (comfirmBox.isYes)
                {
                    StateEffecterManager.EffectToModel(itemView.transform, Model, false);
                }
            }
            else
            {
                StateEffecterManager.EffectToModel(itemView.transform, Model, false);
            }
        }
    }

/*    private ItemManager.ItemIndexer GetitemUsingProcess(ItemView _itemView)
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
    }*/
}
