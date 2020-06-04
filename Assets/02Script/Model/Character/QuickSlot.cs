using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;

public class QuickSlot : MonoBehaviour
{
    Character Character { set; get; }
    private new Transform transform;
    private Rect area;
    private List<Transform> lists = new List<Transform>();
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
        Transform nowSlot = lists[_num];
        ItemView itemView = nowSlot.GetComponent<ItemView>();
        SkillManager.Skill.SkillViewer skillViewer = nowSlot.GetComponent<SkillManager.Skill.SkillViewer>();
        if (itemView != null)
        {
            itemView.UseItem(false);
            if(itemView.ItemCounter.Count <= 0) 
            {
                Inventory inventory = itemView.Inventory;
                SetSlot(inventory.GetHeadItemView(itemView).transform, _num);
            }
        }
        else if(skillViewer != null)
        {
            Character.ActivateSkill(skillViewer.Skill);
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

    public static QuickSlot GetNew(Character character)
    {
        QuickSlot quickSlot = Create.GetNewInCanvas<QuickSlot>();
        quickSlot.Character = character;
        quickSlot.gameObject.SetActive(false);
        return quickSlot;
    }
}
