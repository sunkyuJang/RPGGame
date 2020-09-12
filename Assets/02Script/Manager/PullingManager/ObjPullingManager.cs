using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ObjPullingManager : MonoBehaviour
{
    public static ObjPullingManager staticObjHandler { set; get; }
    public Transform pullingGroup;
    public GameObject objControllerPrefab;
    public List<ObjPullingController> objPullingControllers { set; get; } = new List<ObjPullingController>();

    private void Awake()
    {
        staticObjHandler = this;
        pullingGroup = new GameObject().transform;
        pullingGroup.name = "PullingManager";
    }

    public ObjPullingController ReqeuestObjPullingController(GameObject requestPrefab)
    {
        foreach (ObjPullingController controller in objPullingControllers)
            if (controller.comparePrefab.Equals(requestPrefab))
                return controller;

        return CreatObjPullingController(requestPrefab);
    }

    public ObjPullingController CreatObjPullingController(GameObject requestPrefab)
    {
        var controller = Instantiate(objControllerPrefab, pullingGroup).GetComponent<ObjPullingController>();
        controller.name = requestPrefab.name + "PullList";
        controller.comparePrefab = requestPrefab;
        return controller;
    }
}
