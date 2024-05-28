using UnityEngine;

public class DodgeState : State
{
    public bool dodgeDone;

    bool block;

    public DodgeState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        character.animator.applyRootMotion = true;

        block = false;
        dodgeDone = false;

        if (character.playerVelocity == Vector3.zero)
        {
            character.animator.SetFloat("speedY", 1f);
        }

        character.animator.SetTrigger("dodge");
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (blockAction.triggered)
        {
            block = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (dodgeDone)
        {
            if (block)
            {
                stateMachine.ChangeState(character.blockState);
                return;
            }

            Debug.Log("welp still here");

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
