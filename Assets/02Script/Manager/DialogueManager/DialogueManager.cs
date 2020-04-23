﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GLip;

public class DialogueManager : MonoBehaviour
{
    private static Model model;
    
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
        dialogue = dialogueSheet.sheets[0].list;
        transform = gameObject.GetComponent<RectTransform>();
        
        dialogueManager = this;

        Transform childTransform = transform.Find("DialogueManager");
        DialogueManagerChild = childTransform.gameObject;
        nameText = childTransform.GetChild(0).GetComponent<Text>();
        scriptText = childTransform.GetChild(1).GetComponent<Text>();
        selectorObj = childTransform.GetChild(2).gameObject;
        selecterTransform = selectorObj.GetComponent<Transform>();
        DialogueManagerChild.SetActive(false);
    }
    public static void GetScript(Model _model)
    {
        _model.dialogue = new List<DialogueSheet.Param>();
        _model.selecter = new List<DialogueSheet.Param>();
        for(int i = 0, nowIndex = 0; i < dialogue.Count; i++)
        {
            DialogueSheet.Param nowDialogue = dialogue[i];
            if(nowDialogue.Name == _model.CharacterName) 
            {
                if (nowDialogue.Index == nowIndex) 
                { 
                    _model.dialogue.Add(nowDialogue);
                    nowIndex++;
                }
                else if(nowDialogue.NextStep.Equals("select"))
                {
                    _model.selecter.Add(dialogue[i]);
                }
            }
            else if(_model.dialogue.Count != 0 && dialogue[i].Name != _model.CharacterName) { break; }
        }
    }
    public static void ShowDialogue(Model _model)
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
        else if(nextStep.Substring(nextStep.Length - 2) == "in")
        {
            StartShowLine(ShowLine(++model.lastDialog));
        }
        else if (nextStep.Substring(nextStep.Length - 3) == "out")
        {
            for(int i = model.lastDialog + 1; i < model.dialogue.Count; i++)
            {
                if(model.dialogue[i].NextStep == "selectEnd") 
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
        else if(_npc.HasItems || nextStep == "quest")
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
        if(model is Npc)
        {
            Npc npc = model as Npc;
            string state = npc.dialogue[model.lastDialog].NextStep;
            if(state == "select")
            {
                for(int i = model.lastDialog; i < dialogue.Count || dialogue[i].NextStep == "selectEnd"; i++)
                {
                    if(IsNextSelectScript(selectNum, i)) { npc.lastDialog = i; selectorObj.SetActive(false); StartShowLine(ShowLine(npc.lastDialog)); break; }
                }
            }
            else
            {
                Player player = StaticManager.Player;
                nextStepState step = nextStepStates[selectNum - 1];
                switch (step)
                {
                    case nextStepState.trade: 
                        player.DoTrade(npc);
                        DialogueManagerChild.SetActive(false);
                        break;
                    case nextStepState.quest:
                        selectorObj.SetActive(false);
                        DoQuestState(npc, state);
                        break;
                    case nextStepState.end: 
                        player.IntoNomalUI();
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
                if (QuestManager.CanClearQuest(StaticManager.Player.Inventory, npc.CharacterName, index))
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
        if(_nextStep == "end") { return nextStepState.end; }
        else
        {
            if (_nextStep.Substring(0, 4) == "call") { return nextStepState.call; }
            else if(_nextStep.Substring(0,5) == "quest") { return nextStepState.quest; }
            else { return nextStepState.select; }
        }
    }
}
