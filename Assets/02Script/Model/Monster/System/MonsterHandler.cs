using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHandler : MonoBehaviour
{
    public List<MonsterGenerator> MonsterType { set; get; } = new List<MonsterGenerator>();
    public Transform generator;
    public Transform locator;
    private void Awake()
    {
        foreach(Transform generator in generator)
            MonsterType.Add(generator.GetComponent<MonsterGenerator>());
    }

    public MonsterGenerator SearchGenerator(GameObject gameObject)
    {
        for (int i = 0; i < MonsterType.Count; i++)
        {
            var monsterGenerator = MonsterType[i];
            if (monsterGenerator.monster.Equals(gameObject))
                return monsterGenerator;
        }
        return null;
    }

    public void FixedUpdate()
    {
        foreach(Transform nowLocator in locator)
            if (!nowLocator.gameObject.activeSelf)
                if (Vector3.Distance(nowLocator.position, StaticManager.Character.transform.position) < 10f)
                    nowLocator.gameObject.SetActive(true);
    }
}