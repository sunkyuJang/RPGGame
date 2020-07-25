using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
public partial class Controller : MonoBehaviour
{
    public GameObject joypadObj;
    public GameObject actionKeyObj;
    public GameObject pauseKeyObj;
    public GameObject InventoryKeyObj;
    public GameObject EquipmentKeyObj;
    public GameObject skillTreeKeyObj;

    private SkillManager skillManager { set; get; }
    private Character Character { set; get; }
    private Joypad joypad;
    private CameraController cameraController;

    public bool isJoypadPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        Character = StaticManager.Character;
        skillManager = transform.parent.Find("Managers").GetComponent<SkillManager>();
        joypad = joypadObj.GetComponent<Joypad>();
        cameraController = Camera.main.GetComponent<CameraController>();
    } 

    // Update is called once per frame
    void Update()
    {
        if (joypad.isPressed) { joypad.Pressed(); }
        Character.Move(joypad.isPressed, GetJoypadRadian);
    }
    public void PressActionKey() { Character.SetActionState(Character.ActionState.Action); }
    public void PressInventoryKey()
    {
        SetAllActive(false);
        Character.ShowInventory();
    }
    public void PressStateKey() 
    { 
        SetAllActive(false);
        Character.EquipmentView.gameObject.SetActive(true);
    }
    public void PressSkillTreeKey()
    {
        SetAllActive(false);
        skillManager.ShowSkillViewer();
    }

    public void SetAllActive(bool active)
    {
        joypadObj.SetActive(active);
        actionKeyObj.SetActive(active);
        InventoryKeyObj.SetActive(active);
        pauseKeyObj.SetActive(active);
        EquipmentKeyObj.SetActive(active);
        skillTreeKeyObj.SetActive(active);
    }
    public float GetJoypadRadian { get { return joypad.radian/* + cameraController.Radian*/; } }

    public static Controller GetNew(Character character)
    {
        Controller controller = Create.GetNewInCanvas<Controller>();
        controller.transform.SetAsFirstSibling();
        controller.Character = character;
        return controller;
    }
}
