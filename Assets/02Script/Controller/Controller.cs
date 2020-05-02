using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
public partial class Controller : MonoBehaviour
{
    public GameObject playerObj;
    public GameObject cameraControllerObj;
    public GameObject joypadObj;
    public GameObject actionKeyObj;
    public GameObject pauseKeyObj;
    public GameObject InventoryKeyObj;
    public GameObject EquipmentKeyObj;

    private Player player;
    private Joypad joypad;
    private CameraController cameraController;

    private bool isJoypadPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        player = playerObj.GetComponent<Player>();
        joypad = joypadObj.GetComponent<Joypad>();
        cameraController = cameraControllerObj.GetComponent<CameraController>();
    } 

    // Update is called once per frame
    void Update()
    {
        if (isJoypadPressed) { joypad.Pressed(); }
        player.SetMove(isJoypadPressed, GetJoypadRadian);
        cameraController.Follow(player.Character.Transform.position);
    }
    public void joypadPressed() { isJoypadPressed = true; }
    public void JoypadUp() { isJoypadPressed = false; }
    public void PressActionKey() { player.GetAction(); }
    public void PressInventoryKey()
    {
        SetAllActive(false);
        player.ShowInventoryView();
    }
    public void PressStateKey() 
    { 
        SetAllActive(false);
        player.Character.EquipmentView.gameObject.SetActive(true);
    }
    public void PresscameraController() { cameraController.SetCenterTouch(); }
    public void DragcameraController() { cameraController.SetCamera(player.Character.Transform.position); }
    public void SetAllActive(bool active)
    {
        joypadObj.SetActive(active);
        actionKeyObj.SetActive(active);
        InventoryKeyObj.SetActive(active);
        pauseKeyObj.SetActive(active);
        EquipmentKeyObj.SetActive(active);
    }
    public float GetJoypadRadian { get { return joypad.radian + cameraController.Radian; } }
}
