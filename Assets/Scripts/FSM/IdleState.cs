using UnityEngine;

public class IdleState : State
{
    Vector3 currentVelocity;
    Vector3 smoothVelocity;

    bool jump;
    bool sprint;
    bool isGrounded;
    bool drawWeapon;

    int weaponSlot = 1;
    float playerSpeed;
    float gravityValue;

    public IdleState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        jump = false;
        sprint = false;
        drawWeapon = false;
        input = Vector2.zero;
        velocity = Vector3.zero;
        gravityVelocity.y = 0;

        playerSpeed = character.playerSpeed;
        gravityValue = character.GRAVITY_VALUE;
        isGrounded = character.controller.isGrounded;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (jumpAction.triggered)
            jump = true;

        if (sprintAction.triggered)
            sprint = true;

        if (dodgeAction.triggered)
        {
            if (character.animator.GetFloat("speed") > 0.01f)
                character.animator.SetTrigger("dodge");
        }

        if (drawWeaponAction.triggered)
            drawWeapon = true;

        if (weapon1Action.triggered)
        {
            weaponSlot = 1;
            drawWeapon = true;
        }
        if (weapon2Action.triggered)
        {
            weaponSlot = 2;
            drawWeapon = true;
        }
        if (weapon3Action.triggered)
        {
            weaponSlot = 3;
            drawWeapon = true;
        }
        if (weapon4Action.triggered)
        {
            weaponSlot = 4;
            drawWeapon = true;
        }

        input = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(input.x, 0, input.y);

        velocity = velocity.x * character.mainCameraTransform.right.normalized + velocity.z * character.mainCameraTransform.forward.normalized;
        velocity.y = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        character.animator.SetFloat("speed", input.magnitude, character.speedDampTime, Time.deltaTime);

        //character.animator.SetFloat("speedX", input.x, character.speedDampTime, Time.deltaTime);
        //character.animator.SetFloat("speedY", input.y, character.speedDampTime, Time.deltaTime);

        if (jump)
            stateMachine.ChangeState(character.jumpState);

        if (sprint)
            stateMachine.ChangeState(character.sprintState);

        if (drawWeapon)
        {
            character.weaponEquipment.SetWeapon(weaponSlot);
            character.animator.SetTrigger("drawWeapon");
            stateMachine.ChangeState(character.combatState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        gravityVelocity.y += gravityValue * Time.deltaTime;
        isGrounded = character.controller.isGrounded;

        if (isGrounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref smoothVelocity, character.velocityDampTime);

        character.controller.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);

        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.LookRotation(velocity), character.rotationDampTime);
        }
    }

    public override void Exit()
    {
        base.Exit();

        gravityVelocity.y = 0f;
        character.playerVelocity = new Vector3(input.x, 0, input.y);

        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
}
