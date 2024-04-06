using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeState : State
{
    public bool dodgeDone;

    public DodgeState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        character.animator.applyRootMotion = true;

        dodgeDone = false;

        if (character.playerVelocity == Vector3.zero)
        {
            character.animator.SetFloat("speedY", 1f);
        }

        character.animator.SetTrigger("dodge");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (dodgeDone)
        {
            if (character.animator.GetBool("isCombat"))
            {
                stateMachine.ChangeState(character.combatState);
            }
            else
            {
                stateMachine.ChangeState(character.idleState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        character.animator.applyRootMotion = false;
    }
}
