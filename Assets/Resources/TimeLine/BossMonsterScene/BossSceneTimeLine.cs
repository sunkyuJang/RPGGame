using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSceneTimeLine : TimeLineHandler
{
    public Monster bossMonster;

    int count = 0;
    protected override void ToDo()
    {
        playableDirector.Play();
        SetRunningTImeline(true);
        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        while (!IsInterruptOccur)
            yield return new WaitForFixedUpdate();

        playableDirector.Stop();
        //print(StaticManager.Character.IsRunningTimeLine);

        //StaticManager.Character.DoAnimator(Character.AnimatorState.Battle);
        SetRunningTImeline(false);
        Character.DoAnimator(Character.AnimatorState.Battle);

        DialogueManager.ShowDialogue(bossMonster);
        
        while (DialogueManager.DialogueViewer.gameObject.activeSelf)
            yield return new WaitForFixedUpdate();


        yield return new WaitForFixedUpdate();

    }
}
