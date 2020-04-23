using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;

public partial class Inventory : MonoBehaviour
{
    public Text GoldText { set; get; }
    public double gold { set; get; } = 0;

    void AwakeView()
    {
        GoldText = transform.GetChild(0).GetChild(1).GetComponent<Text>();
        if (!isPlayer) Transform.localPosition = new Vector3(Transform.localPosition.x * -1, Transform.localPosition.y, Transform.localPosition.z);
        Area = GMath.GetRect(Transform);
    }

    public void ShowInventory()
    {
        gameObject.SetActive(true);
        GoldText.text = gold.ToString();
        Vector2 startPosition = new Vector2(-160f, 210f);
        Vector2 nextPosition = startPosition;
        float interval = 10;
        for (int i = 0; i < itemViews.Count; i++)
        {
            ItemView itemView = itemViews[i];
            if (itemView.ItemCounter != null)
            {
                itemView.index = i;
                itemView.Transform.localPosition = nextPosition;
                nextPosition += new Vector2(interval + itemView.Transform.rect.width, 0);
                if ((i + 1) % 4 == 0 && i != 0)
                {
                    nextPosition = new Vector2(startPosition.x, nextPosition.y - (interval + itemView.Transform.rect.height));
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
        StaticManager.Player.HideInventoryView();
    }

    public void SwapingItem(int[] indexs)
    {
        ItemView viewFir = itemViews[indexs[0]];
        ItemView viewSec = itemViews[indexs[1]];

        ItemManager.ItemCounter temp = viewFir.ItemCounter;

        viewFir.SetItemCounterInfo(viewSec.ItemCounter);
        viewSec.SetItemCounterInfo(temp);

        ShowInventory();
    }

    public void itemDrop(ItemView _itemView)
    {
        Vector2 viewPosition = _itemView.Transform.position;
        if (Area.Contains(viewPosition))
        {
            if(!isPlayer) _itemView.SetPositionInInventory(true);
            else { _itemView.SetPositionInInventory(false); }
        }
        else
        {
            Player player = StaticManager.Player;
            if (isPlayer)
            {
                QuickSlot playerQuickSlot = player.Character.QuickSlot;
                if (playerQuickSlot.gameObject.activeSelf)
                {
                    int slotNum = playerQuickSlot.IsIn(viewPosition);
                    if (slotNum >= 0)
                    {
                        if (_itemView.ItemCounter.Indexer.Kinds == ItemManager.Kinds.activeItemList)
                        {
                            playerQuickSlot.SetSlot(GetHeadItemView(_itemView), slotNum);
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
                Inventory playerInventory = player.Inventory;
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
        }
    }
}
