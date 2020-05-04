using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLip;
using UnityEditorInternal;

public partial class Character : Model
{
    bool IsPlayerMove { set; get; }
    Vector3 Rotation { set; get; }

    bool CanMoving { get { return !NowAnimatorInfo.IsName("NomalAttack"); } }
    bool CanAttack { get { return NowAnimatorInfo.IsName("BattleIdle"); } }
    bool IsAttaking { set; get; } = false;

    public void Move(bool isPlayerMove, float joypadRadian)
    {
        NowState = isPlayerMove ? ActionState.Running : ActionState.Idle;
        if (isPlayerMove) Rotation = new Vector3(0, joypadRadian * Mathf.Rad2Deg, 0);
    }
}