using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHPBarViewer : MonoBehaviour
{
    Monster Monster { set; get; }
    RectTransform RectTransform { set; get; }
    float originalWidth { set; get; }
    float originalHigth { set; get; }
    Image HPBar { set; get; }

    public float minDist { private set; get; }
    public float maxDist { private set; get; }
    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        originalWidth = RectTransform.rect.size.x;
        originalHigth = RectTransform.rect.size.y;

        minDist = 3.5f;
        maxDist = 20f;

        HPBar = GetComponent<Image>();
        HPBar.type = Image.Type.Filled;
        HPBar.fillMethod = Image.FillMethod.Horizontal;
        HPBar.fillOrigin = (int)Image.OriginHorizontal.Left;

    }

    public static MonsterHPBarViewer GetNew(Monster monster, Transform parent)
    {
        MonsterHPBarViewer viewer = Instantiate(Resources.Load<GameObject>("Monster/Slime/MonsterHPBarViewer"), parent).GetComponent<MonsterHPBarViewer>();
        viewer.Monster = monster;
        return viewer;
    }

    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(Monster.HPBarPositionGuide.position);
        if(transform.position.z < 10)
        {
            RectTransform.sizeDelta = new Vector2(originalWidth, originalHigth);
        }
        else if (transform.position.z >= 10f)
        {
            float rectSize = transform.position.z / maxDist;
            print(originalWidth - (originalWidth * rectSize));
            RectTransform.sizeDelta = new Vector2(originalWidth - (originalWidth * rectSize), originalHigth - (originalHigth * rectSize));
        }
        HPBar.fillAmount = Monster.nowHP / Monster.HP;
    }
}
