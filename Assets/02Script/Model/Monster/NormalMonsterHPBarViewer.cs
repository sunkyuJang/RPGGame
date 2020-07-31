using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NormalMonsterHPBarViewer : MonoBehaviour, IStateViewerHandler
{
    public NormalMonster Monster { set; get; }
    RectTransform RectTransform { set; get; }
    float OriginalWidth { set; get; }
    float OriginalHigth { set; get; }
    Image HPBar { set; get; }

    public float MinDist { get { return 3.5f; } }
    public float DiminishingDist { get { return 10f; } }
    public float MaxDist { get { return 20f; } }
    private void Awake()
    {
        RectTransform = transform.GetComponent<RectTransform>();
        OriginalWidth = RectTransform.rect.size.x;
        OriginalHigth = RectTransform.rect.size.y;

        HPBar = GetComponent<Image>();
        HPBar.type = Image.Type.Filled;
        HPBar.fillMethod = Image.FillMethod.Horizontal;
        HPBar.fillOrigin = (int)Image.OriginHorizontal.Left;
    }

    private void FixedUpdate()
    {
        //ResizingHPBar();
    }

    public bool CanShowingHPBar
    {
        get
        {
            Vector3 HPBarPositionToScreen = Camera.main.WorldToScreenPoint(Monster.HpBarPositionGuide.transform.position);
            if (MinDist <= HPBarPositionToScreen.z && HPBarPositionToScreen.z < MaxDist)
            {
                return true;
            }

            return false;
        }
    }

    public void ResizingHPBar()
    {
        transform.position = Camera.main.WorldToScreenPoint(Monster.HpBarPositionGuide.transform.position);
        print(transform.position);
        if (transform.position.z >= DiminishingDist)
        {
            float rectSize = (transform.position.z - DiminishingDist) / (MaxDist - DiminishingDist);
            RectTransform.sizeDelta = new Vector2(OriginalWidth - (OriginalWidth * rectSize), OriginalHigth - (OriginalHigth * rectSize));
        }
        print(transform.position + "true");
    }

    void IStateViewerHandler.RefreshState() => HPBar.fillAmount = (float)Monster.nowHP / (float)Monster.HP;

    void IStateViewerHandler.ShowObj(bool souldShow) { }
}
