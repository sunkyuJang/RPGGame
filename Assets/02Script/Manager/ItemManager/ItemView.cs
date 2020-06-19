using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;

public class ItemView : MonoBehaviour
{
    public ItemManager.ItemCounter ItemCounter { set; get; }
    public Inventory inventory { set; get; }
    public Rect Area { private set; get; }
    public Image Icon { set; get; }
    public Text Name { set; get; }
    public bool IsContainInputPosition(Vector2 position) { return Area.Contains(position); }

    ItemViewDiscritionBox discritionBox { set; get; }

    private void Awake()
    {
        Area = GMath.GetRect(GetComponent<RectTransform>());
        Icon = GetComponentInChildren<Image>();
        Name = GetComponentInChildren<Text>();

        Icon = Resources.Load<Image>("Item/" + ItemCounter.Data.ItemType + "/" + ItemCounter.Data.Name_eng);
        discritionBox = new ItemViewDiscritionBox(transform.Find("DiscriptionBox"), this);
        SetDiscriptionBox();
    }

    public void RefreshText() => Name.text = ItemCounter.Data.Name + " X" + ItemCounter.count;
    public void SetDiscriptionBox() => discritionBox.SetDiscription();
    public void ShowDiscriptionBox() => discritionBox.ShowDiscription();
    public void HideDiscriptionBox() => discritionBox.HideDiscriprtion();
    public void UseThis() => inventory.UseItem(this, false);
    public void SelectedIcon()
    {
        bool isTouch = false;
        int touchID = 0;
        bool isMouse = false;
        if (GPosition.IsContainInput(transform.GetComponent<RectTransform>(), out isTouch, out touchID, out isMouse))
        {
            StartCoroutine(TraceInput(isTouch, touchID, isMouse));
        }
    }

    IEnumerator TraceInput(bool isTouch, int touchID, bool isMouse)
    {
        Transform copy = Instantiate(gameObject).GetComponent<Transform>();
        gameObject.SetActive(false);

        bool isInputInOfIcon = true;

        while (GPosition.IsHoldPressedInput(isTouch, touchID, isMouse))
        {
            if (isInputInOfIcon) // showing discription.
            {
                if (GPosition.IsContainInput(Area, out isTouch, out touchID, out isMouse))
                {
                    ShowDiscriptionBox();
                }
                else
                {
                    isInputInOfIcon = false;
                    HideDiscriptionBox();
                }
            }
            else // follow
            {
                copy.position = isTouch ? (Vector3)Input.touches[touchID].position : Input.mousePosition;
                yield return new WaitForFixedUpdate();
            }
        }

        HideDiscriptionBox();
        inventory.ItemDrop(this, copy.position);
        Destroy(copy);
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
            Icon = transform.GetComponentInChildren<Image>();
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