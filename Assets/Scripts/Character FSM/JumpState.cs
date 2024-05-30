using UnityEngine;

public class JumpState : State
{
    bool isGrounded;

    float gravityValue;
    float jumpHeight;
    float playerSpeed;

    Vector3 airVelocity;

    public JumpState(Player _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        player = _character;
    }

    public override void Enter()
    {
        base.Enter();

        isGrounded = false;
        gravityValue = character.GRAVITY_VALUE;
        jumpHeight = player.jumpHeight;
        playerSpeed = player.walkingSpeed;
        gravityVelocity.y = 0;

        character.animator.SetFloat("speedY", 0);
        character.animator.SetTrigger("jump");

        Jump();
    }

    public override void HandleInput()
    {
        base.HandleInput();

        input = player.moveAction.ReadValue<Vector2>();
        verticalInput = input.y;
        horizontalInput = input.x;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isGrounded)
        {
            stateMachine.ChangeState(player.landState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!isGrounded)
        {
            moveVelocity = player.playerVelocity;
            airVelocity = new Vector3(input.x, 0, input.y);

            moveVelocity = horizontalInput * PlayerCamera.Instance.transform.right + moveVelocity.z * PlayerCamera.Instance.transform.forward.normalized;
            moveVelocity.y = 0f;

            airVelocity = airVelocity.x * PlayerCamera.Instance.transform.right.normalized + airVelocity.z * PlayerCamera.Instance.transform.forward.normalized;
            airVelocity.y = 0f;

            character.controller.Move(gravityVelocity * Time.fixedDeltaTime + playerSpeed * Time.fixedDeltaTime * (airVelocity * character.airControl + moveVelocity * (1 - character.airControl)));
        }

        gravityVelocity.y += gravityValue * Time.fixedDeltaTime;
        isGrounded = character.controller.isGrounded;
    }

    private void Jump()
    {
        gravityVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }
}
