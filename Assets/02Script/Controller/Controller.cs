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

    private Character Character { set; get; }
    private Joypad joypad;
    private CameraController cameraController;

    private bool isJoypadPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        joypad = joypadObj.GetComponent<Joypad>();
        cameraController = Camera.main.GetComponent<CameraController>();
    } 

    // Update is called once per frame
    void Update()
    {
        if (isJoypadPressed) { joypad.Pressed(); }
        Character.Move(isJoypadPressed, GetJoypadRadian);
        cameraController.Follow(Character.Transform.position);
    }
    public void joypadPressed() { isJoypadPressed = true; }
    public void JoypadUp() { isJoypadPressed = false; }
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
    public void PresscameraController() { cameraController.SetCenterTouch(); }
    public void DragcameraController() { cameraController.SetCamera(Character.Transform.position); }
    public void SetAllActive(bool active)
    {
        joypadObj.SetActive(active);
        actionKeyObj.SetActive(active);
        InventoryKeyObj.SetActive(active);
        pauseKeyObj.SetActive(active);
        EquipmentKeyObj.SetActive(active);
    }
    public float GetJoypadRadian { get { return joypad.radian + cameraController.Radian; } }

    public static Controller GetNew(Character character)
    {
        Controller controller = Create.GetNewInCanvas<Controller>();
        controller.Character = character;
        return controller;
    }
}
