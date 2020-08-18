using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkiilViewer : MonoBehaviour
{
    Character character;
    Transform skillPulling;
    public GameObject physicSkillGroup;
    public GameObject MagicSkillGroup;
    public GameObject skillTreeViewer;
    private void Awake()
    {
        skillPulling = new GameObject("CharacterSkillPulling").GetComponent<Transform>();
    }

    private void Start()
    {
        character = StaticManager.Character;
        SkillTreeSet();
    }

    void SkillTreeSet()
    {
        for (int i = 0; i < 2; i++)
        {
            var group = i == 0 ? physicSkillGroup.transform : MagicSkillGroup.transform;
            var count = group.childCount;

            for (int j = 0; j < count; j++)
            {
                var skillData = group.GetChild(j).GetComponent<SkillData>();
                skillData.Model = character.transform;
                skillData.skillpulling.parent = skillPulling;
            }
        }
    }
}
