using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;

public class ItemView : MonoBehaviour, IInputTracer
{
    public ItemManager.ItemCounter ItemCounter { set; get; }
    public Inventory inventory { set; get; }
    public Rect Area { get { return GMath.GetRect(transform.GetComponent<RectTransform>()); } }
    public Image Icon { set; get; }
    public Text Name { set; get; }
    public Text Count { set; get; }
    public bool IsContainInputPosition(Vector2 position) { return Area.Contains(position); }

    ItemViewDiscritionBox discritionBox { set; get; }

    private void Awake()
    {
        Icon = transform.Find("ShowImage").GetComponent<Image>();
        Name = transform.Find("Name").GetComponent<Text>();
        Count = transform.Find("Count").GetComponent<Text>();
    }
    public void SetItemCounter(ItemManager.ItemCounter counter, Inventory parentInventory)
    {
        inventory = parentInventory;
        ItemCounter = counter;
        Icon.sprite = Resources.Load<Sprite>("Item/" + ItemCounter.Data.ItemType + "/" + ItemCounter.Data.Name_eng);
        RefreshText();
        discritionBox = new ItemViewDiscritionBox(transform.Find("DiscriptionBox"), this);

        SetDiscriptionBox();

        transform.SetParent(parentInventory.transform);
    }
    public void SwapItemCounter(ItemManager.ItemCounter counter)
    {
        ItemCounter = counter;
        Icon.sprite = Resources.Load<Sprite>("Item/" + ItemCounter.Data.ItemType + "/" + ItemCounter.Data.Name_eng);
        RefreshText();

        SetDiscriptionBox();
    }
    public void RefreshText() { Name.text = ItemCounter.Data.Name; Count.text = ItemCounter.count.ToString(); }
    public void SetDiscriptionBox() => discritionBox.SetDiscription();
    public void ShowDiscriptionBox() => discritionBox.ShowDiscription();
    public void HideDiscriptionBox() => discritionBox.HideDiscriprtion();
    public void UseThis() => StartCoroutine(inventory.UseItem(this, false));
    public void SelectedIcon()
    {
        bool isTouch = false;
        int touchID = 0;
        bool isMouse = false;
        if (GPosition.IsContainInput(transform.GetComponent<RectTransform>(), out isTouch, out touchID, out isMouse))
        {
            StartCoroutine(TraceInput(isTouch, touchID, isMouse));
            if (!inventory.isPlayer) { inventory.SetGold(ItemCounter.Data.Buy * ItemCounter.count); }
        }
    }

    public IEnumerator TraceInput(bool isTouch, int touchID, bool isMouse)
    {
        Color readyColor = new Color(0, 0, 0, 0.7f);
        Transform copy = Instantiate(gameObject, transform.root).GetComponent<Transform>();
        copy.position = transform.position;
        Icon.color -= readyColor;

        while (GPosition.IsHoldPressedInput(isTouch, touchID, isMouse))
        {
            if(GPosition.IsContainInput(Area, isTouch, touchID, isMouse))
            {
                ShowDiscriptionBox();
            }
            else
            {
                HideDiscriptionBox();
            }

            copy.position = isTouch ? (Vector3)Input.touches[touchID].position : Input.mousePosition;
            yield return new WaitForFixedUpdate();
        }

        var copyPosition = copy.position;
        Destroy(copy.gameObject);
        Icon.color += readyColor;
        HideDiscriptionBox();
        inventory.ItemDrop(this, copyPosition);
    }

    public void Pressed()
    {
        throw new System.NotImplementedException();
    }

    class ItemViewDiscritionBox
    {
        ItemView View { set; get; }
        Transform Transform { set; get; }
        Image Icon { set; get; }
        Text NameText { set; get; }
        Text DiscriptionText { set; get; }

        public ItemViewDiscritionBox(Transform transform, ItemView view)
        {
            View = view;
            Transform = transform;
            Icon = transform.Find("ShowImage").GetComponent<Image>();
            NameText = transform.Find("NameTextBox").GetComponent<Text>();
            DiscriptionText = transform.Find("DiscriptionTextBox").GetComponent<Text>();
        }

        public void SetDiscription()
        {
            Icon.sprite = View.Icon.sprite;
            NameText.text = View.Name.text;
            DiscriptionText.text = View.ItemCounter.Data.Description;
            if (!View.inventory.isPlayer)
                Transform.position = GPosition.ReverceLeft(Transform.position);
            Transform.gameObject.SetActive(false);
        }

        public void ShowDiscription() => Transform.gameObject.SetActive(true);
        public void HideDiscriprtion() => Transform.gameObject.SetActive(false);
    }
}