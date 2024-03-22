using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeState : State
{
    float timePassed;
    float dodgeTime;

    public DodgeState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        character.animator.applyRootMotion = true;

        timePassed = 0;
        dodgeTime = 0.8f;

        if (character.playerVelocity == Vector3.zero)
        {
            character.animator.SetFloat("speedY", 0.5f);
        }

        character.animator.SetTrigger("dodge");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (timePassed > dodgeTime)
        {
            character.animator.SetTrigger("move");

            if (character.animator.GetBool("isCombat"))
            {
                stateMachine.ChangeState(character.combatState);
            }
            else
            {
                stateMachine.ChangeState(character.idleState);
            }
        }
        timePassed += Time.deltaTime;
    }

    public override void Exit()
    {
        base.Exit();

        character.animator.applyRootMotion = false;
    }
}
