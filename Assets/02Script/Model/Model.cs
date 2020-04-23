using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    public List<DialogueSheet.Param> dialogue { set; get; }
    public List<DialogueSheet.Param> selecter { set; get; }
    public Inventory Inventory { private set; get; }
    public Transform Transform { private set; get; }
    public Rigidbody Rigidbody { private set; get; }
    public Animator Animator { private set; get; }

    public bool isPlayer { protected set; get; }
    public int lastDialog { set; get; }
 
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

    protected int InventoryLength { set; get; }
    protected void SetInfo(string _CharacterName, int _HP, int _MP, int _ATK, int _DEF, int _SPD)
    {
        CharacterName = _CharacterName; HP = _HP; nowHP = HP; MP = _MP; nowMP = MP; ATK = _ATK; DEF = _DEF; SPD = _SPD;
    }

    protected void Awake()
    {
        Transform = gameObject.transform;
        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();

        //Rigidbody = transform.Find("CollsionObj").GetComponent<Rigidbody>();
    }

    protected void Start()
    {
        Inventory = Inventory.GetNew(InventoryLength, isPlayer);
        DialogueManager.GetScript(this);
    }
    public void GetTalk() 
    {
        DialogueManager .ShowDialogue(this);
    }
    public void ShowInventory() { Inventory.ShowInventory(); }
}
