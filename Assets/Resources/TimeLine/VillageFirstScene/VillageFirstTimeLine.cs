using Cinemachine;
using System.Collections;
using UnityEngine;

public class VillageFirstTimeLine : TimeLineHandler
{
    public Npc npc;
    public CinemachineSmoothPath path;
    public CinemachineDollyCart cart;
    new private void Awake()
    {
        base.Awake();
    }

    public override IEnumerator StartSequence()
    {
        yield return new WaitUntil(() => cart.m_Position >= path.PathLength);

        DialogueManager.instance.ShowDialogue(Character, npc);

        yield return new WaitUntil(() => !DialogueManager.instance.DialogueViewer.gameObject.activeSelf);

        Character.LastTimeTalkingWith[npc.CharacterName]++;

        EndSequence();
    }
}
