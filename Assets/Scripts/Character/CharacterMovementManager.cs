using UnityEngine;
using UnityEngine.Windows;

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
    [HideInInspector] public float inAirTime = 0;
    protected bool fallingVelocitySet = false;
    protected bool isGrounded = false;

    protected float horizontalInput;
    protected float verticalInput;
    protected float speed;
    protected float m_StepCycle;
    protected float m_NextStep;
    protected float speedStepInterval;

    protected Character character;
    protected FootStepsHandler footstepsHandler;

    public virtual void Start()
    {
        character = GetComponent<Character>();
        footstepsHandler = GetComponent<FootStepsHandler>();
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

        speed = character.walkingSpeed;
        speedStepInterval = footstepsHandler.walkingStepInterval;

        if (character.characterStateMachine.currentState == character.sprintState)
        {
            speed = character.sprintSpeed;
            speedStepInterval = footstepsHandler.sprintStepInterval;
        }
        else if (character.characterStateMachine.currentState == character.combatState)
        {
            speed = character.combatSpeed;
            speedStepInterval = footstepsHandler.combatStepInterval;
        }
        else if (character.moveAmount <= 0.5f)
        {
            speed = character.walkingSpeed;
            speedStepInterval = footstepsHandler.walkingStepInterval;
        }
        else if (character.moveAmount > 0.5f)
        {
            speed = character.runningSpeed;
            speedStepInterval = footstepsHandler.runningStepInterval;
        }

        character.controller.Move(speed * Time.deltaTime * moveDirection);

        if (character.controller.velocity.sqrMagnitude > 0 && (horizontalInput != 0 || verticalInput != 0))
        {
            m_StepCycle += (character.controller.velocity.magnitude + (speed * speedStepInterval)) * Time.deltaTime;
        }

        if (!(m_StepCycle > m_NextStep))
        {
            return;
        }

        m_NextStep = m_StepCycle + footstepsHandler.stepInterval;

        character.characterSfxManager.PlayFootStepsSound();
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