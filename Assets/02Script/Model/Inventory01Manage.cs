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

        int overNum = lastCounter.GetExcessCount(addCounter);
        if(lastCounter.View == null) { AddViewAndTableList(lastCounter); }

        if (overNum > 0)
        {
            int carry = overNum / lastCounter.Data.Limit;
            int lest = overNum % lastCounter.Data.Limit;
            int count = newCounter.Data.Limit;
            for(int i = 0; i < carry; i++)
            {
                newCounter = new ItemManager.ItemCounter(newCounter.Data);
                newCounter.GetExcessCount(count);
                AddViewAndTableList(newCounter);
            }

            if(lest > 0)
            {
                newCounter = new ItemManager.ItemCounter(newCounter.Data);
                newCounter.GetExcessCount(lest);
                AddViewAndTableList(newCounter);
            }
        }

        RefreashInventoryView();
    }

    public void AddItemForMonster(int itemIndex, int addCounter, float probablility) 
    {
        var counter = new ItemManager.ItemCounter(ItemManager.GetitemData(itemIndex), addCounter, probablility);
        var view = ItemManager.GetNewItemView(counter, this);

        itemViews.Add(view);
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
                StaticManager.ShowComfirmBox("이 아이템을 " + word + "하시겠습니까?");

                var comfirmBox = StaticManager.GetComfimBox;
                while (comfirmBox.NowState == ComfimBox.State.Waiting)
                    yield return new WaitForFixedUpdate();

                if (comfirmBox.NowState == ComfimBox.State.Yes)
                {
                    StateEffecterManager.EffectToModelByItem(Model, itemView.ItemCounter, false);
                    if(nowType == ItemSheet.Param.ItemTypeEnum.Equipment) { (Model as Character).EquipmentView.SetEquipmetItem(itemView); }
                    RemoveItem(itemView.ItemCounter.Data.Index, 1);
                }
            }
            else
            {
                StateEffecterManager.EffectToModelByItem(Model, itemView.ItemCounter, false);
                RemoveItem(itemView.ItemCounter.Data.Index, 1);
            }
        }
    }
}
