using UnityEngine;

public class DodgeState : State
{
    bool block;

    public DodgeState(Player _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        player = _character;
    }

    public override void Enter()
    {
        base.Enter();
        character.animator.applyRootMotion = true;

        block = false;
        dodgeDone = false;

        if (player.playerVelocity == Vector3.zero)
        {
            character.animator.SetFloat("speedY", 1f);
        }

        character.animator.SetTrigger("dodge");
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (player.blockAction.triggered)
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
                stateMachine.ChangeState(player.blockState);
                return;
            }

            if (character.animator.GetBool("isCombat"))
            {
                stateMachine.ChangeState(player.combatState);
            }
            else
            {
                stateMachine.ChangeState(player.idleState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        character.animator.applyRootMotion = false;
    }
}
