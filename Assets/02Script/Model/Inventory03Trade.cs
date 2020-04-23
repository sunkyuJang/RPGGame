using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class Inventory : MonoBehaviour
{
    public bool CanTrade(ItemManager.ItemCounter _itemCounter)
    {
        ItemManager.Item item = ItemManager.GetItem(_itemCounter.Indexer);
        if (item is ItemManager.ActiveItem)
        {
            ItemManager.ActiveItem tradeItem = item as ItemManager.ActiveItem;
            double price = tradeItem.Buy * _itemCounter.Count;
            if (price <= gold)
            {
                gold -= price;
                AddItem(_itemCounter);
                ShowInventory();
                return true;
            }
            else
            {
                StaticManager.ShowAlert("트레이드 할 수 없습니다.", Color.white);
            }
        }
        return false;
    }
}
