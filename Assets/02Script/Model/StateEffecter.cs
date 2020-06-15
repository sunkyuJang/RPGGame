using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateEffecter : MonoBehaviour
{
    Model model { set; get; }

    void EffectToModel()
    {

    }

    StateEffecter GetNew (Model model)
    {
        StateEffecter effecter = new StateEffecter();
        effecter.model = model;

        return effecter;
    }
}
