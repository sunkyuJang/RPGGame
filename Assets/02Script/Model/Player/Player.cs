using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
public partial class Player : MonoBehaviour
{
    private Controller controller;
    public Character Character { private set; get; }
    private Model OtherModel { set; get; }
    private bool IsInField { set; get; }

    private void Awake()
    {
        controller = Find.GetFind<Controller>();
        Character = Find.GetFind<Character>();
        IsInField = true;
    }
    private void Start()
    {

    }
    public void SetMove(bool isPlayerMove, float radian) { Character.Move(isPlayerMove, radian); }
    public void GetAction()
    {
        OtherModel = Character.GetExistModel;
        if (OtherModel is Npc)
        {
            IntoDialogueUi();
            OtherModel.GetTalk();
        }
        else
        {
            if (IsInField)
            {
                Character.DoAttackMotion();
                print(OtherModel);
                if(OtherModel is Monster)
                {
                    Monster monster = OtherModel as Monster;
                    monster.GetHit(Character.ATK);
                }
            }
            else
            {

            }
        }
    }

    public void IntoDialogueUi() { controller.SetAllActive(false); Character.QuickSlot.TurnOn(false); }
    public void IntoNomalUI() { controller.SetAllActive(true); Character.QuickSlot.TurnOn(true); }
}
