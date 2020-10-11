using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPooler : MonoBehaviour
{
    public GameObject comparePrefab;
    private Queue<GameObject> CreatedObjList { set; get; } = new Queue<GameObject>();

    T GetObjFromCreateObjList<T>()
    {
        return CreatedObjList.Dequeue().GetComponent<T>();
    }

    public List<T> GetObj<T>(int count)
    {
        List<T> requestList = new List<T>();
        if (isRunningOut(count))
        {
            for (int i = 0; i < count; i++)
            {
                var nowObj = Instantiate(comparePrefab, transform);
                nowObj.SetActive(false);
                requestList.Add(nowObj.GetComponent<T>());
            }
        }
        return requestList;
    }
    public T GetObj<T>()
    {
        if (isRunningOut(1))
        {
            return GetObj<T>(1)[0];
        }
        return GetObjFromCreateObjList<T>();
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
            nowObj.SetActive(false);
            CreatedObjList.Enqueue(nowObj);
        }
        yield return null;
    }

    public void MakeReservation(int count)
    {
        StartCoroutine(CreateObj(count));
    }
}
