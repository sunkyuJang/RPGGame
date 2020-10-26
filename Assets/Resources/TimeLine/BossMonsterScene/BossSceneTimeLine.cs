using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSceneTimeLine : TimeLineHandler
{
    public BossMonster bossMonster;

    new protected void Awake() { base.Awake(); }

    public override IEnumerator StartSequence()
    {
        while (!IsInterruptOccur)
            yield return new WaitForFixedUpdate();

        DialogueManager.instance.ShowDialogue(Character, bossMonster.GetComponent<Model>());

        while (DialogueManager.instance.DialogueViewer.gameObject.activeSelf)
        {
            yield return new WaitForFixedUpdate();
            if (IsInterruptOccur)
            {
                playableDirector.time = 7.5f;
                interrupt = 0;
            }
        }

        bossMonster.transform.parent = transform.parent;
        SetIsTimeLineStart(false);
    }
}
