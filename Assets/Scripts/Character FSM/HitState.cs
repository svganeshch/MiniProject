using UnityEngine;

public class HitState : State
{
    public bool hitDone = false;

    private bool dodge = false;

    public HitState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        hitDone = false;
        dodge = false;

        //character.animator.Play("hit_f");
        character.animator.SetTrigger("damage");
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (dodgeAction.triggered)
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
            stateMachine.ChangeState(character.dodgeState);
        }

        if (hitDone)
        {
            stateMachine.ChangeState(character.combatState);
        }
    }

    private void CharacterMovement()
    {
        input = moveAction.ReadValue<Vector2>();
        verticalInput = input.y;
        horizontalInput = input.x;

        HandleRotation();
    }
}
