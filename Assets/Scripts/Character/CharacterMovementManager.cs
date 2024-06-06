using UnityEngine;

public abstract class CharacterMovementManager : MonoBehaviour
{
    public abstract void GetMovementInput();

    Vector3 moveDirection;
    Vector3 targetRotationDirection;
    Vector3 lockedTargetDirection;
    Quaternion targetRotation;
    Quaternion finalRotation;

    [HideInInspector] public Vector3 yVelocity;
    [HideInInspector] public float gravityForce = -40;
    Vector3 jumpDirection;
    float groundCheckSphereRadius = 0.3f;
    float groundedYVelocity = -20;
    float fallStartYVelocity = -5;
    float inAirTime = 0;
    bool fallingVelocitySet = false;
    bool isGrounded = false;

    protected float horizontalInput;
    protected float verticalInput;

    Character character;

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

        moveDirection = PlayerCamera.Instance.transform.forward * verticalInput;
        moveDirection += PlayerCamera.Instance.transform.right * horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (character.characterStateMachine.currentState == character.sprintState)
        {
            // sprint speed
            character.controller.Move(character.sprintSpeed * Time.deltaTime * moveDirection);
        }
        else if (character.characterStateMachine.currentState == character.combatState)
        {
            // combat speed
            character.controller.Move(character.combatSpeed * Time.deltaTime * moveDirection);
        }
        else
        {
            if (character.moveAmount > 0.5f)
            {
                // running speed
                character.controller.Move(character.runningSpeed * Time.deltaTime * moveDirection);
            }
            else if (character.moveAmount <= 0.5f)
            {
                // walking speed
                character.controller.Move(character.walkingSpeed * Time.deltaTime * moveDirection);
            }
        }
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
            if (!character.currentLockedOnTarget)
                return;

            lockedTargetDirection = Vector3.zero;
            lockedTargetDirection = character.currentLockedOnTarget.transform.position - character.transform.position;
            lockedTargetDirection.y = 0f;
            lockedTargetDirection.Normalize();

            targetRotation = Quaternion.LookRotation(lockedTargetDirection);
            finalRotation = Quaternion.Slerp(character.transform.rotation, targetRotation, character.rotationDampTime * Time.deltaTime);
            character.transform.rotation = finalRotation;
        }
        else
        {
            targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.Instance.cameraObj.transform.forward * verticalInput;
            targetRotationDirection += PlayerCamera.Instance.cameraObj.transform.right * horizontalInput;
            targetRotationDirection.y = 0f;
            targetRotationDirection.Normalize();

            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = character.transform.forward;
            }

            targetRotation = Quaternion.LookRotation(targetRotationDirection);
            finalRotation = Quaternion.Slerp(character.transform.rotation, targetRotation, character.rotationDampTime * Time.deltaTime);
            character.transform.rotation = finalRotation;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, groundCheckSphereRadius);
    }
}
