using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class StaticManager : MonoBehaviour
{
    public GameObject characterObj;
    public static Character Character { private set; get; }
    public static Transform canvasTrasform { private set; get; }
    private static StaticManager staticManager;
    public static StaticManager GetStaticManager { get { return staticManager; } }
    private static ComfimBox comfimBox;

    private static List<RectTransform> mainTextPositions = new List<RectTransform>();
    private const int maxTextLength = 3;

    public static List<Model> RunningModels { private set; get; } = new List<Model>();
    public static void AddRunningModel(Model model) { RunningModels.Add(model); }
    public static void EffectToModelByTimeLine(bool isRunning) { foreach (Model model in RunningModels) model.IsRunningTimeLine = isRunning; }
    int alretLimite { set; get; }
    void Awake()
    {
        Character = characterObj.GetComponent<Character>();
        canvasTrasform = transform.parent;
        comfimBox = ComfimBox.GetNew;
        staticManager = this;
    }
    public static Coroutine coroutineStart(IEnumerator _routine)
    {
        return staticManager.StartCoroutine(_routine);
    }
    public static void CorutineStop(Coroutine _coroutine) {
        staticManager.StopCoroutine(_coroutine);
    }
    public static void ShowAlert(string _text, Color _color)
    {
        Text text = NewTextObj(_text, _color);
        GetStaticManager.StartCoroutine(DestroyTextObj(text));
    }
    public static void ShowAlert(string _text, Color _color, Vector3 _position) 
    { 
        Text text = NewTextObj(_text, _color); 
        text.transform.position = _position; 
        GetStaticManager.StartCoroutine(DestroyTextObj(text)); 
    }
    static Text NewTextObj(string _text, Color _color)
    {
        arraingeTextList();
        Text text = Instantiate(Resources.Load<Text>("AlretText"), canvasTrasform).GetComponent<Text>();
        text.text = _text;
        text.color = _color;
        mainTextPositions.Insert(0,text.gameObject.GetComponent<RectTransform>());
        return text;
    }
    static IEnumerator DestroyTextObj(Text _text)
    {
        while (_text.color.a > 0)
        {
            _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, _text.color.a - 0.005f);
            yield return new WaitForFixedUpdate();
            if (_text == null) { break; }
        }
        if (_text != null) Destroy(_text.gameObject);
    }
    private static void arraingeTextList()
    {
        if (mainTextPositions.Count > 0) 
        {
            for (int i = 0; i < mainTextPositions.Count; i++)
            {
                RectTransform transform = mainTextPositions[i];
                if (transform == null) break;
                else
                {
                    transform.position += new Vector3(0, transform.rect.height, 0);
                }
                if(i > maxTextLength) 
                {
                    Destroy(transform.gameObject);
                }
            }
        }
    }

    public static void ShowComfirmBox(string _script)
    {
        comfimBox.ShowComfirmBox(_script);
    }
    public static ComfimBox GetComfimBox
    {
        get { return comfimBox; }
    }
}
