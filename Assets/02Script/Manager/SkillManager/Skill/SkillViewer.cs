using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GLip;
using JetBrains.Annotations;
using System.Globalization;
using System.Diagnostics;

public class SkillViewer : MonoBehaviour, IInputTracer
{
    RectTransform RectTransform;
    public SkillData skillData;
    public Image skillViewerImage;
    public CharacterSkiilViewer characterSkiilViewer;
    GameObject copy;

/*    GraphicRaycaster graphicRaycaster;
    PointerEventData ped;*/
    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        GetComponent<EventTrigger>().triggers[0].callback.AddListener((data) => Pressed());
        SetLearnedIcon();
    }

    public void MakeImage()
    {
        skillViewerImage.sprite = skillData.icon;

        copy = new GameObject(gameObject.name + "copy");
        copy.transform.parent = transform;
        copy.AddComponent<Image>().sprite = skillViewerImage.sprite;
        copy.transform.position = transform.position;
        copy.SetActive(false);
    }

    public void SetLearnedIcon()
    {
        if (skillData.isLearn)
            skillViewerImage.color = new Color(255f, 255f, 255f, 1f);
        else
            skillViewerImage.color = new Color(255f, 255f, 255f, 0.5f);
    }
    public void Pressed() 
    {
        bool isTouch;
        int touchId = 0;
        bool isMouse;
        if (GPosition.IsContainInput(RectTransform, out isTouch, out touchId, out isMouse))
        {
            characterSkiilViewer.ShowDescription(this);
            if(skillData.isLearn)
                StartCoroutine(TraceInput(isTouch, touchId, isMouse));
        }
        else
            print("somting Wrong in SkillManager_SelectedIcon");
    }

    public IEnumerator TraceInput(bool isTouch, int touchId, bool isMouse)
    {
        copy.SetActive(true);
        copy.transform.SetParent(GameManager.mainCanvas);

        //List<RaycastResult> results = new List<RaycastResult>();

        while(GPosition.IsHoldPressedInput(isTouch, touchId, isMouse))
        {
            copy.transform.position = GPosition.GetInputPosition(isTouch, touchId, isMouse);
            yield return new WaitForFixedUpdate();
        }

        /*        ped.position = copy.transform.position;
                graphicRaycaster.Raycast(ped, results);
                foreach(RaycastResult result in results)
                {
                    print(result.gameObject.name);
                }*/

        var quickSlot = characterSkiilViewer.character.QuickSlot;
        var quickSlotNum = quickSlot.IsIn(copy.transform.position);
        if(quickSlotNum >= 0) { quickSlot.SetSlot(transform, quickSlotNum); }

        copy.transform.SetParent(transform);
        copy.transform.position = transform.position;
        copy.SetActive(false);

        yield return null;
    }
}