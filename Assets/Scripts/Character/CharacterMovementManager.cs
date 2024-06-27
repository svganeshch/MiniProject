using UnityEngine;

public abstract class CharacterMovementManager : MonoBehaviour
{
    protected Vector3 moveDirection;
    protected Vector3 targetRotationDirection;
    protected Vector3 lockedTargetDirection;
    protected Quaternion targetRotation;
    protected Quaternion finalRotation;

    [HideInInspector] public Vector3 yVelocity;
    [HideInInspector] public float gravityForce = -40;
    protected float groundCheckSphereRadius = 0.3f;
    protected float groundedYVelocity = -20;
    protected float fallStartYVelocity = -5;
    protected float inAirTime = 0;
    protected bool fallingVelocitySet = false;
    protected bool isGrounded = false;

    protected float horizontalInput;
    protected float verticalInput;

    protected Character character;

    public virtual void Start()
    {
        character = GetComponent<Character>();
    }

    public virtual void Update()
    {
        HandleGroundedMovement();
        HandleGroundCheck();
        HandleRotation();
    }

    protected virtual void HandleGroundedMovement()
    {
        if (!character.canMove)
            return;

        GetMovementInput();

        moveDirection = PlayerCamera.Instance.playerCameraObjTransform.forward * verticalInput;
        moveDirection += PlayerCamera.Instance.playerCameraObjTransform.right * horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        float speed = character.walkingSpeed;

        if (character.characterStateMachine.currentState == character.sprintState)
            speed = character.sprintSpeed;
        else if (character.characterStateMachine.currentState == character.combatState)
            speed = character.combatSpeed;
        else if (character.moveAmount <= 0.5f)
            speed = character.walkingSpeed;
        else if (character.moveAmount > 0.5f)
            speed = character.runningSpeed;

        character.controller.Move(speed * Time.deltaTime * moveDirection);
    }

    protected virtual void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, LayerMaskManager.Instance.groundLayerMask);

        character.characterAnimatorManager.IsGrounded = isGrounded;

        if (isGrounded)
        {
            if (yVelocity.y < 0f)
            {
                inAirTime = 0;
                fallingVelocitySet = false;
                yVelocity.y = groundedYVelocity;
            }
        }
        else
        {
            if (character.characterStateMachine.currentState != character.jumpState && !fallingVelocitySet)
            {
                fallingVelocitySet = true;
                yVelocity.y = fallStartYVelocity;
            }

            inAirTime += Time.deltaTime;
            character.characterAnimatorManager.InAirTime = inAirTime;

            yVelocity.y += gravityForce * Time.deltaTime;
        }

        character.controller.Move(yVelocity * Time.deltaTime);
    }

    protected virtual void HandleRotation()
    {
        if (!character.canRotate)
            return;

        if (character.isLockedOn)
        {
            if (character.characterStateMachine.currentState == character.sprintState ||
                character.characterStateMachine.currentState == character.dodgeState)
            {
                targetRotationDirection = PlayerCamera.Instance.mainCameraTransform.forward * verticalInput;
                targetRotationDirection += PlayerCamera.Instance.mainCameraTransform.right * horizontalInput;
                targetRotationDirection.y = 0f;
                targetRotationDirection.Normalize();

                if (targetRotationDirection == Vector3.zero)
                {
                    targetRotationDirection = character.transform.forward;
                }

                targetRotation = Quaternion.LookRotation(targetRotationDirection);
            }
            else
            {
                if (character.currentLockedOnTarget == null) return;

                lockedTargetDirection = character.currentLockedOnTarget.transform.position - character.transform.position;
                lockedTargetDirection.y = 0f;
                lockedTargetDirection.Normalize();

                targetRotation = Quaternion.LookRotation(lockedTargetDirection);
            }
        }
        else
        {
            targetRotationDirection = PlayerCamera.Instance.mainCameraTransform.forward * verticalInput;
            targetRotationDirection += PlayerCamera.Instance.mainCameraTransform.right * horizontalInput;
            targetRotationDirection.y = 0f;
            targetRotationDirection.Normalize();

            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = character.transform.forward;
            }

            targetRotation = Quaternion.LookRotation(targetRotationDirection);
        }

        finalRotation = Quaternion.Slerp(character.transform.rotation, targetRotation, character.rotationDampTime * Time.deltaTime);
        character.transform.rotation = finalRotation;
    }

    public abstract void GetMovementInput();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckSphereRadius);
    }
}