using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GLip;
using System.Dynamic;

public class StateViewer : MonoBehaviour, IStateViewerHandler
{
    public Character Character { set; get; }
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
    public void DrawState(state _state)
    {
        Image nowBar = null;
        Text nowText = null;
        float _nowState = 0f;
        float _origin = 0f;

        switch (_state)
        {
            case state.hp:
                nowBar = hpBar;
                nowText = hpState;
                _nowState = Character.nowHP;
                _origin = Character.HP;
                break;
            case state.mp:
                nowBar = mpBar;
                nowText = mpState;
                _nowState = Character.nowMP;
                _origin = Character.MP;
                break;
        }
        nowBar.fillAmount = _nowState / _origin;
        nowText.text = _nowState + "/" + _origin;
    }

/*    public static StateViewer GetNew(Character character) 
    { 
        StateViewer viewer = Create.GetNewInCanvas<StateViewer>();
        viewer.Character = character;
        return viewer;
    }*/

    void IStateViewerHandler.RefreshState()
    {
        DrawState(state.hp);
        DrawState(state.mp);
    }

    void IStateViewerHandler.ShowObj(bool souldShow)
    {
        gameObject.SetActive(souldShow);
    }
}
