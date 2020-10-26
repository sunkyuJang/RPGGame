using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.PlayerLoop;
using UnityEngine.Timeline;
using Cinemachine;
using UnityEditorInternal;

public class TimeLineHandler : MonoBehaviour
{
    public TimelineAsset playable;
    public Character Character { set; get; }
    public Transform lookAtCharacterCameraGroup;
    public List<CinemachineVirtualCamera> lookAtCharacterCameras = new List<CinemachineVirtualCamera>(); 
    public PlayableDirector playableDirector { set; get; }
    public int interrupt = 0;
    bool isAlreadyRunning { set; get; }
    public bool IsInterruptOccur { get { return interrupt == 1; } }
    protected void Awake()
    {
        playableDirector = transform.parent.GetComponent<PlayableDirector>();
        playableDirector.playableAsset = playable;
        playableDirector.Stop();

        if(lookAtCharacterCameraGroup != null)
            foreach (Transform transform in lookAtCharacterCameraGroup)
                lookAtCharacterCameras.Add(transform.GetComponent<CinemachineVirtualCamera>());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character") && !isAlreadyRunning)
        {
            Character = other.GetComponent<Character>();

            var isFirstPlay = true;

            foreach (string timeLineName in Character.PassedTimeLineAssetName)
                if (timeLineName.Equals(gameObject.name))
                    isFirstPlay = false;

            if (isFirstPlay)
            {
                Character.PassedTimeLineAssetName.Add(gameObject.name);
                Character.IntoClearUI();
                Character.IsRunningTimeLine = true;
                if (lookAtCharacterCameraGroup != null)
                {
                    foreach (var playerTrack in (playableDirector.playableAsset as TimelineAsset).outputs)
                        if (playerTrack.streamName == "Player")
                            playableDirector.SetGenericBinding(playerTrack.sourceObject, other.gameObject);

                    foreach (var camera in lookAtCharacterCameras)
                        camera.LookAt = other.transform;
                }

                playableDirector.Play();
                SetIsTimeLineStart(true);
                ToDo();
            }
            else
                gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if(IsInterruptOccur) playableDirector.time = 1f;
    }
    public void ToDo() { StartCoroutine(StartSequence()); }

    public virtual IEnumerator StartSequence() { yield break; }

    protected void SetIsTimeLineStart(bool isStart)
    {
        StaticManager.SetRunningTimeLine(isStart ? this : null);
        isAlreadyRunning = isStart;
        Character.IntoClearUI();
        gameObject.SetActive(isStart);
    }

    public void EndSequence()
    {
        Character.IsRunningTimeLine = false;
        Character.IntoNormalUI();
        gameObject.SetActive(false);
    }
}
