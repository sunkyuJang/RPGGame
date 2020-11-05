using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenOtherSceneTimeLine : TimeLineHandler
{
    public GameObject fence;
    public override IEnumerator StartSequence()
    {
        yield return new WaitUntil(() => interrupt == 1);

        //fence.transform.Rotate(new Vector3(0.825f, 420, 358.512f));

        Character.PassedTimeLineAssetName.Remove(gameObject.name);

        yield return new WaitUntil(() =>
            !DialogueManager.instance.DialogueViewer.gameObject.activeSelf);

        EndSequence();
    }
}
