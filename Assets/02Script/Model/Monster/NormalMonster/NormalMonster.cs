using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonster : Monster
{
    protected NormalMonsterHPBarViewer hpBarScrip { private set; get; }
    public GameObject HpBarPositionGuide;
    new protected void OnEnable()
    {
        base.OnEnable();
        hpBarScrip = iStateViewerHandler.GetGameObject().GetComponent<NormalMonsterHPBarViewer>();
    }
    new protected void FixedUpdate()
    {
        base.FixedUpdate();
        if (hpBarScrip.CanShowingHPBar)
        {
            hpBarScrip.ResizingHPBar();
        }
    }
}
