using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;
public class InventoryPoolerManager : MonoBehaviour
{
    public GameObject inventoryPrefab;
    
    public static InventoryPoolerManager instance;
    public ObjPooler inventoryPooler;

    public Transform InventoryPoolGorup;

    private void Awake()
    {
        instance = this;
        inventoryPooler = ObjPoolerManager.instance.ReqeuestObjPooler(inventoryPrefab);
        GPosition.GetRectTransformWithReset(gameObject.GetComponent<RectTransform>(), inventoryPooler.gameObject.AddComponent<RectTransform>());
/*        
        var rectTransform = inventoryPooler.gameObject.AddComponent<RectTransform>();
        rectTransform.SetParent(transform);
        rectTransform.transform.localPosition = Vector3.zero;
        rectTransform.anchorMin = Vector3.zero;
        rectTransform.anchorMax = Vector3.one;
        rectTransform.localScale = Vector3.one;*/
    }
}
