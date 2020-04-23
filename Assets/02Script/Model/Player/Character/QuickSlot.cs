using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;

public class QuickSlot : MonoBehaviour
{
    private new Transform transform;
    private Rect area;
    private List<MonoBehaviour> lists = new List<MonoBehaviour>();
    private List<Rect> childArea = new List<Rect>();
    private List<Transform> child = new List<Transform>();
    public bool IsActive { set; get; }

    private void Awake()
    {
        transform = gameObject.transform;
        area = GMath.GetRect(gameObject.GetComponent<RectTransform>());
        for (int i = 0; i < 5; i++)
        {
            Transform nowChild = transform.GetChild(i);
            lists.Add(null);
            childArea.Add(GMath.GetRect(nowChild.GetComponent<RectTransform>()));
            child.Add(nowChild);
        }
    }

    public void PressSlot1() { DoAction(0); }
    public void PressSlot2() { DoAction(1); }
    public void PressSlot3() { DoAction(2); }
    public void PressSlot4() { DoAction(3); }
    public void PressSlot5() { DoAction(4); }

    private void DoAction(int _num)
    {
        MonoBehaviour nowSlot = lists[_num];
        if(nowSlot is ItemView)
        {
            ItemView itemView = nowSlot as ItemView;
            itemView.UseItem(false);
            if(itemView.ItemCounter.Count <= 0) 
            {
                Inventory inventory = itemView.Inventory;
                SetSlot(inventory.GetHeadItemView(itemView), _num);
            }
        }
    }

    public int IsIn(Vector2 _position)
    {
        if (area.Contains(_position))
        {
            for (int i = 0; i < childArea.Count; i++)
            {
                if (childArea[i].Contains(_position))
                {
                    return i;
                }
            }
        }
        return -1;
    }
    public void SetSlot(MonoBehaviour _monoBehaviour, int _num) 
    {
        lists.RemoveAt(_num);
        lists.Insert(_num, _monoBehaviour);
        child[_num].GetComponent<Image>().sprite
            = _monoBehaviour == null ? null : _monoBehaviour.gameObject.transform.GetChild(0).GetComponent<Image>().sprite;
    }

    public void TurnOn(bool isOn)
    {
        if (IsActive)
        {
            gameObject.SetActive(isOn);
        }
    }

    public static QuickSlot GetNew()
    {
        QuickSlot quickSlot = Create.GetNew<QuickSlot>();
        quickSlot.gameObject.SetActive(false);
        return quickSlot;
    }
}
