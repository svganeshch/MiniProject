using UnityEngine;

public class SprintState : State
{
    bool sprint;
    bool sprintJump;
    bool isGrounded;
    float playerSpeed;
    float gravityValue;

    public SprintState(Player _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        player = _character;
    }

    public override void Enter()
    {
        base.Enter();

        sprint = false;
        sprintJump = false;

        input = Vector2.zero;
        moveVelocity = Vector3.zero;
        targetDirection = Vector3.zero;
        gravityVelocity.y = 0;

        playerSpeed = player.sprintSpeed;
        gravityValue = character.GRAVITY_VALUE;
        isGrounded = character.controller.isGrounded;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        input = player.moveAction.ReadValue<Vector2>();
        verticalInput = input.y;
        horizontalInput = input.x;

        if (player.sprintAction.triggered || input.sqrMagnitude == 0f)
        {
            sprint = false;
        }
        else
        {
            sprint = true;
        }

        if (player.jumpAction.triggered)
        {
            sprintJump = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        moveVelocity = PlayerCamera.Instance.transform.forward * verticalInput;
        moveVelocity += PlayerCamera.Instance.transform.right * horizontalInput;
        moveVelocity.Normalize();
        moveVelocity.y = 0;

        if (sprint)
        {
            character.animator.SetFloat("speedY", input.magnitude + 1f, player.speedDampTime, Time.deltaTime);
        }
        else
        {
            stateMachine.ChangeState(player.idleState);
        }
        if (sprintJump)
        {
            stateMachine.ChangeState(player.sprintJumpState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        gravityVelocity.y += gravityValue * Time.fixedDeltaTime;
        isGrounded = character.controller.isGrounded;

        if (isGrounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        // running speed
        character.controller.Move(playerSpeed * Time.fixedDeltaTime * moveVelocity + gravityVelocity * Time.fixedDeltaTime);

        HandleRotation();
    }
}
