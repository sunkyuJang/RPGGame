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
    public static void SetRunningTimeLine(TimeLineHandler timeLineHandler)
        => RunningTimeLine = timeLineHandler;

    public static bool IsRunningTimeLine { get { return RunningTimeLine != null; } }
    void Awake()
    {
        staticManager = this;
    }
}
