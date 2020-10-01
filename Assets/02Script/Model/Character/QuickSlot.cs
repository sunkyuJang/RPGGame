using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;

public class QuickSlot : MonoBehaviour
{
    public Character Character { set; get; }
    private new Transform transform;
    private Rect area;
    private List<Transform> lists = new List<Transform>();
    private List<Rect> childArea = new List<Rect>();
    private List<Transform> child = new List<Transform>();
    Sprite recoverIcon { set; get; }
    public bool IsActive { set; get; }

    private void Awake()
    {
        transform = gameObject.transform;
        area = GMath.GetRect(gameObject.GetComponent<RectTransform>());
        recoverIcon = transform.GetChild(0).GetComponent<Image>().sprite;
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

    private void DoAction(int num)
    {
        Transform nowSlot = lists[num];

        if (nowSlot != null)
        {
            ItemView itemView = nowSlot.GetComponent<ItemView>();
            SkillViewer skillViewer = nowSlot.GetComponent<SkillViewer>();

            if (itemView != null)
            {
                ItemSheet.Param data = itemView.ItemCounter.Data;
                Inventory inventory = itemView.inventory;
                itemView.UseThis();

                if (itemView.ItemCounter == null)
                {
                    var kind = inventory.table.GetSameKind(data);
                    if (kind == null)
                    {
                        child[num].GetComponent<Image>().sprite = recoverIcon;
                        lists[num] = null;
                    }
                    else
                    {
                        lists[num] = kind[kind.Count - 1].View.transform;
                    }
                }
            }
            else if (skillViewer != null)
            {
                Character.ReservedSkill = skillViewer.skillData;
                Character.SetActionState(Character.ActionState.Attack);
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
    public void SetSlot(Transform targetTrasform, int _num) 
    {
        lists.RemoveAt(_num);
        lists.Insert(_num, targetTrasform);

        child[_num].GetComponent<Image>().sprite
            = targetTrasform.GetComponent<ItemView>() != null ? targetTrasform.GetChild(0).GetComponent<Image>().sprite
            : targetTrasform.GetComponent<Image>().sprite;
    }

    public void TurnOn(bool isOn)
    {
        if (IsActive)
        {
            gameObject.SetActive(isOn);
        }
    }
}
