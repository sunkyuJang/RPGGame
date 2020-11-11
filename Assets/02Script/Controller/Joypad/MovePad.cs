using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;

public class MovePad : Joypad, IInputTracer
{
    public GameObject ActionPadObj;
    ActionPad ActionPad { set; get; }
    new private void Awake()
    {
        base.Awake();
        ActionPad = ActionPadObj.GetComponent<ActionPad>();
    }

    public override IEnumerator TraceInput(bool isTouch, int touchID, bool isMouse)
    {
        isPressed = true;
        float limit = downTransform.rect.width * 0.3f;
        Vector2 centerPosition = new Vector2(downTransform.position.x, downTransform.position.y);

        while (GPosition.IsHoldPressedInput(isTouch, touchID, isMouse))
        {
            Vector2 inputPosition = GPosition.GetInputPosition(isTouch, touchID, isMouse);
            float dist = Vector2.Distance(inputPosition, centerPosition);
            Vector2 nowNomal = (inputPosition - centerPosition).normalized;

            if (dist < limit) { upTransform.position = inputPosition; }
            else { upTransform.position = centerPosition + nowNomal * limit; }

            controller.MoveCharacter(isPressed, Mathf.Atan2(nowNomal.x, nowNomal.y));
            yield return new WaitForFixedUpdate();
        }

        isPressed = false;
        controller.MoveCharacter(isPressed, 0f);
        upTransform.position = downTransform.position;
    }
}
