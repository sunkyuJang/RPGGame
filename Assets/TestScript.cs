using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Model model;
    // Start is called before the first frame update
    void Start()
    {
    }

    bool isAlreadyRunning = false;
    // Update is called once per frame
    void Update()
    {

        if (!isAlreadyRunning)
        {
            isAlreadyRunning = true;
            DialogueManager.ShowDialogue(model);

        }
    }
}
