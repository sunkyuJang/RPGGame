using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
public class ActionPad : Joypad
{
    public bool ShouldCameraMove { private set; get; }
    public bool IsMovingLeft { get { return upTransform.position.x - downTransform.position.x < 0f; } }
    public float CameraSpeed 
    { 
        get 
        {
            if (upTransform.position.x > (downRect.xMax - downRect.width * 0.25f)
                || upTransform.position.x < (downRect.xMin + downRect.width * 0.25f))
                return 2f;
            else
                return 1f;
        } 
    }

    new private void Awake()
    {
        base.Awake();
    }

    protected override IEnumerator TraceInput(bool isTouch, int touchID, bool isMouse)
    {
        isPressed = true;
        float limit = downTransform.rect.width * 0.5f;
        Vector2 centerPosition = new Vector2(downTransform.position.x, downTransform.position.y);

        while (GPosition.IsHoldPressedInput(isTouch, touchID, isMouse))
        {
            Vector2 inputPosition = GPosition.GetInputPosition(isTouch, touchID, isMouse);
            float dist = Vector2.Distance(inputPosition, centerPosition);
            Vector2 nowNomal = (inputPosition - centerPosition).normalized;

            if (dist < limit) { upTransform.position = inputPosition; }
            else { upTransform.position = centerPosition + nowNomal * limit; }

            ShouldCameraMove = CanCameraMove;

            yield return new WaitForFixedUpdate();
        }

        isPressed = false;
        upTransform.position = downTransform.position;
    }

    bool CanCameraMove { get { return holdRect.Contains(upTransform.position); } }

}
