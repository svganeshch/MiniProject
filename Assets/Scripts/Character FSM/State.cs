using UnityEngine;
using UnityEngine.InputSystem;

public class State
{
    protected Character character;
    protected StateMachine stateMachine;

    protected float verticalInput;
    protected float horizontalInput;
    protected float moveAmount;
    protected Vector2 input;
    protected Vector3 moveVelocity;
    protected Vector3 lockedTargetDirection;
    protected Vector3 targetDirection;
    protected Vector3 gravityVelocity;

    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction sprintAction;
    public InputAction dodgeAction;

    public InputAction drawWeaponAction;
    public InputAction blockAction;
    public InputAction liteAttackWeaponAction;
    public InputAction heavyAttackWeaponAction;
    public InputAction lockOnAction;
    public InputAction leftLockOnAction;
    public InputAction rightLockOnAction;

    public InputAction weapon1Action;
    public InputAction weapon2Action;
    public InputAction weapon3Action;
    public InputAction weapon4Action;

    public State(Character _character, StateMachine _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;

        moveAction = character.playerInput.actions["Move"];
        jumpAction = character.playerInput.actions["Jump"];
        sprintAction = character.playerInput.actions["Sprint"];
        dodgeAction = character.playerInput.actions["Dodge"];
        drawWeaponAction = character.playerInput.actions["DrawWeapon"];
        blockAction = character.playerInput.actions["Block"];
        liteAttackWeaponAction = character.playerInput.actions["LiteAttack"];
        heavyAttackWeaponAction = character.playerInput.actions["HeavyAttack"];
        lockOnAction = character.playerInput.actions["LockOn"];
        leftLockOnAction = character.playerInput.actions["LeftLockOn"];
        rightLockOnAction = character.playerInput.actions["RightLockOn"];

        weapon1Action = character.playerInput.actions["Weapon1"];
        weapon2Action = character.playerInput.actions["Weapon2"];
        weapon3Action = character.playerInput.actions["Weapon3"];
        weapon4Action = character.playerInput.actions["Weapon4"];
    }

    public virtual void Enter()
    {
        //Debug.Log("Entered state " + this.ToString());
    }

    public virtual void HandleInput()
    {

    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {

    }

    public virtual void Exit()
    {
        //Debug.Log("Exited state " + this.ToString());
    }

    public void HandleRotation()
    {
        if (character.isLockedOn)
        {
            if (character.currentLockedOnTarget == null)
                return;

            lockedTargetDirection = character.currentLockedOnTarget.transform.position - character.transform.position;
            lockedTargetDirection.y = 0f;
            lockedTargetDirection.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(lockedTargetDirection);
            Quaternion finalRotation = Quaternion.Slerp(character.transform.rotation, targetRotation, character.rotationDampTime * Time.fixedDeltaTime);
            character.transform.rotation = finalRotation;
        }
        else
        {
            targetDirection = Vector3.zero;
            targetDirection = PlayerCamera.instance.cameraObj.transform.forward * verticalInput;
            targetDirection += PlayerCamera.instance.cameraObj.transform.right * horizontalInput;
            targetDirection.Normalize();
            targetDirection.y = 0f;

            if (targetDirection == Vector3.zero)
            {
                targetDirection = character.transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetDirection);
            Quaternion targetRotation = Quaternion.Slerp(character.transform.rotation, newRotation, character.rotationDampTime * Time.fixedDeltaTime);
            character.transform.rotation = targetRotation;
        }
    }

    public void SetAnimationParameters(float horizontalInput, float verticalInput)
    {
        float snappedHorizontal = horizontalInput;
        float snappedVertical = verticalInput;

        if (horizontalInput > 0 && horizontalInput <= 0.5f)
        {
            snappedHorizontal = 0.5f;
        }
        else if (horizontalInput > 0.5f && horizontalInput <= 1)
        {
            snappedHorizontal = 1;
        }
        else if (horizontalInput < 0 && horizontalInput >= -0.5f)
        {
            snappedHorizontal = -0.5f;
        }
        else if (horizontalInput < -0.5f && horizontalInput >= -1)
        {
            snappedHorizontal = -1;
        }
        else
        {
            snappedHorizontal = 0;
        }

        if (verticalInput > 0 && verticalInput <= 0.5f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalInput > 0.5f && verticalInput <= 1)
        {
            snappedVertical = 1;
        }
        else if (verticalInput < 0 && verticalInput >= -0.5f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalInput < -0.5f && verticalInput >= -1)
        {
            snappedVertical = -1;
        }
        else
        {
            snappedVertical = 0;
        }

        character.animator.SetFloat("speedX", snappedHorizontal, character.speedDampTime, Time.deltaTime);
        character.animator.SetFloat("speedY", snappedVertical, character.speedDampTime, Time.deltaTime);
    }
}
