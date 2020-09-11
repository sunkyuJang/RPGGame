using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GLip
{
    interface IInputTracer
    {
        void Pressed();
        IEnumerator TraceInput(bool isTouch, int touchId, bool isMouse);
    }
}