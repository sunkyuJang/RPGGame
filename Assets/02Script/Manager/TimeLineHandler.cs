using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimeLineHandler : MonoBehaviour
{
    public Character Character { set; get; }
    public CameraController CameraController { set; get; }
    public PlayableDirector playableDirector { set; get; }
    public int interrupt = 0;

    bool isAlreadyRunning { set; get; }
    public bool IsInterruptOccur { get { return interrupt == 1; } }
    private void Awake()
    {
        playableDirector = gameObject.GetComponent<PlayableDirector>();
        playableDirector.Stop();
    }
    private void Start()
    {
        Character = StaticManager.Character;
        CameraController = StaticManager.cameraController;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character") && !isAlreadyRunning)
        {
            isAlreadyRunning = true;
            Character.ShowGameUI(false);
            ToDo();
        }
    }
    protected virtual void ToDo() { }
}
