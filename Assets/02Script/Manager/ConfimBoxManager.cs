using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ConfimBoxManager : MonoBehaviour
{
    public static ConfimBoxManager instance;
    public GameObject confirmBoxPrefabObj;
    private RectTransform rectTransform;
    private Text script;
    public enum State { Yes, No, Waiting }
    public State NowState { private set; get; }
    public bool isPressed { set; get; }
    public bool isYes { set; get; }
    private void Awake()
    {
        instance = this;
        confirmBoxPrefabObj = Instantiate(confirmBoxPrefabObj, transform.root);
        rectTransform = confirmBoxPrefabObj.GetComponent<RectTransform>();

        script = confirmBoxPrefabObj.transform.GetChild(0).GetComponent<Text>();
        isPressed = false;
        isYes = false;
        confirmBoxPrefabObj.transform.Find("Yes").GetComponent<Button>().onClick.AddListener(PreseedYes);
        confirmBoxPrefabObj.transform.Find("No").GetComponent<Button>().onClick.AddListener(PressedNo);
        confirmBoxPrefabObj.SetActive(false);
    }
    public void ShowComfirmBox(string _script) 
    {
        confirmBoxPrefabObj.transform.SetAsLastSibling();
        script.text = _script;
        confirmBoxPrefabObj.SetActive(true);
        rectTransform.SetAsLastSibling();
        NowState = State.Waiting;
    }

    public void PreseedYes() { NowState = State.Yes; confirmBoxPrefabObj.SetActive(false); }
    public void PressedNo() { NowState = State.No; confirmBoxPrefabObj.SetActive(false); }
}
