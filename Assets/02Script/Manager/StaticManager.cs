using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class StaticManager : MonoBehaviour
{
    public GameObject characterObj;
    public GameObject CameraControllerObj;
    public static Character Character { private set; get; }
    private static StaticManager staticManager;
    public static StaticManager GetStaticManager { get { return staticManager; } }

    static TimeLineHandler RunningTimeLine = null;

    public static CharacterSkiilViewer CharacterSkiilViewer;

    public static Transform CharacterSkillPulling;
    public static Transform MonsterSkillPulling;
    public static void SetRunningTimeLine(TimeLineHandler timeLineHandler)
        => RunningTimeLine = timeLineHandler;

    public static bool IsRunningTimeLine { get { return RunningTimeLine != null; } }
    void Awake()
    {
        Character = characterObj.GetComponent<Character>();
        staticManager = this;
        CharacterSkiilViewer = transform.Find("CharacterSkillViewer").GetComponent<CharacterSkiilViewer>();

        CharacterSkillPulling = new GameObject("CharaceterSkillPulling").GetComponent<Transform>();
        MonsterSkillPulling = new GameObject("MonsterSkillPulling").GetComponent<Transform>();
    }
    public static Coroutine coroutineStart(IEnumerator _routine)
    {
        return staticManager.StartCoroutine(_routine);
    }
    public static void CorutineStop(Coroutine _coroutine) {
        staticManager.StopCoroutine(_coroutine);
    }
}
