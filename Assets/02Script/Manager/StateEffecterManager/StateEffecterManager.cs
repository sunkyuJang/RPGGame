using GLip;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using UnityEngine;

public class StateEffecterManager : MonoBehaviour
{
    public static StateEffecterManager instance { set; get; }
    public GameObject CharacterStateViewPrefab;
    public GameObject NormalMonsterStatViewPrefab;
    public GameObject BossMonsterStateViewPrefab;
    
    RectTransform StateViewGroupTransform { set; get; }

    public StateEffecterSheet sheet;
    public static List<StateEffecterSheet.Param> data { set; get; }
    public static List<Transform> nowRunningList { set; get; } = new List<Transform>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            data = sheet.sheets[0].list;

            StateViewGroupTransform = GPosition.GetNewRectTransformWithReset(GameManager.mainCanvas, "StateViewGroupTransform");
        }
    }

    public IStateViewerHandler GetStateView(Model model)
    {
        IStateViewerHandler stateViewer = null;
        if (model is Character)
        {
            var nowViewer = Instantiate(CharacterStateViewPrefab, StateViewGroupTransform).GetComponent<CharacterStateViewer>();
            nowViewer.Character = model as Character;
            stateViewer = nowViewer;
        }
        else if(model is NormalMonster)
        {
            var nowViewer = Instantiate(NormalMonsterStatViewPrefab, StateViewGroupTransform).GetComponent<NormalMonsterHPBarViewer>();
            nowViewer.Monster = model as NormalMonster;
            stateViewer = nowViewer;
        }
        else if(model is BossMonster)
        {
            var nowViewer = Instantiate(BossMonsterStateViewPrefab, StateViewGroupTransform).GetComponent<BossHPBarViewer>();
            nowViewer.BossMonster = model as BossMonster;
            stateViewer = nowViewer;
        }
        return stateViewer;
    }
    static void EffectToModel(int index, bool isDeEffect, Model targetModel)
    {
        /*        int coroutineIndex = 0;
                if (CoroutinForEffecter.IsAlreadyRunning(targetModel, data[index], out coroutineIndex))
                {
                    CoroutinForEffecter.StopRunningCoroutine(targetModel, coroutineIndex);
                }

                var coroutineEffecter = new CoroutinForEffecter(targetModel, data[index], isDeEffect);
                if (coroutineEffecter.data.During > 0) { targetModel.listForStateEffecter.Add(coroutineEffecter); }*/
        targetModel.SetEffect(data[index], isDeEffect);
    }
    public static void EffectToModelBySkill(SkillData skill, Model target, float damage)
    {
        if (skill.DamagePercentage != 0)
        {
            target.GetHit(damage, null, false);
        }

        if (skill.shouldLookAtEffectSheet)
        {
            EffectToModel(skill.EffectSheetNum, false, target);
        }
    }

    public static void EffectToModelByItem(ItemManager.ItemCounter counter, Model target, bool isDeEffect)
    {
        EffectToModel(counter.Data.EffecterIndex, isDeEffect, target);
    }


}
/*public class StateEffecterManager : MonoBehaviour
{
    public StateEffecterSheet sheet;
    public static List<StateEffecterSheet.Param> data { set; get; }
    public static List<Transform> nowRunningList { set; get; } = new List<Transform>();

    private void Awake() => data = sheet.sheets[0].list;
    static void EffectToModel(int index, int requestIndex, bool isItem, bool isDeEffect, Model targetModel)
    {
        if (CompareIndexer(index, requestIndex, isItem))
        {
            int coroutineIndex = 0;
            if (CoroutinForEffecter.IsAlreadyRunning(targetModel, data[requestIndex], out coroutineIndex))
            {
                CoroutinForEffecter.StopRunningCoroutine(targetModel, coroutineIndex);
            }

            var coroutineEffecter = new CoroutinForEffecter(targetModel, data[requestIndex], isDeEffect);
            if (coroutineEffecter.data.During > 0) { targetModel.listForStateEffecter.Add(coroutineEffecter); }
        }
        else { Debug.Log("Somthing Worng in stateEffecter "); }
    }

    static void EffectToModel(int index, bool isDeEffect, Model targetModel)
    {
        int coroutineIndex = 0;
        if (CoroutinForEffecter.IsAlreadyRunning(targetModel, data[index], out coroutineIndex))
        {
            CoroutinForEffecter.StopRunningCoroutine(targetModel, coroutineIndex);
        }

        var coroutineEffecter = new CoroutinForEffecter(targetModel, data[index], isDeEffect);
        if (coroutineEffecter.data.During > 0) { targetModel.listForStateEffecter.Add(coroutineEffecter); }
    }

    public static void EffectToModelBySkill(SkillManager.Skill skill, Model target, float damage, GameObject hitFX, bool isFXStartFromGround)
    {
        var isNpc = target is Npc;

        if (!isNpc)
        {
            if (skill.data.Damage_Percentage != 0 || skill.data.TargetTo == "Monster")
            {
                target.GetHit(damage, hitFX, isFXStartFromGround);
            }

            if (skill.data.StateEffecterIndex != 0)
            {
                EffectToModel(skill.data.Index, skill.data.StateEffecterIndex, false, false, target);
            }
        }
    }
    public static void EffectToModelBySkill(SkillData skill, Model target, float damage)
    {
        if (skill.DamagePercentage != 0)
        {
            target.GetHit(damage, null, false);
        }

        if (skill.shouldLookAtEffectSheet)
        {
            EffectToModel(skill.EffectSheetNum, false, target);
        }
    }

    public static void EffectToModelBySkill(Model target, float damage, GameObject hitFX, bool isFXStartFromGround)
    {
        var isNpc = target is Npc;

        if (!isNpc)
        {
            target.GetHit(damage, null, false);
        }
    }

    public static void EffectToModelByItem(Model targetModel, ItemManager.ItemCounter counter, bool isDeEffect)
    {
        EffectToModel(counter.Data.Index, counter.Data.EffecterIndex, true, isDeEffect, targetModel);
    }
    static bool CompareIndexer(double index, double requestIndex, bool isItem)
    {
        return data[(int)requestIndex].RequestIndex == index &&
                (data[(int)requestIndex].RequestType == "Item" ? true : false) == isItem;
    }

    protected void StartCoroutineForInnerClass(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
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
            foreach (CoroutinForEffecter coroutinForEffecter in model.listForStateEffecter)
            {
                if (coroutinForEffecter.data == data)
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
}*/