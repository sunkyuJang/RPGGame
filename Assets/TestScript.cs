using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool isAlreadyRunning = false;
    // Update is called once per frame
    void FixedUpdate()
    {
        obj.transform.position += Vector3.left;
    }
}
