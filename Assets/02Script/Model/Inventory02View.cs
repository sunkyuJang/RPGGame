using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;
using System.IO;
using JetBrains.Annotations;

public partial class Inventory : MonoBehaviour
{
    public Vector3 goldTextPosition;
    public Text GoldText { set; get; }
    public double gold { set; get; } = 0;
    public void SetGold(double gold) { this.gold = gold; GoldText.text = this.gold.ToString(); }
    Vector2 viewStartPoint { get { return new Vector2(-160f, 210f); } }
    void SetViewPosition()
    {
        Transform goldTransform = transform.GetChild(0);
        goldTextPosition = goldTransform.localPosition;
        GoldText = goldTransform.GetChild(1).GetComponent<Text>();
        if (!isPlayer) transform.localPosition = new Vector3(transform.localPosition.x * -1, transform.localPosition.y, transform.localPosition.z);
        Area = GMath.GetRect(rectTransform);
    }
    public void ShowInventory()
    {
        gameObject.SetActive(true);
        RefreashInventoryView();
    }

    void RefreashInventoryView()
    {
        GoldText.text = gold.ToString();
        Vector2 startPosition = viewStartPoint;
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
                Destroy(itemView.gameObject);
                itemViews.RemoveAt(i);
                i--;
            }
        }
    }

    public void HideInventory()
    {
        gameObject.SetActive(false);
        if (Model is Character) (Model as Character).ShowGameUI(true);
    }

    public void ItemDrop(ItemView itemView, Vector2 dropPosition)
    {
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
                    //SwapItem
                    SwapItem(itemView, dropPosition);
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
                else if(TargetInventory != null)
                {
                    if (TargetInventory.Area.Contains(dropPosition))
                    {
                        //sellItem
                        StartCoroutine(TradeItem(itemView, TargetInventory, false));
                    }
                }
            }
        }
        else
        {
            //Buy item
            StartCoroutine(TradeItem(itemView, TargetInventory, true));
        }
    }
    void SwapItem(ItemView itemView, Vector2 dropPosition)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(dropPosition, itemView.GetComponent<BoxCollider2D>().size, 0f);

        dropPosition -= new Vector2(transform.position.x, transform.position.y);
        var itemViewCollider = itemView.gameObject.GetComponent<BoxCollider2D>(); 
        Collider2D targetCollider = null;
        if(colliders.Length > 0)
        {
            float closeDist = 1000f;
            foreach (Collider2D collider in colliders)
            {
                if (itemViewCollider != collider.transform)
                {
                    var nowDist = Vector2.Distance(collider.transform.position, dropPosition);
                    if (nowDist < closeDist)
                    {
                        closeDist = nowDist;
                        targetCollider = collider;
                    }
                }
            }

            var targetView = targetCollider.transform.GetComponent<ItemView>();
            if (targetView.ItemCounter.Data.Index == itemView.ItemCounter.Data.Index)
            {
                if (targetView.ItemCounter.count < targetView.ItemCounter.Data.Limit)
                {
                    var excessCount = targetView.ItemCounter.GetExcessCount(itemView.ItemCounter.count);

                    if (excessCount <= 0)
                    {
                        table.RemoveItemCounter(itemView.ItemCounter);
                        itemView.ItemCounter = null;
                        RefreashInventoryView();
                        print(table.GetSameKind(targetView.ItemCounter.Data).Count);
                    }
                    else
                        itemView.ItemCounter.RemoveCountWithOverFlow(itemView.ItemCounter.Data.Limit - excessCount);
                }
            }
            else
            {
                var tempCounter = itemView.ItemCounter;
                itemView.SwapItemCounter(targetView.ItemCounter);
                targetView.SwapItemCounter(tempCounter);

                for (int i = 0, max = 2; i < max; i++)
                {
                    int indexCount = 0;
                    foreach (ItemView nowView in itemViews)
                    {
                        if (nowView == itemView) { break; }
                        else if (nowView.ItemCounter.Data.Index == itemView.ItemCounter.Data.Index)
                            indexCount++;
                    }
                    table.RemoveItemCounter(itemView.ItemCounter);
                    table.InsertItemCounter(itemView.ItemCounter, indexCount);
                    if (i == 0) itemView = targetView;
                }
            }
        }
    }

    IEnumerator TradeItem(ItemView itemView, Inventory targetInventory, bool isBuying)
    {
        StaticManager.ShowComfirmBox("아이템을 " + (isBuying ? "구매" : "판매") + " 하시겠습니까?");

        while (StaticManager.GetComfimBox.NowState == ComfimBox.State.Waiting)
            yield return new WaitForFixedUpdate();

        if(StaticManager.GetComfimBox.NowState == ComfimBox.State.Yes)
        {
            if (isBuying) // !isPlayer
            {
                var cost = itemView.ItemCounter.Data.Buy * itemView.ItemCounter.count;
                var nowGold = targetInventory.gold - cost;
                if (nowGold >= 0)
                {
                    targetInventory.AddItem(itemView.ItemCounter);
                    targetInventory.SetGold(nowGold);
                    try
                    {
                        RemoveItem(itemView.ItemCounter);
                    }
                    catch { print("SomethingWrongInInventory02"); }
                }
                else 
                {
                    (targetInventory.Model as Character).ShowAlert("잔액이 모자릅니다.", Color.red); 
                }
            }
            else
            {
                gold += itemView.ItemCounter.count * itemView.ItemCounter.Data.Sell;
                RemoveItem(itemView.ItemCounter);
            }
        }
    }
}

