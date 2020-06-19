using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;
using System.IO;
using JetBrains.Annotations;

public partial class Inventory : MonoBehaviour
{
    public Text GoldText { set; get; }
    public double gold { set; get; } = 0;


    public void ShowInventory()
    {
        gameObject.SetActive(true);
        GoldText.text = gold.ToString();
        Vector2 startPosition = new Vector2(-160f, 210f);
        Vector2 nextPosition = startPosition;
        float interval = 10;
        for (int i = 0; i < itemViews.Count; i++)
        {
            var itemView = itemViews[i];
            if (itemView.ItemCounter != null)
            {
                itemView.transform.localPosition = nextPosition;
                nextPosition += new Vector2(interval + itemView.Area.width, 0);
                if ((i + 1) % 4 == 0 && i != 0)
                {
                    nextPosition = new Vector2(startPosition.x, nextPosition.y - (interval + itemView.Area.height));
                }
            }
            else
            {
                Destroy(itemView);
                itemViews.RemoveAt(i);
                i--;
            }
        }
    }
    public void HideInventory()
    {
        gameObject.SetActive(false);
        if (Model is Character) (Model as Character).IntoNomalUI();
    }

    /*public void SwapingItem(int[] indexs)
    {
        ItemView viewFir = itemViews[indexs[0]];
        ItemView viewSec = itemViews[indexs[1]];

        ItemManager.ItemCounter temp = viewFir.ItemCounter;

        viewFir.SetItemCounterInfo(viewSec.ItemCounter);
        viewSec.SetItemCounterInfo(temp);

        ShowInventory();
    }*/

    public void ItemDrop(ItemView itemView, Vector2 dropPosition)
    {
        Collider2D[] objects = Physics2D.OverlapBoxAll(dropPosition, itemView.GetComponent<RectTransform>().sizeDelta * 2, 0f);
        if (isPlayer)
        {
            if (Area.Contains(dropPosition))
            {
                if (itemView.IsContainInputPosition(dropPosition))
                {
                    //usingItem
                    StartCoroutine(UseItem(itemView, true));
                }
                else
                {
                    //swapItem

                }
            }
            else
            {
                var Character = Model as Character;

                var slotNum = Character.QuickSlot.IsIn(dropPosition);
                if (slotNum >= 0)
                {
                    //registQuickSlot
                    Character.QuickSlot.SetSlot(itemView.transform, slotNum);
                }
                else
                {
                    //sellItem
                    StartCoroutine(TradeItem(itemView, TargetInventory, false));
                }
            }
        }
        else
        {
            //Buy item
            StartCoroutine(TradeItem(itemView, TargetInventory, true));
        }
    }

/*        List<T> GetOrderList<T>(Collider2D[] colliders)
        {
            List<T> orderList = new List<T>();
            foreach (Collider2D collider in colliders)
            {
                T nowT = collider.GetComponent<T>();
                if (nowT != null)
                {
                    orderList.Add(nowT);
                    if(!(nowT is ItemManager.ItemView))
                    {
                        break;
                    }
                }
            }
            return orderList;
        }*/

        IEnumerator TradeItem(ItemView itemView, Inventory targetInventory, bool isBuying)
        {
            StaticManager.ShowComfirmBox("아이템을 " + (isBuying ? "구매" : "판매") + " 하시겠습니까?");

            while (StaticManager.GetComfimBox.NowState == ComfimBox.State.Waiting)
                yield return new WaitForFixedUpdate();

            if(StaticManager.GetComfimBox.NowState == ComfimBox.State.Yes)
            {
                if (isBuying)
                {
                    var cost = itemView.ItemCounter.Data.Buy;
                    var nowGold = gold - cost;
                    if (nowGold >= 0)
                    {
                        AddItem(itemView.ItemCounter);
                        gold = nowGold;
                        try
                        {
                            targetInventory.RemoveItem(itemView.ItemCounter);
                        }
                        catch { print("SomethingWrongInInventory02"); }
                    }
                    else { StaticManager.ShowAlert("잔액이 모자릅니다.", Color.red); }
                }
                else
                {
                    gold += itemView.ItemCounter.count * itemView.ItemCounter.Data.Sell;
                    table.RemoveItemCounter(itemView.ItemCounter);
                    itemViews.Remove(itemView);
                }
            }
        }
        /*Vector2 viewPosition = _itemView.Transform.position;
        if (Area.Contains(viewPosition))
        {
            if(!isPlayer) _itemView.SetPositionInInventory(true);
            else { _itemView.SetPositionInInventory(false); }
        }
        else
        {
            Character character= StaticManager.Character;
            if (isPlayer)
            {
                QuickSlot playerQuickSlot = character.QuickSlot;
                if (playerQuickSlot.gameObject.activeSelf)
                {
                    int slotNum = playerQuickSlot.IsIn(viewPosition);
                    if (slotNum >= 0)
                    {
                        if (_itemView.ItemCounter.Indexer.Kinds == ItemManager.Kinds.activeItemList)
                        {
                            playerQuickSlot.SetSlot(GetHeadItemView(_itemView).transform, slotNum);
                        }
                        else
                        {
                            StaticManager.ShowAlert("해당 아이템은 퀵슬롯에 등록할 수 없습니다.", Color.red);
                        }
                    }
                }
            }
            else
            {
                Inventory playerInventory = character.Inventory;
                if (playerInventory.Area.Contains(viewPosition))
                {
                    if (playerInventory.CanTrade(_itemView.ItemCounter))
                    {
                        itemViews.RemoveAt(_itemView.index);
                        Destroy(_itemView.gameObject);
                        return;
                    }
                }
            }
            _itemView.SetPositionInInventory(true);
        }*/
}

