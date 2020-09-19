using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
public partial class Controller : MonoBehaviour
{
    public GameObject CharacterPrefab;
    Character Character { set; get; }

    public GameObject BtnGroupObj;
    public MovePad joypad;
    public ActionPad actionpad;
    public GameObject pauseKeyObj;
    public GameObject InventoryKeyObj;
    public GameObject EquipmentKeyObj;
    public GameObject skillTreeKeyObj;

    public GameObject CameraControllerPrefab;
    CameraController CameraController { set; get; }

    public CharacterSkiilViewer characterSkiilViewer;

    public bool isJoypadPressed = false;

    private void Awake()
    {
        Character = Instantiate(CharacterPrefab).GetComponent<Character>();
        Character.controller = this;
        CreatCameraController();
        BtnGroupObj.SetActive(true);
    }

    private void FixedUpdate()
    {
        CameraController.transform.position = Character.transform.position;
    }
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
        characterSkiilViewer.skillTreeViewer.SetActive(true);
    }

    public void SetAllActive(bool active)
    {
        BtnGroupObj.SetActive(active);
    }
    void CreatCameraController()
    {
        CameraController = Instantiate(CameraControllerPrefab).GetComponent<CameraController>();
    }
    
    public void MoveCharacter(bool isPressed, float radian) 
        => Character.Move(isPressed, radian + CameraController.GetRadianFromFront());
    public void RotateCamera(float raidan) 
        => CameraController.RotateCamera(raidan);
    public void DoCharacterAtion()
        => Character.SetActionState(Character.ActionState.Action);
}
