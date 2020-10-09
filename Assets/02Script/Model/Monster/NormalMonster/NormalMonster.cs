using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonster : Monster
{
    protected NormalMonsterHPBarViewer hpBarScrip { private set; get; }
    public GameObject HpBarPositionGuide;

    new protected void Awake()
    {
        base.Awake();

    }
    // Start is called before the first frame update
    new protected void Start()
    {
        base.Start();
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
