using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealizeNpc : Npc
{
    public TimeLineHandler OpenOtherScene;

    new protected void Awake()
    {
        base.Awake();
        SetInfo("수상한 남자", 10, 10, 10, 10, 10);
    }
    private void Update()
    {
        if (lastDialog == 25)
            if (!OpenOtherScene.gameObject.activeSelf)
            {
                OpenOtherScene.gameObject.SetActive(true);
            }
    }
}
