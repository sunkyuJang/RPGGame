using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;

public partial class Inventory : MonoBehaviour
{
    public RectTransform Transform { private set; get; }
    public Rect Area { private set; get; }
    public bool isPlayer { private set; get; }

    public List<ItemManager.ItemCounter> ItemCounters { private set; get; } = new List<ItemManager.ItemCounter>();
    private List<ItemView> itemViews = new List<ItemView>();
    public int length { set; get; }
    List<ItemCounterTable> itemTable = new List<ItemCounterTable>();

    void SetConstructor()
    {
        Transform = gameObject.GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }
    public bool HasItemCounters { get { return ItemCounters.Count > 0; } }
    public static Inventory GetNew(int length, bool isPlayer) 
    { 
        Inventory inventory = Create.GetNew<Inventory>();
        inventory.SetConstructor();
        inventory.length = length;
        inventory.isPlayer = isPlayer;
        inventory.AwakeView();
        return inventory;
    }
    class ItemCounterTable
    {
        public List<ItemManager.ItemCounter> lists = new List<ItemManager.ItemCounter>();
        public ItemManager.ItemIndexer GetIndexer { get { return lists.Count > 0 ? lists[0].Indexer : null; } }
        public ItemManager.ItemCounter GetLastCounter { get { return lists.Count > 0 ? lists[lists.Count - 1] : null; } }
    }
}
