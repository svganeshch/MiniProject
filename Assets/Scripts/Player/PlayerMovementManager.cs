using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{
    Vector3 moveDirection;
    Vector3 targetRotationDirection;
    Vector3 lockedTargetDirection;
    Quaternion targetRotation;
    Quaternion finalRotation;

    [HideInInspector] public Vector3 yVelocity;
    Vector3 jumpDirection;
    [HideInInspector] public float gravityForce = -40;
    float groundCheckSphereRadius = 0.3f;
    float groundedYVelocity = -20;
    float fallStartYVelocity = -5;
    float inAirTime = 0;
    bool fallingVelocitySet = false;
    bool isGrounded = false;

    float horizontalInput;
    float verticalInput;

    Player player;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        HandleAllMovement();
    }

    private void HandleAllMovement()
    {
        HandleGroundedMovement();
        HandleGroundCheck();
        HandleRotation();
    }

    private void GetMovementInput()
    {
        horizontalInput = player.horizontalInput;
        verticalInput = player.verticalInput;
    }

    private void HandleGroundedMovement()
    {
        if (!player.canMove)
            return;

        GetMovementInput();

        moveDirection = PlayerCamera.Instance.transform.forward * verticalInput;
        moveDirection += PlayerCamera.Instance.transform.right * horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (player.characterStateMachine.currentState == player.sprintState)
        {
            // sprint speed
            player.controller.Move(player.sprintSpeed * Time.deltaTime * moveDirection);
        }
        else
        {
            if (player.moveAmount > 0.5f)
            {
                // running speed
                player.controller.Move(player.runningSpeed * Time.deltaTime * moveDirection);
            }
            else if (player.moveAmount <= 0.5f)
            {
                // walking speed
                player.controller.Move(player.walkingSpeed * Time.deltaTime * moveDirection);
            }
        }
    }

    private void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(player.transform.position, groundCheckSphereRadius, LayerMaskManager.Instance.groundLayerMask);

        player.playerAnimatorManager.IsGrounded = isGrounded;

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
            if (player.characterStateMachine.currentState != player.jumpState && !fallingVelocitySet)
            {
                fallingVelocitySet = true;
                yVelocity.y = fallStartYVelocity;
            }

            inAirTime += Time.deltaTime;
            player.playerAnimatorManager.InAirTime = inAirTime;

            yVelocity.y += gravityForce * Time.deltaTime;
        }

        player.controller.Move(yVelocity * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (!player.canRotate)
            return;

        if (player.isLockedOn)
        {
            if (!player.currentLockedOnTarget)
                return;

            lockedTargetDirection = Vector3.zero;
            lockedTargetDirection = player.currentLockedOnTarget.transform.position - player.transform.position;
            lockedTargetDirection.y = 0f;
            lockedTargetDirection.Normalize();

            targetRotation = Quaternion.LookRotation(lockedTargetDirection);
            finalRotation = Quaternion.Slerp(player.transform.rotation, targetRotation, player.rotationDampTime * Time.deltaTime);
            player.transform.rotation = finalRotation;
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
                targetRotationDirection = player.transform.forward;
            }

            targetRotation = Quaternion.LookRotation(targetRotationDirection);
            finalRotation = Quaternion.Slerp(player.transform.rotation, targetRotation, player.rotationDampTime * Time.deltaTime);
            player.transform.rotation = finalRotation;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, groundCheckSphereRadius);
    }
}
