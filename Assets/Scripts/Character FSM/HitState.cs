using UnityEngine;

public class HitState : State
{
    private bool dodge = false;

    public HitState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        hitDone = false;
        dodge = false;

        //character.animator.Play("hit_f");
        character.animator.applyRootMotion = true;
        character.animator.SetTrigger("damage");
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (player.dodgeAction.triggered)
        {
            dodge = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        CharacterMovement();

        if (dodge)
        {
            dodge = false;
            stateMachine.ChangeState(player.dodgeState);
        }

        if (hitDone)
        {
            stateMachine.ChangeState(character.combatState);
        }
    }

    private void CharacterMovement()
    {
        input = player.moveAction.ReadValue<Vector2>();
        verticalInput = input.y;
        horizontalInput = input.x;

        HandleRotation();
    }

    public override void Exit()
    {
        base.Exit();

        character.animator.applyRootMotion = false;
    }
}
