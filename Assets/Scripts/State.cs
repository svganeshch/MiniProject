using UnityEngine;

public class State
{
    protected Character character;
    protected Player player;
    protected Enemy enemy;
    protected StateMachine stateMachine;

    protected float verticalInput;
    protected float horizontalInput;
    protected float moveAmount;
    protected Vector2 input;
    protected Vector3 moveVelocity;
    protected Vector3 lockedTargetDirection;
    protected Vector3 targetDirection;
    protected Vector3 gravityVelocity;

    // Swap weapon vars
    public int defaultWeaponSlot = 1;
    public int previousWeaponSlot = 0;
    public virtual void SwapWeapon() { }

    // State Reset bools
    public bool hitDone = false;
    public bool dodgeDone = false;
    public bool blockBrokenDone = false;

    // Player lockon
    public virtual void SetTarget(Enemy target) { }

    public State(Character _character, StateMachine _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;

        switch (_character)
        {
            case Player:
                player = _character as Player;
                break;
            case Enemy:
                enemy = _character as Enemy;
                break;
        }
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
            targetDirection = PlayerCamera.Instance.cameraObj.transform.forward * verticalInput;
            targetDirection += PlayerCamera.Instance.cameraObj.transform.right * horizontalInput;
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
