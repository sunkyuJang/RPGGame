using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using UnityEngine;

public class StateEffecterManager : MonoBehaviour
{
    public StateEffecterSheet sheet;
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
        {
            int coroutineIndex = 0;
            if(CoroutinForEffecter.IsAlreadyRunning(targetModel, data[requestIndex], out coroutineIndex))
            {
                CoroutinForEffecter.StopRunningCoroutine(targetModel, coroutineIndex);
            }

            targetModel.listForStateEffecter.Add(new CoroutinForEffecter(targetModel, data[requestIndex], isDeEffect));
        }
        else { Debug.Log("Somthing Worng in stateEffecter "); }
    }
    static bool CompareIndexer(double index, double requestIndex, bool isItem)
    {
        return data[(int)requestIndex].RequestIndex == index &&
                (data[(int)requestIndex].RequestType == "Item" ? true : false) == isItem;
    }

    public class CoroutinForEffecter
    {
        public StateEffecterSheet.Param data;
        public Coroutine coroutine;

        public int increasePercentageATK { set; get; }
        public int increasePercentageDEF { set; get; }
        public int increasePercentageSPD { set; get; }
        public float time = 0;
        public CoroutinForEffecter(StateEffecterSheet.Param data, Coroutine coroutine)
        {
            this.data = data; 
            this.coroutine = coroutine;
        }
        public CoroutinForEffecter(Model target, StateEffecterSheet.Param data, bool isDeEffect)
        {
            this.data = data;
            coroutine = StaticManager.coroutineStart(isDeEffect ? target.GetProcessDeincreaseEffect(data, this) : target.GetProcessIncreaseEffect(data, this));
        }

        public static bool IsAlreadyRunning(Model model, StateEffecterSheet.Param data, out int index)
        {
            index = 0;
            foreach(CoroutinForEffecter coroutinForEffecter in model.listForStateEffecter)
            {
                if(coroutinForEffecter.data == data)
                {
                    return true;
                }
                index++;
            }
            return false;
        }

        public static void StopRunningCoroutine(Model model, int index)
        {
            var nowCoroutineEffecter = model.listForStateEffecter[index];
            StaticManager.CorutineStop(nowCoroutineEffecter.coroutine);
            model.StopEffectToModel(nowCoroutineEffecter);
            RemoveCoroutine(model, index);
        }

        public static void RemoveCoroutine(Model model, int index) => model.listForStateEffecter.RemoveAt(index);
    }
}
