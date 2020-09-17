using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSceneTimeLine : TimeLineHandler
{
    public GameObject bossMonster;

    protected override void ToDo()
    {
        playableDirector.Play();

        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        Character.DoAnimator(Character.AnimatorState.Battle);

        while (!IsInterruptOccur)
            yield return new WaitForFixedUpdate();

        playableDirector.Stop();

        DialogueManager.ShowDialogue(bossMonster.GetComponent<Model>());
        
        while (DialogueManager.DialogueViewer.gameObject.activeSelf)
            yield return new WaitForFixedUpdate();

        SetIsTimeLineStart(false);
    }
}
