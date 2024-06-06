using UnityEngine;

public class JumpState : State
{
    Vector3 characterDirection;
    Vector3 jumpDirection;
    Vector3 freefallDirection;

    public JumpState(Player _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        player = _character;
    }

    public override void Enter()
    {
        base.Enter();

        characterDirection = Vector3.zero;

        jumpDirection = GetCharacterDirection();
        SetJumpDirectionVelocity();

        player.playerAnimatorManager.PlayJumpAction();
    }

    public override void HandleInput()
    {
        base.HandleInput();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        freefallDirection = GetCharacterDirection();
        player.controller.Move((player.jumpForwardVelocity * jumpDirection + player.freeFallControlVelocity * freefallDirection) * Time.deltaTime);

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void SetJumpDirectionVelocity()
    {
        if (jumpDirection != Vector3.zero)
        {
            if (player.characterStateMachine.previousState == player.sprintState)
            {
                jumpDirection *= 1;
                Debug.Log("sprint jump");
            }
            else if (player.moveAmount > 0.5f)
            {
                jumpDirection *= 0.5f;
                Debug.Log("run jump");
            }
            else if (player.moveAmount <= 0.5f)
            {
                jumpDirection *= 0.25f;
                Debug.Log("normal jump");
            }
        }
    }
}
