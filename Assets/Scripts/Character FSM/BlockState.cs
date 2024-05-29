using UnityEngine;

public class BlockState : State
{
    public BlockState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        input = Vector2.zero;

        character.animator.applyRootMotion = true;
        character.animator.SetTrigger("Block");
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (!blockAction.IsPressed())
        {
            character.animator.SetTrigger("releaseBlock");
            stateMachine.ChangeState(character.combatState);
        }

        input = moveAction.ReadValue<Vector2>();
        verticalInput = input.y;
        horizontalInput = input.x;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5f && moveAmount <= 1)
        {
            moveAmount = 1;
        }

        moveVelocity = PlayerCamera.instance.transform.forward * verticalInput;
        moveVelocity += PlayerCamera.instance.transform.right * horizontalInput;
        moveVelocity.Normalize();
        moveVelocity.y = 0;

        if (character.isLockedOn)
        {
            SetAnimationParameters(horizontalInput, verticalInput);
        }
        else
        {
            SetAnimationParameters(0, moveAmount);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        character.controller.Move(character.walkingSpeed * Time.fixedDeltaTime * moveVelocity + gravityVelocity * Time.fixedDeltaTime);
        HandleRotation();
    }

    public override void Exit()
    {
        base.Exit();

        character.animator.applyRootMotion = false;
    }
}
