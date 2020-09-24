using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;
public class MonseterLocator : MonoBehaviour
{
    public GameObject requestMonster;
    public int maxCount;
    public int nowCount;
    public float responeTime;
    public float resetDist = 100f;
    public float nowDistFromCharacter = 0f;
    public int vertical;
    public int horizon;
    public Color color;
    Rect RoamingArea { set; get; }
    List<Monster> MonsterInArea { set; get; } = new List<Monster>();
    ObjPullingController MonsterPullController;
    private void Awake()
    {
        var position = GMath.ConvertV3xzToV2(transform.position);
        var positionX = position.x - vertical / 2;
        var positionY = position.y - horizon / 2;
        RoamingArea = new Rect(new Vector2(positionX, positionY), new Vector2(vertical, horizon));
    }

    private void Start()
    {
        MonsterPullController = ObjPullingManager.instance.ReqeuestObjPullingController(requestMonster);
        StartCoroutine(DoNext());
    }

    IEnumerator DoNext()
    {
        while(true)
        {
            nowDistFromCharacter = Vector3.Distance(StaticManager.Character.transform.position, transform.position);
            nowCount = MonsterInArea.Count;
            if(nowDistFromCharacter < resetDist)
            {
                if(MonsterInArea.Count < maxCount)
                {
                    yield return StartCoroutine(MonsterPullController.CheckCanUseObj(1));
                    var nowMonster = MonsterPullController.GetObj().GetComponent<Monster>();
                    nowMonster.transform.parent = transform;
                    nowMonster.gameObject.SetActive(true);
                    LocatedMonster(nowMonster);
                }
            }
            else
                if(MonsterInArea.Count > 0)
                    returnAllMonsterObj();
            
            yield return new WaitForSeconds(responeTime);
        }
    }
    void returnAllMonsterObj()
    {
        print(true);
        for (int i = 0; i < MonsterInArea.Count; i++)
        {
            var nowMonster = MonsterInArea[i];
            nowMonster.gameObject.SetActive(false);
            returnMonsterObj(nowMonster.gameObject);
        }
        MonsterInArea.Clear();
    }

    void returnMonsterObj(GameObject gameObject)
    {
        MonsterPullController.returnObj(gameObject);
    }
    void LocatedMonster(Monster monster)
    {
        monster.transform.position =
            new Vector3(
                Random.Range(RoamingArea.xMin, RoamingArea.xMax),
                0,
                Random.Range(RoamingArea.yMin, RoamingArea.yMax));

        monster.RoamingArea = RoamingArea;
        MonsterInArea.Add(monster);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawCube(transform.position + Vector3.up * 0.5f, new Vector3(vertical, 1, horizon));
    }
}
/*public class MonseterLocator : MonoBehaviour
{
    public GameObject requestMonster;
    public int maxCount;
    public int nowCount;
    public float responeTime;

    public int vertical;
    public int horizon;
    public Color color;
    Rect RoamingArea { set; get; }
    List<Monster> MonsterInArea { set; get; } = new List<Monster>(); 
    MonsterGenerator MonsterGenerator { set; get; }

    private void Awake()
    {
        MonsterGenerator = transform.root.GetComponent<MonsterHandler>().SearchGenerator(requestMonster);
        RoamingArea = new Rect(GMath.ConvertV3xzToV2(transform.position), new Vector2(vertical/2, horizon/2));
    }

    void Start()
    {
        StartCoroutine(LocatedMonster());
    }

    IEnumerator LocatedMonster()
    {
        while (true)
        {
            if (MonsterInArea.Count < maxCount)
            {
                var nowObj = MonsterGenerator.CreatedObj.Dequeue();
                var nowMonster = nowObj.GetComponent<Monster>();
                nowMonster.transform.position = 
                    new Vector3(
                        Random.Range(RoamingArea.xMin, RoamingArea.xMax), 
                        0, 
                        Random.Range(RoamingArea.yMin, RoamingArea.yMax));
                nowMonster.RoamingArea = RoamingArea;
                MonsterInArea.Add(nowMonster);
            }
            yield return new WaitForSeconds(responeTime);
        }
    }

    public void GiveBackMonster(Monster monster) 
    { 
        for(int i = 0; i < MonsterInArea.Count; i++)
        {
            var nowMonster = MonsterInArea[i];
            if (nowMonster.Equals(monster))
            {
                MonsterInArea.RemoveAt(i);
                MonsterGenerator.CreatedObj.Enqueue(monster.gameObject);
            }
        }
    }

    private void Update()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawCube(transform.position, new Vector3(vertical, 1, horizon));
    }
}
*/