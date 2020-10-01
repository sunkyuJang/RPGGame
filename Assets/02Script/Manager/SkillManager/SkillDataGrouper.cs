using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDataGrouper : MonoBehaviour
{
    public static SkillDataGrouper instance;
    public Transform CharacterGroup;
    public Transform MonsterGroup;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.position = Vector3.zero;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void SetSkillDataParent(SkillData skillData)
    {
        var nowSkillGroup = new GameObject(skillData.Model.CharacterName);
        nowSkillGroup.transform.position = Vector3.zero;
        skillData.skillPooling.SetParent(nowSkillGroup.transform);

        if (skillData.Model is Character) nowSkillGroup.transform.SetParent(CharacterGroup);
        else if (skillData.Model is Monster) nowSkillGroup.transform.SetParent(MonsterGroup);
    }
}
