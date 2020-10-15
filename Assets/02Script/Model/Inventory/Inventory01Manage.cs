using GLip;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public partial class Inventory : MonoBehaviour
{
    public void AddItem(int itemIndex, int addCounter, float probability)
    {
        StartCoroutine(ProcessAddItem(new ItemManager.ItemCounter(ItemManager.Instance.GetitemData(itemIndex), addCounter, probability), 0));
    }

    public void AddItem(int itemIndex, int addCounter)
    {
        StartCoroutine(ProcessAddItem(new ItemManager.ItemCounter(ItemManager.Instance.GetitemData(itemIndex)), addCounter));
    }

    public void AddGold(int gold)
    {
        SetGold(this.gold + gold);
    }
    
    public IEnumerator ProcessAddItem(ItemManager.ItemCounter itemCounter, int addCounter)
    {
        ItemManager.ItemCounter newCounter = itemCounter;
        ItemManager.ItemCounter sameKind = table.GetLastPositionItemCounter(newCounter.Data);

        ItemManager.ItemCounter lastCounter = sameKind == null ? newCounter : sameKind;

        int overNum = lastCounter.GetExcessCount(addCounter);
        //if (lastCounter.View == null) { yield return StartCoroutine(AddViewAndTableList(lastCounter)); }

        if (overNum > 0)
        {
            int carry = overNum / lastCounter.Data.Limit;
            int lest = overNum % lastCounter.Data.Limit;
            int count = newCounter.Data.Limit;
            for (int i = 0; i < carry; i++)
            {
                newCounter = new ItemManager.ItemCounter(newCounter.Data);
                newCounter.GetExcessCount(count);
                yield return StartCoroutine(AddViewAndTableList(newCounter));
            }

            if (lest > 0)
            {
                newCounter = new ItemManager.ItemCounter(newCounter.Data);
                newCounter.GetExcessCount(lest);
                yield return StartCoroutine(AddViewAndTableList(newCounter));
            }
        }
        else if(lastCounter.isWaitingNewItemView)
        {
            yield return StartCoroutine(AddViewAndTableList(lastCounter));
        }

        RefreashInventoryView();
    }

    public void AddItem(ItemManager.ItemCounter counter)
    {
        if (counter.View != null) 
            ItemManager.Instance.ReturnItemView(counter.View);
        
        AddItem(counter.Data.Index, counter.count);
    }
    IEnumerator AddViewAndTableList(ItemManager.ItemCounter newCounter)
    {
        table.AddItemCounter(newCounter);
        newCounter.isWaitingNewItemView = false;
        yield return new WaitUntil(() => ItemManager.Instance.IsItemViewPoolerReady);
        var itemView = ItemManager.Instance.GetNewItemView(newCounter, this);
        GPosition.GetRectTransformWithReset(rectTransform, itemView.GetComponent<RectTransform>());
        itemView.gameObject.SetActive(true);
        itemViews.Add(itemView);
        yield return null;
    }

    public bool RemoveItem(int itemIndex, int removeCount)
    {
        List<ItemManager.ItemCounter> kindList;
        if(table.GetSameKindTotalCount(ItemManager.Instance.GetitemData(itemIndex), out kindList) >= removeCount)
        {
            while(removeCount > 0)
            {
                var nowCounter = kindList[kindList.Count - 1];
                removeCount = nowCounter.RemoveCountWithOverFlow(removeCount);

                if (removeCount <= 0)
                {
                    table.RemoveItemCounter(nowCounter);
                    nowCounter.View.ItemCounter = null;
                }
                removeCount *= -1;
            }
            RefreashInventoryView();
            return true;
        }
        return false;
    }
    public bool RemoveItem(ItemManager.ItemCounter counter) => RemoveItem(counter.Data.Index, counter.count);

    public IEnumerator UseItem(ItemView itemView, bool useComfirmBox)
    {
        ItemSheet.Param.ItemTypeEnum nowType = itemView.ItemCounter.Data.GetItemType;
        var character = Model as Character;
        if (nowType == ItemSheet.Param.ItemTypeEnum.Key)
        {
            character.ShowAlert("인위적으로 사용할 수 없는 아이템 입니다.", Color.red);
        }
        else
        {
            if (useComfirmBox)
            {
                string word = nowType == ItemSheet.Param.ItemTypeEnum.Active ? "사용" : "장착";
                var confirmBox = ConfimBoxManager.instance;
                confirmBox.ShowComfirmBox("이 아이템을 " + word + "하시겠습니까?");

                while (confirmBox.NowState == ConfimBoxManager.State.Waiting)
                    yield return new WaitForFixedUpdate();

                if (confirmBox.NowState == ConfimBoxManager.State.Yes)
                {
                    StateEffecterManager.EffectToModelByItem(itemView.ItemCounter, Model, false);
                    if(nowType == ItemSheet.Param.ItemTypeEnum.Equipment) { (Model as Character).EquipmentView.SetEquipmetItem(itemView); }
                    RemoveItem(itemView.ItemCounter.Data.Index, 1);
                }
            }
            else
            {
                StateEffecterManager.EffectToModelByItem(itemView.ItemCounter, Model, false);
                RemoveItem(itemView.ItemCounter.Data.Index, 1);
            }
        }
    }
}
