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

    public bool CanShowingHPBar
    {
        get
        {
            Vector3 HPBarPositionToScreen = Camera.main.WorldToScreenPoint(Monster.HpBarPositionGuide.transform.position);
            if (MinDist <= HPBarPositionToScreen.z && HPBarPositionToScreen.z < MaxDist)
            {
                if (!gameObject.activeSelf)
                    gameObject.SetActive(true);
                return true;
            }

            gameObject.SetActive(false);
            return false;
        }
    }

    public void ResizingHPBar()
    {
        var nowPosition = Camera.main.WorldToScreenPoint(Monster.HpBarPositionGuide.transform.position);
        transform.position = nowPosition;
        if (nowPosition.z >= DiminishingDist)
        {
            float rectSize = (nowPosition.z - DiminishingDist) / (MaxDist - DiminishingDist);
            RectTransform.sizeDelta = new Vector2(OriginalWidth - (OriginalWidth * rectSize), OriginalHigth - (OriginalHigth * rectSize));
        }
    }

    void IStateViewerHandler.RefreshState() => HPBar.fillAmount = (float)Monster.nowHP / (float)Monster.HP;

    void IStateViewerHandler.ShowObj(bool souldShow) { gameObject.SetActive(souldShow); }

    GameObject IStateViewerHandler.GetGameObject()
    {
        return gameObject;
    }
}
