using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;

public class ItemDescriptionBox : MonoBehaviour
{
    public GameObject frame;
    public Image icon;
    new public Text name;
    public Text description;
    public Vector2 StartPosition { set; get; } = new Vector2(-340f, 150f);
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show(ItemView itemView)
    {
        transform.SetAsLastSibling();
        frame.transform.localPosition = GPosition.GetInputPosition().x < Screen.width * 0.5f ? new Vector2(StartPosition.x * -1, StartPosition.y) : StartPosition;
        gameObject.SetActive(true);
        icon.sprite = itemView.icon.sprite;
        name.text = itemView.ItemCounter.Data.Name;
        description.text = "<" + itemView.ItemCounter.Data.ItemType + ">\r\n" + itemView.ItemCounter.Data.Description;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
