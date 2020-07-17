using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSelecter : MonoBehaviour
{
    List<Text> Selecters = new List<Text>();
    public int selectNum = -1;
    private void Awake()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Selecters.Add(transform.GetChild(0).GetComponent<Text>());
            Selecters[0].gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public void SetText(int position, string text) => Selecters[position].text = text;
    public void ShowSelecter(bool shouldShowing)
    {
        gameObject.SetActive(shouldShowing);
        foreach(Text text in Selecters)
        {
            if (shouldShowing)
            {
                if(text.text == "") 
                    break;
            }
            else { text.text = ""; }
            text.gameObject.SetActive(shouldShowing);
        }
    }
    public int GetSelectNum 
    { 
        get
        {
            if(selectNum > -1)
            {
                var nowNum = selectNum;
                selectNum = -1;
                return nowNum;
            }

            return selectNum;
        } 
    }
}