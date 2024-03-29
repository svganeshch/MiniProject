using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : State
{
    public bool hitDone = false;

    public HitState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        hitDone = false;

        //character.animator.Play("hit_f");
        character.animator.SetTrigger("damage");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (hitDone)
        {
            stateMachine.ChangeState(stateMachine.previousState);
        }
    }
}
