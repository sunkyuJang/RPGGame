using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHPBarViewer : MonoBehaviour
{
    Monster Monster { set; get; }
    RectTransform RectTransform { set; get; }
    float OriginalWidth { set; get; }
    float OriginalHigth { set; get; }
    Image HPBar { set; get; }

    public float MinDist { get { return 3.5f; } }
    public float DiminishingDist { get { return 10f; } }
    public float MaxDist { get { return 20f; } }
    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        OriginalWidth = RectTransform.rect.size.x;
        OriginalHigth = RectTransform.rect.size.y;

        HPBar = GetComponent<Image>();
        HPBar.type = Image.Type.Filled;
        HPBar.fillMethod = Image.FillMethod.Horizontal;
        HPBar.fillOrigin = (int)Image.OriginHorizontal.Left;
    }

    public static MonsterHPBarViewer GetNew(Monster monster, Transform parent)
    {
        MonsterHPBarViewer viewer = Instantiate(Resources.Load<GameObject>("Monster/MonsterHPBarViewer"), parent).GetComponent<MonsterHPBarViewer>();
        viewer.Monster = monster;
        return viewer;
    }

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(Monster.HPBarPositionGuide.position);
        if (transform.position.z >= DiminishingDist)
        {
            float rectSize = (transform.position.z - DiminishingDist) / (MaxDist - DiminishingDist);
            RectTransform.sizeDelta = new Vector2(OriginalWidth - (OriginalWidth * rectSize), OriginalHigth - (OriginalHigth * rectSize));
        }
        HPBar.fillAmount = (float)Monster.nowHP / (float)Monster.HP;
    }
}
