using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class StateEffecter : MonoBehaviour
{
    public StateEffecterSheet sheet;
    public List<StateEffecterSheet.Param> data { set; get; }
    Model model { set; get; }

    private void Awake()
    {
        data = sheet.sheets[0].list;
    }

    public StateEffecterSheet.Param EffectToModel(Transform transform, bool isDeEffect)
    {
        int index = 0;
        int requestIndex = 0;
        bool isItem = false;

        ItemManager.Item nowItem = transform.GetComponent<ItemManager.Item>();
        SkillManager.Skill nowSkill = transform.GetComponent<SkillManager.Skill>();

        if (nowItem != null)
        {
            index = nowItem.Index;
            isItem = true;
            //it need Effecter index;
        }
        else if(nowSkill != null)
        {
            index = nowSkill.data.Index;
        }

        if (CompareIndexer(index, requestIndex, isItem)) return data[index];
        return null;
    }
    bool CompareIndexer(double index, double requestIndex, bool isItem)
    {
        return data[(int)index].RequestIndex == requestIndex &&
                (data[(int)index].RequestType == "Item" ? true : false) == isItem;
    }
    StateEffecter GetNew (Model model)
    {
        StateEffecter effecter = new StateEffecter();
        effecter.model = model;

        return effecter;
    }
}
