using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventroyDiscription : MonoBehaviour
{
    Image icon;
    Text name;
    Text discription;

    public void Show(ItemView itemView)
    {
        gameObject.SetActive(true);
        icon.sprite = itemView.Icon.sprite;
        name.text = itemView.ItemCounter.Data.Name;
        discription.text = itemView.ItemCounter.Data.Description;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
