using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GLip;
public class SkillViewer : MonoBehaviour
{
    public SkillData skillData;
    public Image skillViewerImage;
    private void Awake()
    {
        skillViewerImage.sprite = skillData.icon;
        GetComponent<EventTrigger>().triggers[0].callback.AddListener((data) => PressedIcon());
        SetLearnedIcon(skillData.isLearn);
    }

    void SetLearnedIcon(bool isLearned)
    {
        if (isLearned)
            skillViewerImage.color = new Color(255f, 255f, 255f, 1f);
        else
            skillViewerImage.color = new Color(255f, 255f, 255f, 0.5f);
    }
    public void PressedIcon() 
    {
        /*bool isTouch;
        int touchId = 0;
        bool isMouse;
        if (GPosition.IsContainInput(GetComponent<RectTransform>(), out isTouch, out touchId, out isMouse))
        {
            StartCoroutine(TraceInput(viewer, isTouch, touchId, isMouse));
        }
        else
            print("somting Wrong in SkillManager_SelectedIcon");*/

        skillData.ActivateSkill();
    }

/*    public IEnumerator TraceInput()
    {

    }*/
}
