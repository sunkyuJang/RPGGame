using GLip;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEditor.Animations;
using UnityEngine;

public partial class Model : MonoBehaviour
{
    public GameObject HPBar;
    protected IStateViewerHandler iStateViewerHandler { private set; get; }
    bool isIStateObjOn { set; get; }
    //public GameObject inventoryPrefab;
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

    public bool IsRunningTimeLine { get { return StaticManager.IsRunningTimeLine; } }
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
        StartCoroutine(SetInventoryFromInventoryPoolerManager());
    }

    protected void Start()
    {
        if (HPBar != null)
        {
            HPBar = Instantiate(HPBar, GameManager.mainCanvas);
            iStateViewerHandler = HPBar.GetComponent<IStateViewerHandler>();
        }
    }

    protected void OnDisable()
    {
        StopAllCoroutines();
    }
    IEnumerator SetInventoryFromInventoryPoolerManager()
    {
        yield return new WaitUntil(() => InventoryPoolerManager.instance.inventoryPooler != null);
        var inventoryPooler = InventoryPoolerManager.instance.inventoryPooler;
        yield return StartCoroutine(inventoryPooler.CheckCanUseObj());
        Inventory = inventoryPooler.GetObj<Inventory>();
        Inventory.gameObject.SetActive(true);
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
            if(!IsRunningTimeLine != HPBar.gameObject.activeSelf)
            {
                iStateViewerHandler.ShowObj(!IsRunningTimeLine);
            }
        }
    }

    protected void RefreshedHPBar() { if (HPBar != null) iStateViewerHandler.RefreshState(); }

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
}
