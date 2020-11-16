using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;

public class ItemView : MonoBehaviour, IInputTracer
{
    public ItemManager.ItemCounter ItemCounter { set; get; }
    public Inventory inventory { set; get; }
    public Rect Area { get { return GMath.GetRect(frame.GetComponent<RectTransform>()); } }
    public Image icon;
    public Text itemname;
    public Text count;
    public Transform frame;
    public bool IsContainInputPosition(Vector2 position) { return Area.Contains(position); }

    public ItemView SetItemCounter(ItemManager.ItemCounter counter, Inventory parentInventory)
    {
        ItemCounter = counter;
        counter.View = this;
        icon.sprite = Resources.Load<Sprite>("Item/" + ItemCounter.Data.ItemType + "/" + ItemCounter.Data.Name_eng);
        RefreshText();

        if (parentInventory != null)
        {
            inventory = parentInventory;
            transform.SetParent(parentInventory.itemViewGroup);
        }
        return this;
    }

    /*    public ItemView SetItemCounter(ItemManager.ItemCounter counter)
        {
            ItemCounter = counter;
            counter.View = this;
            icon.sprite = Resources.Load<Sprite>("Item/" + ItemCounter.Data.ItemType + "/" + ItemCounter.Data.Name_eng);
            RefreshText();

            return this;
        }*/
    public void SwapItemCounter(ItemManager.ItemCounter counter)
    {
        ItemCounter = counter;
        icon.sprite = Resources.Load<Sprite>("Item/" + ItemCounter.Data.ItemType + "/" + ItemCounter.Data.Name_eng);
        RefreshText();
    }
    public void RefreshText() { itemname.text = ItemCounter.Data.Name; count.text = ItemCounter.count.ToString(); }
    public void UseThis() => inventory.UseItem(this, false);
    public void SelectedIcon()
    {
        bool isTouch = false;
        int touchID = 0;
        bool isMouse = false;
        if (GPosition.IsContainInput(frame.GetComponent<RectTransform>(), out isTouch, out touchID, out isMouse))
        {
            StartCoroutine(TraceInput(isTouch, touchID, isMouse));

            if (inventory != null)
                if (!inventory.isPlayer)
                    inventory.SetGold(ItemCounter.Data.Buy * ItemCounter.count);
        }
    }

    public IEnumerator TraceInput(bool isTouch, int touchID, bool isMouse)
    {
        Color readyColor = new Color(0, 0, 0, 0.7f);
        var copy = Instantiate(gameObject, transform.root).GetComponent<ItemView>();
        icon.color -= readyColor;

        while (GPosition.IsHoldPressedInput(isTouch, touchID, isMouse))
        {
            if (GPosition.IsContainInput(Area, isTouch, touchID, isMouse))
            {
                ItemManager.Instance.ShowItemDescription(this);
            }
            else
            {
                ItemManager.Instance.HideItemDescription();
            }

            copy.frame.position = isTouch ? (Vector3)Input.touches[touchID].position : Input.mousePosition;
            yield return new WaitForFixedUpdate();
        }

        var copyPosition = copy.frame.position;
        Destroy(copy.gameObject);
        icon.color += readyColor;
        ItemManager.Instance.HideItemDescription();
        if (inventory != null) inventory.ItemDrop(this, copyPosition);
    }

    public void Pressed()
    {
        throw new System.NotImplementedException();
    }
}