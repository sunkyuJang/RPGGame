﻿using System.Collections;
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

        foreach (TimeLineAnimationController controller in timeLineAnimationControllers)
            controller.LoopLastAnimation(true);

        playableDirector.Stop();

        DialogueManager.instance.ShowDialogue(Character, bossMonster.GetComponent<Model>());
        
        while (DialogueManager.instance.DialogueViewer.gameObject.activeSelf)
            yield return new WaitForFixedUpdate();

        SetIsTimeLineStart(false);
    }
}
