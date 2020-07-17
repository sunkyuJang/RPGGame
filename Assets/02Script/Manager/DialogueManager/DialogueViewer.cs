using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueViewer : MonoBehaviour
{
    public Text nameText;
    public Text DialogueText;
    
    public DialogueSelecter dialogueSelecter { private set; get; }

    public Coroutine nowRunningScript;
    public bool StopRunningScript = false;
 
    private void Awake()
    {
        nameText = transform.Find("Name").GetComponent<Text>();
        DialogueText = transform.Find("Dialogue").GetComponent<Text>();

        dialogueSelecter = transform.Find("Selecter").GetComponent<DialogueSelecter>();
        dialogueSelecter.ShowSelecter(false);
        gameObject.SetActive(false);
    }

    public void ShowDiaogue(string name, string dialogue)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        StopRunningScript = false;
        nameText.text = name;
        DialogueText.text = "";
        nowRunningScript = StartCoroutine(ShowOnebyOne(dialogue));
    }

    IEnumerator ShowOnebyOne(string script)
    {
        for(int i = 0; i < script.Length && !StopRunningScript; i++)
        {
            DialogueText.text += script[i];
            yield return new WaitForSeconds(0.1f);
        }

        if (StopRunningScript)
            DialogueText.text = script;

        StopRunningScript = false;
        nowRunningScript = null;
    }
    public bool IsStillShowing { get { return nowRunningScript != null; } }
}
