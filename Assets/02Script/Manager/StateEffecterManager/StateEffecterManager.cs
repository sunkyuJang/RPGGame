using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using UnityEngine;

public class StateEffecterManager : MonoBehaviour
{
    public static StateEffecterSheet sheet;
    public static List<StateEffecterSheet.Param> data { set; get; }
    public static List<Transform> nowRunningList { set; get; } = new List<Transform>();

    private void Awake() => data = sheet.sheets[0].list;

    public static void EffectToModel(Transform transform, Model targetModel, bool isDeEffect)
    {
        int index = 0;
        int requestIndex = 0;
        bool isItem = false;

        ItemManager.ItemCounter nowItem = transform.GetComponent<ItemView>().ItemCounter;
        SkillManager.Skill nowSkill = transform.GetComponent<SkillManager.Skill>();

        if (nowItem != null)
        {
            index = nowItem.Data.Index;
            isItem = true;
            requestIndex = nowItem.Data.EffecterIndex;
        }
        else if(nowSkill != null)
        {
            index = nowSkill.data.Index;
            //it need Effecter index;
        }

        if (CompareIndexer(index, requestIndex, isItem)) 
            targetModel.SetEffecterToModel(data[index]);
        else { Debug.Log("Somthing Worng in stateEffecter "); }
    }
    static bool CompareIndexer(double index, double requestIndex, bool isItem)
    {
        return data[(int)index].RequestIndex == requestIndex &&
                (data[(int)index].RequestType == "Item" ? true : false) == isItem;
    }
}
