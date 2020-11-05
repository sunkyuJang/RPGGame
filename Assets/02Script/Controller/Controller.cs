using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;

public partial class Controller : MonoBehaviour
{
    public Character Character { set; get; }

    public GameObject BtnGroupObj;
    public MovePad joypad;
    public ActionPad actionpad;
    public QuickSlot quickSlot;
    public Button menuBtn;
    /*    public GameObject pauseKeyObj;
        public GameObject InventoryKeyObj;
        public GameObject EquipmentKeyObj;
        public GameObject skillTreeKeyObj;*/

    public GameObject CameraControllerPrefab;
    CameraController CameraController { set; get; }

    public CharacterSkiilViewer characterSkiilViewer;

    public bool isJoypadPressed = false;

    private void Awake()
    {
        CreatCameraController();
        BtnGroupObj.SetActive(true);
    }

    void Start()
    {
        joypad.controller = this;
        actionpad.controller = this;
    }

    private void FixedUpdate()
    {
        if (Character != null)
        {
            var characterPosition = Character.transform.position;
            //CameraController.transform.position = Character.transform.position;
            var yPosition = Mathf.Lerp(CameraController.transform.position.y, characterPosition.y, Time.fixedDeltaTime * 2f);
            CameraController.transform.position
                = new Vector3(characterPosition.x, yPosition, characterPosition.z); //Vector3.Lerp(CameraController.transform.position, Character.transform.position, Time.fixedDeltaTime + 2.5f);

        }
    }
    public void PressInventoryKey()
    {
        SetAllActive(false);
        Character.ShowGameUI(Character.UIList.inventory, true);
    }
    public void PressStateKey()
    {
        SetAllActive(false);
        Character.ShowGameUI(Character.UIList.equipment, true);
    }
    public void PressSkillTreeKey()
    {
        SetAllActive(false);
        Character.ShowGameUI(Character.UIList.skillViewer, true);
    }
    public void PressQuestKey()
    {
        SetAllActive(false);
        Character.ShowGameUI(Character.UIList.questViewer, true);
    }
    public void PressMenuBtn()
    {
        SetAllActive(false);
        Character.IntoClearUI();
        PlayerDataManager.instance.ShowView(this);
    }

    public void SetAllActive(bool active)
    {
        BtnGroupObj.SetActive(active);
    }
    void CreatCameraController()
    {
        CameraController = Instantiate(CameraControllerPrefab).GetComponent<CameraController>();
        DontDestroyOnLoad(CameraController.gameObject);
    }

    public void MoveCharacter(bool isPressed, float radian)
        => Character.Move(isPressed, radian + CameraController.GetRadianFromFront());
    public void RotateCamera(float raidan)
        => CameraController.RotateCamera(raidan);
    public void DoCharacterAtion()
        => Character.SetActionState(Character.ActionState.Action);

    public void ResetJoystick()
    {
        joypad.up.transform.position = joypad.down.transform.position;
        actionpad.up.transform.position = actionpad.down.transform.position;
    }
}
