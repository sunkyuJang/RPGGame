using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSelecter : MonoBehaviour
{
    List<Text> Selectors = new List<Text>();
    public int selectNum = -1;
    int num = 0;
    private void Awake()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Selectors.Add(transform.GetChild(i).GetComponent<Text>());
            Selectors[i].gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public void ShowSelectors(string text)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        var selector = Selectors[num++];
        selector.gameObject.SetActive(true);
        selector.text = text;
    }
    public void HideSelecter()
    {
        foreach(Text text in Selectors)
        {
            text.text = "";
            text.gameObject.SetActive(false);
        }
        num = 0;
        gameObject.SetActive(false);
    }

    public void SelectedFirst() => selectNum = 0;
    public void SelectedSecond() => selectNum = 1;
    public void SelectedThird() => selectNum = 2;

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