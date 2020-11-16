using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;

public class QuickSlot : MonoBehaviour
{
    public Character Character { set; get; }
    private new Transform transform;
    private List<Transform> lists = new List<Transform>();
    private List<RectTransform> childs = new List<RectTransform>();
    Sprite recoverIcon { set; get; }
    public bool IsActive { set; get; }

    private void Awake()
    {
        transform = gameObject.transform;
        recoverIcon = transform.GetChild(0).GetComponent<Image>().sprite;
        for (int i = 0; i < 5; i++)
        {
            Transform nowChild = transform.GetChild(i);
            lists.Add(null);
            childs.Add(nowChild.GetComponent<RectTransform>());
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
                StartCoroutine(CountingItemCoolDown(num, itemView));

                if (itemView.ItemCounter == null)
                {
                    var kind = inventory.table.GetSameKind(data);
                    if (kind == null)
                    {
                        childs[num].GetComponent<Image>().sprite = recoverIcon;
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
                StartCoroutine(CountingSkillCoolDown(num, skillViewer.skillData));
            }
        }
    }

    IEnumerator CountingSkillCoolDown(int slotNum, SkillData data)
    {
        yield return new WaitWhile(() => data.nowCoolDown == 0);
        var icon = childs[slotNum].GetComponent<Image>();
        icon.fillAmount = 0f;
        while (data.isCoolDown)
        {
            yield return new WaitForFixedUpdate();
            icon.fillAmount = (data.CoolDown - data.nowCoolDown) / data.CoolDown;
        }
    }

    public IEnumerator CountingItemCoolDown(int slotNum, ItemView view)
    {
        var icon = childs[slotNum].GetComponent<Image>();
        Model.StateSaver saver = null;
        if (view.ItemCounter.Data.EffecterIndex >= 0)
        {
            while (saver == null)
            {
                yield return new WaitForFixedUpdate();
                foreach (Model.StateSaver nowSaver in Character.StateSavers)
                {
                    if (nowSaver.data.Index == view.ItemCounter.Data.EffecterIndex)
                    {
                        saver = nowSaver;
                        break;
                    }
                }
            }
        }

        while (saver != null)
        {
            yield return new WaitForFixedUpdate();
            icon.fillAmount = (float)((saver.data.During - (double)saver.time) / saver.data.During);
        }

        icon.fillAmount = 1;
    }


    public int IsIn(Vector2 _position)
    {
        //area.Contains(_position)
        var rect = GMath.GetRect(transform.GetComponent<RectTransform>());
        if (rect.Contains(_position))
        {
            for (int i = 0; i < childs.Count; i++)
            {
                if (GMath.GetRect(childs[i]).Contains(_position))
                {
                    return i;
                }
            }
        }
        return -1;
    }
    public void SetSlot(Transform targetTrasform, Sprite icon, int _num)
    {
        lists.RemoveAt(_num);
        lists.Insert(_num, targetTrasform);
        childs[_num].GetComponent<Image>().sprite = icon;
    }

    public void TurnOn(bool isOn)
    {
        if (IsActive)
        {
            gameObject.SetActive(isOn);
        }
    }

    private void OnEnable()
    {
        int i = 0;
        foreach (Transform nowTransform in childs)
        {
            SkillViewer viewer = nowTransform.GetComponent<SkillViewer>();
            if (viewer != null)
            {
                StartCoroutine(CountingSkillCoolDown(i, viewer.skillData));
            }

            ItemView itemView = nowTransform.GetComponent<ItemView>();
            if (itemView != null)
            {
                StartCoroutine(CountingItemCoolDown(i, itemView));
            }

            i++;

        }
    }
}
