using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MonsterGenerator : MonoBehaviour
{
    public GameObject monster;
    public int count;

    public Transform PullingList { private set; get; }
    public Queue<GameObject> CreatedObj { private set; get; } = new Queue<GameObject>();

    private void Awake()
    {
        PullingList = new GameObject().transform;
        PullingList.gameObject.name = monster.name + "PullingList";

        for (int i = 0; i < count; i++)
        {
            CreatedObj.Enqueue(Instantiate(monster, PullingList));
        }
    }

    private void Start()
    {

    }
}

/*    public Vector2 GenerateArea;
    public Color GenerateColor;
    public List<GameObject> MonsterTypes;
    public int MonsterCount;

    public void Start()
    {
        for(int i = 0; i < MonsterCount; i++)
        {
            
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = GenerateColor;
        Gizmos.DrawCube(transform.position, new Vector3(GenerateArea.x, 1, GenerateArea.y));
    }*/