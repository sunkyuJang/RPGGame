using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPooler : MonoBehaviour
{
    public GameObject comparePrefab;
    private Queue<GameObject> CreatedObjList { set; get; } = new Queue<GameObject>();

    public List<GameObject> GetObj(int count) 
    {
        var list = new List<GameObject>();
        for (int i = 0; i < count; i++)
            list.Add(CreatedObjList.Dequeue());

        return list;
    }
    public GameObject GetObj()
    {
        return CreatedObjList.Dequeue();
    }
    public T GetObj<T>()
    {
        return CreatedObjList.Dequeue().GetComponent<T>();
    }

    bool isRunningOut(int count) { return count > CreatedObjList.Count; }

    public IEnumerator CheckCanUseObj(int count)
    {
        if (isRunningOut(count))
        {
            count -= CreatedObjList.Count;
            StartCoroutine(CreateObj(count));
            yield return StartCoroutine(CreateObj(count));
        }
        yield return null;
    }
    public IEnumerator CheckCanUseObj()
    {
        var count = 1;
        if (isRunningOut(count))
        {
            count -= CreatedObjList.Count;
            StartCoroutine(CreateObj(count));
            yield return StartCoroutine(CreateObj(count));
        }
        yield return null;
    }
    public void returnObj(GameObject gameObject)
    {
        gameObject.transform.SetParent(transform);
        gameObject.SetActive(false);
        CreatedObjList.Enqueue(gameObject);
    }

    public void returnObj(List<GameObject> gameObjects)
    {
        for (int i = 0; i < gameObjects.Count; i++)
            CreatedObjList.Enqueue(gameObjects[i]);
    }

    IEnumerator CreateObj(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var nowObj = Instantiate(comparePrefab, transform);
            yield return new WaitForFixedUpdate();
            nowObj.SetActive(false);
            CreatedObjList.Enqueue(nowObj);
        }
    }
}
