using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBarViewer : MonoBehaviour, IStateViewerHandler
{
    public GameObject NameObj;
    public GameObject HPHarObj;

    public BossMonster BossMonster { set; get; }
    Image HPBar { set; get; }
    Text NameText { set; get; }

    private void Awake()
    {
        HPBar = HPHarObj.GetComponent<Image>();
        HPBar.type = Image.Type.Filled;
        HPBar.fillMethod = Image.FillMethod.Horizontal;
        HPBar.fillOrigin = (int)Image.OriginHorizontal.Left;

        NameText = NameObj.GetComponent<Text>();
    }

    public void SetName(string name) => NameText.text = name;
    public void RefreshState()
    {
        HPBar.fillAmount = (float)BossMonster.nowHP / (float)BossMonster.HP;
    }

    public void ShowObj(bool shouldShow)
    {
        if (shouldShow) 
            NameText.text = BossMonster.CharacterName;
        gameObject.SetActive(shouldShow);
    }
}
