using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
public partial class Controller : MonoBehaviour
{
    public GameObject joypadObj;
    public GameObject actionpadObj;
    public GameObject pauseKeyObj;
    public GameObject InventoryKeyObj;
    public GameObject EquipmentKeyObj;
    public GameObject skillTreeKeyObj;

    private SkillManager skillManager { set; get; }
    public Character Character { set; get; }
    public Joypad joypad { private set; get; }
    public CameraController cameraController { private set; get; }

    public bool isJoypadPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        skillManager = transform.parent.Find("Managers").GetComponent<SkillManager>();
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
        skillManager.ShowSkillViewer();
    }

    public void SetAllActive(bool active)
    {
        joypadObj.SetActive(active);
        actionpadObj.SetActive(active);
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
