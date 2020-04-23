using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;

public class ItemView : MonoBehaviour
{
    public Inventory Inventory { private set; get; }
    public ItemManager.ItemCounter ItemCounter { private set; get; }
    public RectTransform Transform { private set; get; }
    private Vector3 startPosition;
    public int index { set; get; }

    bool isDraging = false;
    List<Collider2D> otherColiders = new List<Collider2D>();

    private RectTransform scriptTransform;
    private Text scriptText;

    public static ItemView GetNew(Inventory _inventory, ItemManager.ItemCounter _ItemCounter, int _index)
    {
        ItemView view = Create.GetNew<ItemView>(_inventory.Transform);
        view.Inventory = _inventory;
        view.Transform = view.gameObject.GetComponent<RectTransform>();
        view.SetItemCounterInfo(_ItemCounter);
        view.index = _index;
        return view;
    }
    public void SetItemCounterInfo(ItemManager.ItemCounter _counter)
    {
        ItemCounter = _counter;
        ItemManager.Item item = ItemManager.GetItem(ItemCounter.Indexer);
        transform.GetChild(0).GetComponent<Image>().sprite = item.Image;
        transform.GetChild(1).GetComponent<Text>().text = item.Name + "x" + ItemCounter.Count;

        scriptTransform = transform.GetChild(2).gameObject.GetComponent<RectTransform>();
        scriptText = scriptTransform.GetChild(0).GetComponent<Text>();
        scriptTransform.gameObject.SetActive(false);
    }
    public void OnEntered() { 
        transform.SetAsLastSibling(); 
        scriptTransform.gameObject.SetActive(true); 
        scriptText.text = ItemManager.GetItem(ItemCounter.Indexer).Description;
        if (!Inventory.Area.Contains(scriptTransform.position)) { scriptTransform.localPosition = new Vector2(scriptTransform.localPosition.x * -1, scriptTransform.localPosition.y); }
    }
    public void OnOut() { scriptTransform.gameObject.SetActive(false); }
    public void Pressed() { 
        startPosition = transform.position; transform.parent.SetAsLastSibling(); 
        scriptTransform.gameObject.SetActive(false);
        if (!Inventory.isPlayer)
        {
            ItemManager.Item item = ItemManager.GetItem(ItemCounter.Indexer);
            if (item is ItemManager.ActiveItem)
            {
                ItemManager.ActiveItem nowItem = item as ItemManager.ActiveItem;
                Inventory.GoldText.text = (nowItem.Buy * ItemCounter.Count).ToString();
            }
        }
    }
    public void OnDrag() { transform.position = TouchManager.GetTouch(Transform); isDraging = true; }
    public void PressUp() 
    {
        if (isDraging)
        {
            Inventory.itemDrop(this);
            isDraging = false;
        }
        else
        {
            if (Inventory.isPlayer)
            {
                UseItem(true);
            }
        }
    }
    public void UseItem(bool _needComfirm)
    {
        StaticManager.coroutineStart(Inventory.UsePlayerItem(this, _needComfirm));
    }

    public void SetPositionInInventory(bool _isStartPosition)
    {
        if (otherColiders.Count <= 0 || _isStartPosition)
        {
            transform.position = startPosition;
        }
        else
        {
            float dist = Screen.width;
            int closer = otherColiders.Count + 1;
            for (int i = 0; i < otherColiders.Count; i++)
            {
                float nowDist = Vector2.Distance(otherColiders[i].transform.position, transform.position);
                if (nowDist < dist) { dist = nowDist; closer = otherColiders[i].GetComponent<ItemView>().index; }
            }
            Inventory.SwapingItem(new int[] { index, closer });
        }
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (isDraging)
        {
            if (otherColiders.Count == 0) { otherColiders.Add(collision); }
            else
            {
                bool found = false;
                for (int i = 0; i < otherColiders.Count; i++)
                {
                    if (otherColiders[i].Equals(collision)) { found = true; break; }
                }
                if (!found) { otherColiders.Add(collision); }
            }
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (isDraging)
        {
            for (int i = 0; i < otherColiders.Count; i++)
            {
                if (otherColiders[i].Equals(collision)) { otherColiders.RemoveAt(i); break; }
            }
        }
    }
}
