using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GLip;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEditorInternal;
using System.Linq;
using System.Net.NetworkInformation;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public DialogueViewer DialogueViewer { set; get; }
    public DialogueSelecter DialogueSelecter { set; get; }
    public Character PlayerModel { set; get; }
    public Model ScriptModel { set; get; }
    public List<DialogueSheet.Param> DialogueScript { set; get; } 
    enum NextState { None, Select, Quest, ComfirmBox, End, Exit, Trade, Skip}
    NextState GetNextState { get
        {
            switch (DialogueScript[ScriptModel.lastDialog].Type)
            {
                case "None": return NextState.None;
                case "Select": return NextState.Select;
                case "Quest": return NextState.Quest;
                case "ComfirmBox": return NextState.ComfirmBox;
                case "Skip": return NextState.Skip;
                case "Exit": return NextState.Exit;
            }
            return NextState.End;
        } }


    Dictionary<string, int> lastTimetalkingWith { set; get; } = new Dictionary<string, int>();
    private void Awake()
    {
        instance = this;
        DialogueViewer = Instantiate(Resources.Load<GameObject>("Managers/DialogueViewer"), transform).GetComponent<DialogueViewer>();
        DialogueViewer.transform.Find("Dialogue").Find("Dialogue").GetComponent<EventTrigger>().triggers[0].callback.AddListener((data) => SetNextDialogue());
        DialogueSelecter = DialogueViewer.dialogueSelecter;
    }

    public void ShowDialogue(Character playerModel, Model model)
    {
        if(model.HasDialogue)
        {
            PlayerModel = playerModel;
            ScriptModel = model;

            lastTimetalkingWith = playerModel.LastTimeTalkingWith;

            DialogueScript = model.Dialogue;
            model.lastDialog = GetLastDialogueIndex();
            DialogueViewer.ShowDiaogue(model.name, DialogueScript[model.lastDialog].Script);
            SetNextDialogue();
        }
    }

    int GetLastDialogueIndex()
    {
        foreach (KeyValuePair<string, int> list in lastTimetalkingWith)
            if (list.Key.Equals(ScriptModel.CharacterName))
                return list.Value;

        lastTimetalkingWith.Add(ScriptModel.CharacterName, 0);
        return 0;
    }

    bool canSkipNext { set; get; } = true;
    void SetNextDialogue()
    {
        if (DialogueViewer.IsStillShowing)
        {
            DialogueViewer.StopRunningScript = true;
        }
        else
        {
            if (canSkipNext)
            {
                switch (GetNextState)
                {
                    case NextState.None: 
                        ScriptModel.lastDialog++;
                        DialogueViewer.ShowDiaogue(ScriptModel.name, DialogueScript[ScriptModel.lastDialog].Script);
                        break;
                    case NextState.Select: 
                        StartCoroutine(SelectScript());
                        break;
                    case NextState.End:
                        StartCoroutine(EndDialogue()); 
                        break;
                    case NextState.Skip: 
                        ScriptModel.lastDialog = DialogueScript[ScriptModel.lastDialog].GoTo;
                        DialogueViewer.ShowDiaogue(ScriptModel.name, DialogueScript[ScriptModel.lastDialog].Script);
                        break;
                    case NextState.Exit:
                        StartCoroutine(DoExit()); 
                        break;
                    case NextState.Quest: 
                        CheckQuest((ScriptModel as Npc)); 
                        break;
                }
            }
        }
    }
    IEnumerator SelectScript()
    {
        canSkipNext = false;
        var selector = DialogueViewer.dialogueSelecter;
        List<DialogueSheet.Param> selectSub = new List<DialogueSheet.Param>();
        var nowIndex = ScriptModel.lastDialog + 1;
        while(GetScriptByIndex(nowIndex).Type == "SelectSub")
        {
            var nowScript = GetScriptByIndex(nowIndex++);
            selector.ShowSelectors(nowScript.Script);
            selectSub.Add(nowScript);
        }

        while(selector.selectNum < 0)
            yield return new WaitForFixedUpdate();

        selector.HideSelecter();
        ScriptModel.lastDialog = selectSub[selector.GetSelectNum].GoTo;
        canSkipNext = true;
        DialogueViewer.ShowDiaogue(ScriptModel.name, DialogueScript[ScriptModel.lastDialog].Script);
    }

    IEnumerator EndDialogue()
    {
        canSkipNext = false;
        var npc = ScriptModel as Npc;
        List<NextState> states = new List<NextState>();

        states.Add(NextState.End);
        DialogueSelecter.ShowSelectors("대화를 끝낸다.");
        if (npc.Inventory.HasItem) { states.Add(NextState.Trade); DialogueSelecter.ShowSelectors("거래를 한다"); }
        if (QuestManager.NpcHasQuest(npc, PlayerModel)) { states.Add(NextState.Quest); DialogueSelecter.ShowSelectors("퀘스트를 진행한다"); }
        
        while (DialogueSelecter.selectNum < 0)
            yield return new WaitForFixedUpdate();

        DialogueSelecter.HideSelecter();
        canSkipNext = true;
        switch (states[DialogueSelecter.GetSelectNum])
        {
            case NextState.End:
                IntoNomalUI();
                break;
            case NextState.Trade:
                IntoNomalUI();
                PlayerModel.SetActionState(Character.ActionState.Trade);
                break;
            case NextState.Quest:
                CheckQuest(npc);
                //DialogueViewer.ShowDiaogue(ScriptModel.name, DialogueScript[++ScriptModel.lastDialog].Script);
                break;
        }
    }
    void CheckQuest(Npc npc)
    {
        canSkipNext = false;

        int processingIndex = 0;
        bool isAlreadyQuestAccept = QuestManager.isAlreadyAccept(npc, PlayerModel, out processingIndex);
        //var quest = QuestManager.GetQuest(npc, PlayerModel, processingIndex);

        if (isAlreadyQuestAccept)
            if (QuestManager.CanClearQuest(PlayerModel, processingIndex))
                npc.lastDialog = npc.Dialogue[npc.lastDialog].GoTo;
            else
                npc.lastDialog++;

        else
            if (npc.Dialogue[npc.lastDialog].Type == "Quest")
            {
                StartCoroutine(ConfirmQuest(QuestManager.GetNewQuest(npc)));
                return;
            }
        else
            npc.lastDialog++;
        
        canSkipNext = true;
        DialogueViewer.ShowDiaogue(ScriptModel.name, DialogueScript[ScriptModel.lastDialog].Script);
    }

    IEnumerator ConfirmQuest(QuestManager.QuestTable quest)
    {
        var list = new List<DialogueSheet.Param>();

        for(int i = 1; i < 3; i++)
        {
            var nowIndex = ScriptModel.lastDialog + i;
            DialogueSelecter.ShowSelectors(ScriptModel.Dialogue[nowIndex].Script);
            list.Add(ScriptModel.Dialogue[nowIndex]);
        }

        yield return new WaitUntil(() => DialogueSelecter.selectNum >= 0);

        if (list[DialogueSelecter.selectNum].Type == "Yes")
            QuestManager.AcceptQuest(quest, PlayerModel);

        ScriptModel.lastDialog = list[DialogueSelecter.GetSelectNum].GoTo;
        DialogueSelecter.HideSelecter();

        canSkipNext = true;
        DialogueViewer.ShowDiaogue(ScriptModel.name, DialogueScript[ScriptModel.lastDialog].Script);
/*        while (DialogueViewer.IsStillShowing)
            yield return new WaitForFixedUpdate();

        yield return new WaitForSeconds(0.5f);

        var npc = ScriptModel as Npc;
        string comfirmText = "퀘스트를 수락하시겠습니까? \r\n\r\n";

        comfirmText += "요구사항: ";
        foreach (ItemManager.ItemCounter counter in quest.RequireList)
        {
            comfirmText += counter.Data.Name + "X" + counter.count + "\r\n";
        }

        comfirmText += "\r\n보상: ";
        foreach (ItemManager.ItemCounter counter in quest.RewardList)
        {
            comfirmText += counter.Data.Name + "X" + counter.count + "\r\n";
        }

        var confirmBox = ConfimBoxManager.instance;
        confirmBox.ShowComfirmBox(comfirmText);

        while (confirmBox.NowState == ConfimBoxManager.State.Waiting)
            yield return new WaitForFixedUpdate();

        canSkipNext = true;
        switch (confirmBox.NowState)
        {
            case ConfimBoxManager.State.Yes:
                QuestManager.AcceptQuest(quest);
                npc.lastDialog = GetScriptByIndex(npc.lastDialog).GoTo;
                break;
            case ConfimBoxManager.State.No:
                npc.lastDialog++;
                break;
        }

        DialogueViewer.ShowDiaogue(ScriptModel.name, DialogueScript[ScriptModel.lastDialog].Script);*/
    }


    IEnumerator DoExit()
    {
        while (DialogueViewer.IsStillShowing)
            yield return new WaitForFixedUpdate();

        IntoNomalUI();
    }
    void IntoNomalUI()
    {
        for (int i = 0; i < lastTimetalkingWith.Count; i++)
            if (lastTimetalkingWith.Keys.ToList()[i].Equals(ScriptModel.CharacterName))
            {
                lastTimetalkingWith[ScriptModel.CharacterName] = ScriptModel.lastDialog;
                break;
            }

        DialogueSelecter.HideSelecter();
        DialogueViewer.gameObject.SetActive(false);
        PlayerModel.IntoNormalUI();
    }
    DialogueSheet.Param GetScriptByIndex(int i) { return DialogueScript[i]; }
}
/*    public Character Character { set; get; }

    private static Npc model;

    public static DialogueManager dialogueManager;
    public static GameObject DialogueManagerChild;
    public DialogueSheet dialogueSheet;
    public static List<DialogueSheet.Param> dialogue;
    new private RectTransform transform;

    private static Text nameText;
    private static Text scriptText;

    private static GameObject selectorObj;
    private Transform selecterTransform;

    private static Coroutine runningShowLine;
    private static string nowScript;

    private static bool isShowLinerunning;
    private enum nextStepState { select, trade, quest, end, call }
    List<nextStepState> nextStepStates = new List<nextStepState>();
    private enum questState { enter, process, falseQuest, trueQuest }

    void Awake()
    {
        Character = GameObject.Find("Character").GetComponent<Character>();

        transform = gameObject.GetComponent<RectTransform>();

        dialogueManager = this;

        Transform childTransform = Instantiate(Resources.Load<GameObject>("Managers/DialogueManager"), transform).GetComponent<Transform>();
        DialogueManagerChild = childTransform.gameObject;
        childTransform.Find("Dialog").GetComponent<EventTrigger>().triggers[0].callback.AddListener((data) => SkipDialogue());

        nameText = childTransform.GetChild(0).GetComponent<Text>();
        scriptText = childTransform.GetChild(1).GetComponent<Text>();
        selectorObj = childTransform.GetChild(2).gameObject;
        selecterTransform = selectorObj.GetComponent<Transform>();
        for (int i = 0; i < 3; i++)
        {
            EventTrigger.Entry entry = selecterTransform.GetChild(i).GetComponent<EventTrigger>().triggers[0];
            switch (i)
            {
                case 0: entry.callback.AddListener((data) => SelectedNum1()); break;
                case 1: entry.callback.AddListener((data) => SelectedNum2()); break;
                case 2: entry.callback.AddListener((data) => SelectedNum3()); break;
            }
        }
        DialogueManagerChild.SetActive(false);
    }

    private void Start()
    {
        transform.SetAsLastSibling();
    }
    public static void GetScript(Npc _model)
    {
        _model.dialogue = new List<DialogueSheet.Param>();
        _model.selecter = new List<DialogueSheet.Param>();
        for (int i = 0, nowIndex = 0; i < dialogue.Count; i++)
        {
            DialogueSheet.Param nowDialogue = dialogue[i];
            if (nowDialogue.Name == _model.CharacterName)
            {
                if (nowDialogue.Index == nowIndex)
                {
                    _model.dialogue.Add(nowDialogue);
                    nowIndex++;
                }
                else if (nowDialogue.NextStep.Equals("select"))
                {
                    _model.selecter.Add(dialogue[i]);
                }
            }
            else if (_model.dialogue.Count != 0 && dialogue[i].Name != _model.CharacterName) { break; }
        }
    }
    public static void ShowDialogue(Npc _model)
    {
        model = _model;
        nameText.text = model.CharacterName;
        DialogueManagerChild.SetActive(true);
        StartShowLine(ShowLine(model.lastDialog));
        selectorObj.SetActive(false);
    }
    private static void StartShowLine(IEnumerator _enumerator)
    {
        runningShowLine = StaticManager.coroutineStart(_enumerator);
    }
    static IEnumerator ShowLine(int i)
    {
        scriptText.text = "";
        isShowLinerunning = true;
        nowScript = model.dialogue[i].Script;
        for (int spelling = 0; spelling < nowScript.Length; spelling++)
        {
            scriptText.text += nowScript[spelling];
            yield return new WaitForSeconds(0.1f);
        }
        isShowLinerunning = false;
    }
    private IEnumerator StopShowLine()
    {
        StaticManager.CorutineStop(runningShowLine);
        yield return new WaitForEndOfFrame();
        scriptText.text = "";
        scriptText.text = nowScript;
        isShowLinerunning = false;
    }
    public void SkipDialogue()
    {
        if (DialogueManagerChild.activeSelf)
        {
            dialogue = model.dialogue;
            if (isShowLinerunning)
            {
                StaticManager.coroutineStart(StopShowLine());
            }
            else
            {
                if (!dialogue[model.lastDialog].Stop)
                {
                    StartShowLine(ShowLine(++model.lastDialog));
                }
                else
                {
                    if (model is Npc)
                    {
                        Npc npc = model as Npc;
                        string nextStep = npc.dialogue[npc.lastDialog].NextStep;
                        if (GetNowStep(nextStep) == nextStepState.select)
                        {
                            SelectDialogue(nextStep);
                        }
                        else
                        {
                            EndDialogue(npc, nextStep);
                        }
                    }
                }
            }
        }
    }
    private void SelectDialogue(string nextStep)
    {
        if (nextStep == "select")
        {
            selectorObj.SetActive(true);
            int lineNum = 0;
            for (int i = 0, limit = model.selecter.Count; i < limit; i++)
            {
                if (model.selecter[i].Index == model.lastDialog) { ChangeSelectorText(model.selecter[i].Script, lineNum++); limit = model.selecter.Count < (i + 3) ? model.selecter.Count : i + 3; }
            }
            if (lineNum < 3)
            {
                for (int i = lineNum; i < 3; i++)
                {
                    selecterTransform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        else if (nextStep.Substring(nextStep.Length - 2) == "in")
        {
            StartShowLine(ShowLine(++model.lastDialog));
        }
        else if (nextStep.Substring(nextStep.Length - 3) == "out")
        {
            for (int i = model.lastDialog + 1; i < model.dialogue.Count; i++)
            {
                if (model.dialogue[i].NextStep == "selectEnd")
                {
                    model.lastDialog = i;
                    string trxt = model.dialogue[model.lastDialog].Script;
                    StartShowLine(ShowLine(++model.lastDialog));
                    break;
                }
            }
        }
    }
    private void EndDialogue(Npc _npc, string nextStep)
    {
        nextStepState nowStep = GetNowStep(nextStep);
        if (nowStep == nextStepState.call)
        {
            _npc.lastDialog = int.Parse(nextStep.Substring(5, nextStep.Length - 5));
            DoNextStep(3);
        }
        else if (_npc.HasItems || nextStep == "quest")
        {
            nextStepStates.Clear();
            int lineNum = 0;
            selectorObj.SetActive(true);
            if (_npc.HasItems) { ChangeSelectorText("거래를 한다", lineNum++); nextStepStates.Add(nextStepState.trade); }
            if (nowStep == nextStepState.quest) { ChangeSelectorText("퀘스트를 본다", lineNum++); nextStepStates.Add(nextStepState.quest); }
            ChangeSelectorText("대화를 끝낸다", lineNum++); nextStepStates.Add(nextStepState.end);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    public void SelectedNum1() { DoNextStep(1); }
    public void SelectedNum2() { DoNextStep(2); }
    public void SelectedNum3() { DoNextStep(3); }

    private void DoNextStep(int selectNum)
    {
        if (model is Npc)
        {
            Npc npc = model as Npc;
            string state = npc.dialogue[model.lastDialog].NextStep;
            if (state == "select")
            {
                for (int i = model.lastDialog; i < dialogue.Count || dialogue[i].NextStep == "selectEnd"; i++)
                {
                    if (IsNextSelectScript(selectNum, i)) { npc.lastDialog = i; selectorObj.SetActive(false); StartShowLine(ShowLine(npc.lastDialog)); break; }
                }
            }
            else
            {
                nextStepState step = nextStepStates[selectNum - 1];
                switch (step)
                {
                    case nextStepState.trade:
                        Character.SetActionState(Character.ActionState.Trade);
                        DialogueManagerChild.SetActive(false);
                        break;
                    case nextStepState.quest:
                        selectorObj.SetActive(false);
                        DoQuestState(npc, state);
                        break;
                    case nextStepState.end:
                        Character.IntoNomalUI();
                        DialogueManagerChild.SetActive(false);
                        break;
                }
            }
        }
        for (int i = 0, selectorChild = 3; i < selectorChild; i++)
        {
            selecterTransform.GetChild(i).gameObject.SetActive(true);
            ChangeSelectorText("", i);
        }
    }
    private bool IsNextSelectScript(int selectNum, int i)
    {
        string nowStep = model.dialogue[i].NextStep;
        string inPart = "select/" + selectNum + "in";
        string outPart = "select/" + selectNum + "out";
        return nowStep.Equals(inPart) || nowStep.Equals(outPart);
    }

    private void DoQuestState(Npc npc, string _state)
    {
        questState state = GetQuestState(_state);
        switch (state)
        {
            case questState.enter:
                StartShowLine(ShowLine(++npc.lastDialog));
                break;
            case questState.process:
                int index = int.Parse(_state.Substring(14, _state.Length - 14));
                npc.lastDialog++;
                if (QuestManager.CanClearQuest(Character.Inventory, npc.CharacterName, index))
                {
                    StartShowLine(ShowLine(++npc.lastDialog));
                }
                else
                {
                    StartShowLine(ShowLine(--npc.lastDialog));
                }
                break;
        }
    }
    private questState GetQuestState(string _nextStep)
    {
        string state = _nextStep.Substring(6);
        switch (state)
        {
            case "in": return questState.enter;
            case "true": return questState.trueQuest;
        }
        return questState.process;
    }

    private void ChangeSelectorText(string _text, int i)
    {
        selecterTransform.GetChild(i).GetComponent<Text>().text = _text;
    }

    private nextStepState GetNowStep(string _nextStep)
    {
        if (_nextStep == "end") { return nextStepState.end; }
        else
        {
            if (_nextStep.Substring(0, 4) == "call") { return nextStepState.call; }
            else if (_nextStep.Substring(0, 5) == "quest") { return nextStepState.quest; }
            else { return nextStepState.select; }
        }
    }*/