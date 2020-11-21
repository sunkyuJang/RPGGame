using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ObjPoolerManager : MonoBehaviour
{
    public static ObjPoolerManager instance { set; get; }
    public GameObject objControllerPrefab;
    public List<ObjPooler> ObjPoolers { set; get; } = new List<ObjPooler>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public ObjPooler ReqeuestObjPooler(GameObject requestPrefab)
    {
        foreach (ObjPooler controller in ObjPoolers)
            if (controller.comparePrefab.Equals(requestPrefab))
                return controller;

        return CreatObjPooler(requestPrefab);
    }
    public ObjPooler CreatObjPooler(GameObject requestPrefab)
    {
        var pooler = Instantiate(objControllerPrefab).GetComponent<ObjPooler>();
        pooler.name = requestPrefab.name + "Pooler";
        pooler.comparePrefab = requestPrefab;
        ObjPoolers.Add(pooler);
        return pooler;
    }
}
