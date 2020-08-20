using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
using UnityEngine.EventSystems;

public class Joypad : MonoBehaviour
{
    protected Character character;
    public GameObject up, hold, down;
    protected RectTransform upTransform;
    protected RectTransform holdTransform;
    protected RectTransform downTransform;
    protected RectTransform rectTransform;

    protected Rect upRect;
    protected Rect downRect;
    protected Rect holdRect;
    
    public float radian;

    public bool isPressed { set; get; }
    protected void Awake()
    {
        upTransform = up.GetComponent<RectTransform>();
        holdTransform = hold.GetComponent<RectTransform>();
        downTransform = down.GetComponent<RectTransform>();
        rectTransform = gameObject.GetComponent<RectTransform>();

        upRect = GMath.GetRect(upTransform);
        downRect = GMath.GetRect(downTransform);
        holdRect = GMath.GetRect(holdTransform);
    }

    public void Start()
    {
        character = StaticManager.Character;
    }

    public void Pressed()
    {
        bool isTouch = false;
        int touchID = 0;
        bool isMouse = false;
        if (GPosition.IsContainInput(upTransform, out isTouch, out touchID, out isMouse))
        {
            StartCoroutine(TraceInput(isTouch, touchID, isMouse));
        }
    }
    protected virtual IEnumerator TraceInput(bool isTouch, int touchID, bool isMouse)
    {
        isPressed = true;
        float limit = downTransform.rect.width * 0.5f;
        Vector2 centerPosition = new Vector2(downTransform.position.x, downTransform.position.y);

        while (GPosition.IsHoldPressedInput(isTouch, touchID, isMouse))
        {
            Vector2 inputPosition = GPosition.GetInputPosition(isTouch, touchID, isMouse);
            float dist = Vector2.Distance(inputPosition, centerPosition);
            Vector2 nowNomal = (inputPosition - centerPosition).normalized;

            if(dist < limit) { upTransform.position = inputPosition; }
            else { upTransform.position = centerPosition + nowNomal * limit; }
            
            radian = Mathf.Atan2(nowNomal.x, nowNomal.y);
            yield return new WaitForFixedUpdate();
        }

        isPressed = false;
        upTransform.position = downTransform.position;
    }
}