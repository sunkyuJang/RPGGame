using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;
public class StateViewer : MonoBehaviour
{
    RectTransform rectTransform;
    Image hpBar, mpBar;
    Text hpState, mpState;

    private void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        SetEachState(rectTransform.GetChild(0), ref hpBar, ref hpState);
        SetEachState(rectTransform.GetChild(1), ref mpBar, ref mpState);

    }
    void SetEachState(Transform _object, ref Image _image, ref Text _text)
    {
        _image = _object.GetChild(0).GetComponent<Image>();
        _image.type = Image.Type.Filled;
        _image.fillMethod = Image.FillMethod.Horizontal;
        _image.fillOrigin = (int)Image.OriginHorizontal.Left;
        _text = _object.GetChild(1).GetComponent<Text>();
    }

    public enum state { hp, mp }
    public void DrawState(state _state, int _origin, int _nowState)
    {
        Image nowBar = _state == state.hp ? hpBar : mpBar;
        Text nowText = _state == state.hp ? hpState : mpState;

        nowText.text = _nowState + "/" + _origin;
        nowBar.fillAmount = (float)_nowState / (float)_origin;
    }

    public static StateViewer GetNew() { return Create.GetNew<StateViewer>(); }
}
