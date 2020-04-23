using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
public class Joypad : MonoBehaviour
{
    public GameObject up, down;
    private RectTransform upTransform;
    private RectTransform downTransform;
    private RectTransform rectTransform;
    public float radian;
    private void Awake()
    {
        upTransform = up.GetComponent<RectTransform>();
        downTransform = down.GetComponent<RectTransform>();
        rectTransform = gameObject.GetComponent<RectTransform>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Pressed()
    {
        Vector2 touchPosition = TouchManager.GetTouch(rectTransform);

        float limit = downTransform.rect.width * 0.5f;
        Vector2 centerPosition = new Vector2(downTransform.position.x, downTransform.position.y);
        float dist = Vector2.Distance(touchPosition, centerPosition);
        Vector2 nowNomal = (touchPosition - centerPosition).normalized;
        if (dist < limit)
        {
            upTransform.position = touchPosition;
        }
        else
        {
            upTransform.position = centerPosition + nowNomal * limit;
        }
        radian = Mathf.Atan2(nowNomal.x, nowNomal.y);
    }
    public void PressedUp()
    {
        upTransform.position = downTransform.position;
    }
}
