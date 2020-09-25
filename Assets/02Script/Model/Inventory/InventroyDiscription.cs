using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventroyDiscription : MonoBehaviour
{
    public Image icon;
    public Text name;
    public Text discription;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show(ItemView itemView)
    {
        gameObject.SetActive(true);
        icon.sprite = itemView.icon.sprite;
        name.text = itemView.ItemCounter.Data.Name;
        discription.text = itemView.ItemCounter.Data.Description;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
