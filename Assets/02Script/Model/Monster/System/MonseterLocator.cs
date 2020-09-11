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
