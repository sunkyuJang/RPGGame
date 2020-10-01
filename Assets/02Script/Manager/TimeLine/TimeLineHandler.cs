using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.PlayerLoop;
using UnityEngine.Timeline;
using Cinemachine;

public class TimeLineHandler : MonoBehaviour
{
    public Character Character { set; get; }

    public Transform cameraGroup;
    public List<CinemachineVirtualCamera> lookAtCharacterCameras = new List<CinemachineVirtualCamera>(); 
    public PlayableDirector playableDirector { set; get; }
    public int interrupt = 0;
    bool isAlreadyRunning { set; get; }
    public bool IsInterruptOccur { get { return interrupt == 1; } }
    private void Awake()
    {
        playableDirector = gameObject.GetComponent<PlayableDirector>();
        playableDirector.Stop();

        foreach (Transform transform in cameraGroup)
            lookAtCharacterCameras.Add(transform.GetComponent<CinemachineVirtualCamera>());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character") && !isAlreadyRunning)
        {
            foreach (var playerTrack in (playableDirector.playableAsset as TimelineAsset).outputs)
                if (playerTrack.streamName == "Player")
                    playableDirector.SetGenericBinding(playerTrack.sourceObject, other.gameObject);

            foreach (var camera in lookAtCharacterCameras)
                camera.LookAt = other.transform;
            
            Character = other.GetComponent<Character>();

            playableDirector.Play();
            SetIsTimeLineStart(true);
            ToDo();
        }
    }

    void FixedUpdate()
    {
        if(IsInterruptOccur) playableDirector.time = 1f;
    }
    protected virtual void ToDo() { }

    protected void SetIsTimeLineStart(bool isStart)
    {
        StaticManager.SetRunningTimeLine(isStart ? this : null);
        isAlreadyRunning = isStart;
        Character.ShowGameUI(Character.UIList.all, false);
        gameObject.SetActive(isStart);
    }
}
