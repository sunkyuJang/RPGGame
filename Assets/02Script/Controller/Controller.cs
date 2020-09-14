using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
public partial class Controller : MonoBehaviour
{
    public static Controller instance;

    public GameObject BtnGroupObj;
    public MovePad joypad;
    public ActionPad actionpad;
    public GameObject pauseKeyObj;
    public GameObject InventoryKeyObj;
    public GameObject EquipmentKeyObj;
    public GameObject skillTreeKeyObj;

    public CharacterSkiilViewer characterSkiilViewer;
    public Character Character { set; get; }
    public CameraController cameraController { private set; get; }

    public bool isJoypadPressed = false;

    private void Awake()
    {
        instance = this;
        BtnGroupObj.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        Character = StaticManager.Character;
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
    public float GetJoypadRadian { get { return joypad.radian/* + cameraController.Radian*/; } }

    public static Controller GetNew(Character character)
    {
        Controller controller = Create.GetNewInCanvas<Controller>();
        controller.transform.SetAsFirstSibling();
        controller.Character = character;
        return controller;
    }
}
