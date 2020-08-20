using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSkillDescriptionBox : MonoBehaviour
{
    public Image icon;
    public Text description;
    public GameObject learnBtn;

    public void ShowDescription(SkillData skill)
    {
        icon.sprite = skill.icon;
        description.text = skill.description;
        learnBtn.SetActive(!skill.isLearn);
    }
}
