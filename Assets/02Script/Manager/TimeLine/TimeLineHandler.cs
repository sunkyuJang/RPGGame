using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimeLineHandler : MonoBehaviour
{
    public Character Character { set; get; }
    public PlayableDirector playableDirector { set; get; }
    public int interrupt = 0;

    public List<TimeLineAnimationController> timeLineAnimationControllers = new List<TimeLineAnimationController>();
    bool isAlreadyRunning { set; get; }
    public bool IsInterruptOccur { get { return interrupt == 1; } }
    private void Awake()
    {
        playableDirector = gameObject.GetComponent<PlayableDirector>();
        playableDirector.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character") && !isAlreadyRunning)
        {
            Character = other.GetComponent<Character>();
            playableDirector.Play();
            SetIsTimeLineStart(true);
            StartCoroutine(doit());
            //ToDo();
        }
    }

    IEnumerator doit()
    {
        while (!IsInterruptOccur)
            yield return new WaitForFixedUpdate();

        playableDirector.Pause();

        foreach (TimeLineAnimationController controller in timeLineAnimationControllers)
            controller.LoopLastAnimation(true);

    }
    protected virtual void ToDo() 
    {
        
    }

    protected void SetIsTimeLineStart(bool isStart)
    {
/*        StaticManager.SetRunningTimeLine(isStart ? this : null);
        isAlreadyRunning = isStart;
        Character.ShowGameUI(!isStart);
        gameObject.SetActive(isStart);*/
    }
}
