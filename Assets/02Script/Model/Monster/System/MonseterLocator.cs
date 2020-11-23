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
    public int vertical;
    public int horizontal;
    public float height;
    public Color color;
    Rect RoamingArea { set; get; }
    List<Monster> MonsterInArea { set; get; } = new List<Monster>();
    ObjPooler MonsterPooler;

    List<Character> Characters { set; get; } = new List<Character>();
    public bool canMonsterLocatorWorks { set; get; }
    private void Awake()
    {
        var position = GMath.ConvertV3xzToV2(transform.position);
        var positionX = position.x - vertical / 2;
        var positionY = position.y - horizontal / 2;
        RoamingArea = new Rect(new Vector2(positionX, positionY), new Vector2(vertical, horizontal));
        transform.GetComponent<BoxCollider>().size = new Vector3(RoamingArea.width, transform.position.y + height, RoamingArea.height);
    }

    private void Start()
    {
        MonsterPooler = ObjPoolerManager.instance.ReqeuestObjPooler(requestMonster);
        print(MonsterPooler);
        StartCoroutine(DoNext());
    }

    public void TurnOnLocator(Character character)
    {
        var isNew = true;
        for (int i = 0; i < Characters.Count; i++)
            if (Characters[i].Equals(character))
            {
                isNew = false;
                break;
            }

        if (isNew)
            Characters.Add(character);

        canMonsterLocatorWorks = true;
    }

    bool CanMonsterLocatorWorks()
    {
        if (Characters.Count > 0)
        {
            for (int i = 0; i < Characters.Count; i++)
            {
                var nowCharacter = Characters[i];
                var dist = Vector3.Distance(transform.position, nowCharacter.transform.position);
                if (dist < resetDist)
                    return true;
                else
                    Characters.RemoveAt(i--);
            }
        }

        return false;
    }
    IEnumerator DoNext()
    {
        while (true)
        {
            if (canMonsterLocatorWorks)
            {
                canMonsterLocatorWorks = CanMonsterLocatorWorks();
                nowCount = MonsterInArea.Count;
                if (MonsterInArea.Count < maxCount)
                {
                    var nowMonster = MonsterPooler.GetObj<Monster>();
                    nowMonster.transform.parent = transform;
                    nowMonster.gameObject.SetActive(true);
                    LocatedMonster(nowMonster);
                }
            }
            else
                if (MonsterInArea.Count > 0)
                returnAllMonsterObj();

            yield return new WaitForSeconds(responeTime);
        }
    }
    void returnAllMonsterObj()
    {
        for (int i = 0; i < MonsterInArea.Count; i++)
        {
            var nowMonster = MonsterInArea[i];
            returnMonsterObj(nowMonster.gameObject);
        }
        MonsterInArea.Clear();
    }

    void returnMonsterObj(GameObject gameObject)
    {
        gameObject.SetActive(false);
        if (MonsterPooler == null)
            Destroy(gameObject);
        else
            MonsterPooler.returnObj(gameObject);
    }
    void LocatedMonster(Monster monster)
    {
        monster.transform.position =
            new Vector3(
                Random.Range(RoamingArea.xMin, RoamingArea.xMax),
                0,
                Random.Range(RoamingArea.yMin, RoamingArea.yMax));

        monster.RoamingArea = RoamingArea;
        monster.MonsterLocator = this;
        MonsterInArea.Add(monster);
    }

    public void MonsterReturn(Monster monster)
    {
        monster.gameObject.SetActive(false);
        MonsterInArea.Remove(monster);
        returnMonsterObj(monster.gameObject);
    }
    protected void OnDisable()
    {
        DestroyAllMonster();
    }

    protected void DestroyAllMonster()
    {
        for (int i = MonsterInArea.Count - 1; i >= 0; i--)
            Destroy(MonsterInArea[i].gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawCube(transform.position + Vector3.up * 0.5f, new Vector3(vertical, height, horizontal));
    }
}
/*public class MonseterLocator : MonoBehaviour
{
    public GameObject requestMonster;
    public int maxCount;
    public int nowCount;
    public float responeTime;

    public int vertical;
    public int horizontal;
    public Color color;
    Rect RoamingArea { set; get; }
    List<Monster> MonsterInArea { set; get; } = new List<Monster>(); 
    MonsterGenerator MonsterGenerator { set; get; }

    private void Awake()
    {
        MonsterGenerator = transform.root.GetComponent<MonsterHandler>().SearchGenerator(requestMonster);
        RoamingArea = new Rect(GMath.ConvertV3xzToV2(transform.position), new Vector2(vertical/2, horizontal/2));
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
        Gizmos.DrawCube(transform.position, new Vector3(vertical, 1, horizontal));
    }
}
*/