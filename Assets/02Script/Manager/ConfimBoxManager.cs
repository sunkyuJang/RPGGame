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

    Button YesBtn { set; get; }
    Button NoBtn { set; get; }

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
        YesBtn = confirmBoxPrefabObj.transform.Find("BtnSpacing").Find("Yes").GetComponent<Button>();
        YesBtn.onClick.AddListener(PreseedYes);
        NoBtn = confirmBoxPrefabObj.transform.Find("BtnSpacing").Find("No").GetComponent<Button>();
        NoBtn.onClick.AddListener(PressedNo);

        confirmBoxPrefabObj.SetActive(false);
    }
    public void ShowComfirmBox(string _script)
    {
        YesBtn.gameObject.SetActive(true);
        NoBtn.gameObject.SetActive(true);
        confirmBoxPrefabObj.transform.SetAsLastSibling();
        script.text = _script;
        confirmBoxPrefabObj.SetActive(true);
        rectTransform.SetAsLastSibling();
        NowState = State.Waiting;
    }

    public void ShowConfirmBoxSimple(string _script)
    {
        YesBtn.gameObject.SetActive(true);
        NoBtn.gameObject.SetActive(false);
        confirmBoxPrefabObj.transform.SetAsLastSibling();
        script.text = _script;
        confirmBoxPrefabObj.SetActive(true);
        rectTransform.SetAsLastSibling();
        NowState = State.Waiting;
    }

    public void PreseedYes() { NowState = State.Yes; confirmBoxPrefabObj.SetActive(false); }
    public void PressedNo() { NowState = State.No; confirmBoxPrefabObj.SetActive(false); }
}
