using GLip;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEditor.Animations;
using UnityEngine;

public partial class Model : MonoBehaviour
{
    protected IStateViewerHandler iStateViewerHandler { private set; get; }
    bool isIStateObjOn { set; get; }
    //public GameObject inventoryPrefab;
    public ObjPooler InventoryPooler { private set; get; }
    public Inventory Inventory { private set; get; }
    public Transform Transform { private set; get; }
    public Rigidbody Rigidbody { private set; get; }
    public Animator Animator { private set; get; }
    public AnimatorStateInfo NowAnimatorInfo { get { return Animator.GetCurrentAnimatorStateInfo(0); } }

    public int lastDialog { set; get; }
    public DialogueSheet DialogueSheet;
    public List<DialogueSheet.Param> Dialogue { get { return DialogueSheet.sheets[0].list; } }
    public bool HasDialogue { get { return DialogueSheet != null; } }
    //state
    public string CharacterName { protected set; get; }
    public int HP { protected set; get; }
    public int nowHP { protected set; get; }
    public int MP { protected set; get; }
    public int nowMP { protected set; get; }
    public int ATK { protected set; get; }
    public int DEF { protected set; get; }
    public int SPD { protected set; get; }
    public bool isSuper { protected set; get; }

    public bool IsRunningTimeLine { set; get; }
    protected int InventoryLength { set; get; }
    protected RuntimeAnimatorController animatorController { set; get; }
    public SkillListHandler skillListHandler;

    protected string SkillDataName = "SkillData";
    protected void SetInfo(string _CharacterName, int _HP, int _MP, int _ATK, int _DEF, int _SPD)
    {
        CharacterName = _CharacterName; HP = _HP; nowHP = HP; MP = _MP; nowMP = MP; ATK = _ATK; DEF = _DEF; SPD = _SPD;
    }

    protected void Awake()
    {
        Transform = gameObject.transform;
        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
        if(Animator != null)
            animatorController = Animator.runtimeAnimatorController;

        AwakeInAlert();
    }

    protected void OnEnable()
    {
        iStateViewerHandler = StateEffecterManager.instance.GetStateView(this);
        StartCoroutine(SetInventoryFromInventoryPoolerManager());

        nowHP = HP;
        nowMP = MP;
        RefreshedHPBar();
    }

    protected void OnDisable()
    {
        StopAllCoroutines();
        Inventory.RemoveAllItem();
        InventoryPooler.returnObj(Inventory.gameObject);
    }
    IEnumerator SetInventoryFromInventoryPoolerManager()
    {
        print(Inventory == null);
        yield return new WaitUntil(() => InventoryPoolerManager.instance.inventoryPooler != null);
        InventoryPooler = InventoryPoolerManager.instance.inventoryPooler;
        Inventory = InventoryPooler.GetObj<Inventory>();
        print(Inventory.transform);
        Inventory.gameObject.SetActive(true);
        Inventory.gameObject.name = CharacterName + "Inventory";
        Inventory.SetTransformParent();
        Inventory.SetDefault(this);
    }
    public void SetTimeLineRunning(bool isRunning) 
    {
        if (isRunning)
        {
            Animator.runtimeAnimatorController = null;
        }
        else
        {
            Animator.runtimeAnimatorController = animatorController;
        }
    }

    protected void FixedUpdate()
    {
        if(iStateViewerHandler != null)
        {
            if(!IsRunningTimeLine != iStateViewerHandler.GetGameObject().activeSelf)
            {
                iStateViewerHandler.ShowObj(!IsRunningTimeLine);
            }
        }
    }

    protected void RefreshedHPBar() { if (iStateViewerHandler != null) iStateViewerHandler.RefreshState(); }

    public IEnumerator WaitTillAnimator(string name, bool isWaitforStart)
    {

        while (isWaitforStart ? !NowAnimatorInfo.IsName(name) 
            : NowAnimatorInfo.IsName(name)) 
            yield return new WaitForFixedUpdate();
    }
    public IEnumerator WaitTillTimeEnd(float time)
    {
        float elapsedTime = 0f;
        while(elapsedTime <= time)
        {
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
    protected int interrupt = 0;
    public void Interrupt(int i) { interrupt = i; }
    public IEnumerator WaitTillInterrupt(int n)
    {
        while (interrupt != n)
            yield return new WaitForFixedUpdate();
        interrupt = 0;
    }

    public void AddItem(int itemIndex, int count)
    {
        StartCoroutine(AddItemIntoInventory(itemIndex, count, 0));
    }
    public void AddItemWithGold(int itemIndex, int count, int gold)
    {
        StartCoroutine(AddItemIntoInventory(itemIndex, count, gold));
    }

    public void AddGold(int gold)
    {
        StartCoroutine(AddItemIntoInventory(-1, -1, gold));
    }

    int incven = 0;
    IEnumerator AddItemIntoInventory(int itemIndex, int count, int gold)
    {
        yield return new WaitUntil(() => Inventory != null);

        if(itemIndex >= 0)
            Inventory.AddItem(itemIndex, count);

        Inventory.AddGold(gold);
    }
}
