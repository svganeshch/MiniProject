using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance;

    public Transform playerCameraObjTransform;
    public Transform mainCameraTransform;
    public Transform cameraPivotTransform;
    public Transform followTarget;
    public Player player;
    public PlayerInput playerInput;

    private InputDevice currentDevice;
    private InputDevice sensDevice;

    [Header("Camera Settings")]
    private float upDownRotationSpeed = 0;
    private float leftRightRotationSpeed = 0;

    [Range(0.0f, 0.5f)]
    public float cameraSmoothSpeed = 1.0f;
    [Range(0.0f, 10f)]
    public float cameraPullSpeed = 1.0f;
    [Range(0.0f, 250f)]
    public float MouseYSensitivity = 40;
    [Range(0.0f, 250f)]
    public float MouseXSensitivity = 40;
    [Range(0.0f, 250f)]
    public float GamepadYSensitivity = 220;
    [Range(0.0f, 250f)]
    public float GamepadXSensitivity = 220;

    public float minimumPivot = -30;
    public float maximumPivot = 60;
    public float cameraCollisionRadius = 0.2f;

    [Header("Camera values")]
    private Vector3 cameraVelocity;
    private Vector3 cameraObjPosition;
    private float leftRightLookAngle;
    private float upDownLookAngle;
    private float cameraZPosition;
    private float targetCameraZPosition;

    [Header("Camera Input")]
    private InputAction lookAction;
    private Vector2 lookInput;
    private float cameraVerticalInput;
    private float cameraHorizontalInput;

    [Header("Lock On")]
    [Range(0.0f, 50f)]
    [SerializeField] private float lockOnRadius = 20f;
    [Range(0.0f, -180f)]
    [SerializeField] private float minimumViewableAngle = -50;
    [Range(0.0f, 180f)]
    [SerializeField] private float maximumViewableAngle = 50;
    [Range(0.0f, 50f)]
    [SerializeField] private float lockOnTargetFollowSpeed = 15f;
    [Range(0.0f, 0.5f)]
    [SerializeField] private float setCameraHeightSpeed = 0.05f;
    [Range(0, 150)]
    [SerializeField] public float mouseLockOnSwitchTreshold = 50;
    [Range(0.0f, 10f)]
    [SerializeField] private float unlockedCameraHeight = 1.5f;
    [Range(0.0f, 10f)]
    [SerializeField] private float lockedCameraHeight = 2.0f;

    private Coroutine cameraLockHeightCoroutine;
    private List<Enemy> availableTargets = new List<Enemy>();
    [HideInInspector] public Enemy nearestTarget;
    [HideInInspector] public Enemy leftLockOnTarget;
    [HideInInspector] public Enemy rightLockOnTarget;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        playerInput = player.GetComponent<PlayerInput>();

        cameraZPosition = mainCameraTransform.localPosition.z;
        lookAction = playerInput.actions["CameraLook"];

        lookAction.performed += input => lookInput = input.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (lookAction != null)
        {
            cameraVerticalInput = lookInput.y;
            cameraHorizontalInput = lookInput.x;
        }

        SetInputDeviceSensitivity();
    }

    private void SetInputDeviceSensitivity()
    {
        currentDevice = HelperFunctions.instance.GetCurrenInputDevice();

        if (currentDevice == sensDevice)
            return;

        if (currentDevice != null)
        {
            if (currentDevice is Keyboard)
            {
                Debug.Log("setting mouse sens");
                leftRightRotationSpeed = MouseXSensitivity;
                upDownRotationSpeed = MouseYSensitivity;
            }
            else if (currentDevice is Gamepad)
            {
                Debug.Log("setting gamepad sens");
                leftRightRotationSpeed = GamepadXSensitivity;
                upDownRotationSpeed = GamepadYSensitivity;
            }

            sensDevice = currentDevice;
        }
    }

    public void CameraActions()
    {
        HandleFollowTarget();
        HandleRotations();
        HandleCollisions();
    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(playerCameraObjTransform.position, followTarget.position, ref cameraVelocity, cameraSmoothSpeed);
        playerCameraObjTransform.position = targetCameraPosition;
    }

    private void HandleRotations()
    {
        if (player.isLockedOn)
        {
            HandleLockedOnRotations();
        }
        else
        {
            HandleFreeRotations();
        }
    }

    private void HandleLockedOnRotations()
    {
        if (player.currentLockedOnTarget == null) return;

        Vector3 rotationDirection = player.currentLockedOnTarget.targetLockCast.position - playerCameraObjTransform.position;
        rotationDirection.Normalize();
        rotationDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
        playerCameraObjTransform.rotation = Quaternion.Slerp(playerCameraObjTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

        rotationDirection = player.currentLockedOnTarget.targetLockCast.position - cameraPivotTransform.position;
        rotationDirection.Normalize();
        targetRotation = Quaternion.LookRotation(rotationDirection);
        cameraPivotTransform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

        leftRightLookAngle = playerCameraObjTransform.eulerAngles.y;
        upDownLookAngle = playerCameraObjTransform.eulerAngles.x;
    }

    private void HandleFreeRotations()
    {
        leftRightLookAngle += (cameraHorizontalInput * leftRightRotationSpeed) * Time.deltaTime;
        upDownLookAngle -= (cameraVerticalInput * upDownRotationSpeed) * Time.deltaTime;
        upDownLookAngle = Mathf.Clamp(upDownLookAngle, minimumPivot, maximumPivot);

        Vector3 cameraRotation = Vector3.zero;
        cameraRotation.y = leftRightLookAngle;
        playerCameraObjTransform.rotation = Quaternion.Euler(cameraRotation);

        cameraRotation = Vector3.zero;
        cameraRotation.x = upDownLookAngle;
        cameraPivotTransform.localRotation = Quaternion.Euler(cameraRotation);
    }

    private void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition;
        RaycastHit hit;
        Vector3 direction = mainCameraTransform.position - cameraPivotTransform.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), LayerMaskManager.Instance.obstaclesLayerMask))
        {
            float distanceFromObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetCameraZPosition = -(distanceFromObject - cameraCollisionRadius);
        }

        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        cameraObjPosition.z = Mathf.Lerp(mainCameraTransform.localPosition.z, targetCameraZPosition, cameraPullSpeed * Time.deltaTime);
        mainCameraTransform.localPosition = cameraObjPosition;
    }

    public void FindLockOnTarget()
    {
        float shortestDistance = Mathf.Infinity;
        float shortestDistanceOfRightTarget = Mathf.Infinity;
        float shortestDistanceOfLeftTarget = -Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, LayerMaskManager.Instance.enemyLayerMask);

        foreach (var collider in colliders)
        {
            if (!collider.TryGetComponent<Enemy>(out var lockOnTarget))
                continue;

            if (availableTargets.Contains(lockOnTarget) || lockOnTarget.isDead)
                continue;

            Vector3 lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;
            float viewableAngle = Vector3.Angle(lockOnTargetDirection, transform.forward);

            if (viewableAngle < minimumViewableAngle || viewableAngle > maximumViewableAngle)
                continue;

            if (Physics.Linecast(player.targetLockCast.position, lockOnTarget.targetLockCast.transform.position, out RaycastHit hit, LayerMaskManager.Instance.obstaclesLayerMask))
                continue;

            availableTargets.Add(lockOnTarget);
        }

        foreach (var target in availableTargets)
        {
            if (target == null)
            {
                ClearLockOnTargets();
                player.isLockedOn = false;
                continue;
            }

            float distanceFromTarget = Vector3.Distance(player.transform.position, target.transform.position);
            if (distanceFromTarget < shortestDistance)
            {
                shortestDistance = distanceFromTarget;
                nearestTarget = target;
            }

            if (player.isLockedOn)
            {
                Vector3 relativeTargetPosition = playerCameraObjTransform.InverseTransformPoint(target.transform.position);
                if (relativeTargetPosition.x <= 0.00 && relativeTargetPosition.x > shortestDistanceOfLeftTarget)
                {
                    shortestDistanceOfLeftTarget = relativeTargetPosition.x;
                    leftLockOnTarget = target;
                }
                else if (relativeTargetPosition.x >= 0.00 && relativeTargetPosition.x < shortestDistanceOfRightTarget)
                {
                    shortestDistanceOfRightTarget = relativeTargetPosition.x;
                    rightLockOnTarget = target;
                }
            }
        }
    }

    public void ClearLockOnTargets()
    {
        nearestTarget = null;
        leftLockOnTarget = null;
        rightLockOnTarget = null;
        availableTargets.Clear();
    }

    public IEnumerator WaitFindNewTarget()
    {
        ClearLockOnTargets();
        FindLockOnTarget();

        if (nearestTarget != null)
        {
            player.combatState.SetTarget(nearestTarget);
            player.isLockedOn = true;
        }

        yield return null;
    }

    public void SetLockOnCameraHeight()
    {
        if (cameraLockHeightCoroutine != null)
            StopCoroutine(cameraLockHeightCoroutine);

        cameraLockHeightCoroutine = StartCoroutine(SetCameraHeight());
    }

    private IEnumerator SetCameraHeight()
    {
        float duration = 1;
        float timer = 0;

        Vector3 velocity = Vector3.zero;
        Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.localPosition.x, lockedCameraHeight);
        Vector3 newUnlockedCameraHeight = new Vector3(cameraPivotTransform.localPosition.x, unlockedCameraHeight);

        while (timer < duration)
        {
            timer += Time.deltaTime;

            if (player.currentLockedOnTarget != null)
            {
                cameraPivotTransform.localPosition =
                    Vector3.SmoothDamp(cameraPivotTransform.localPosition, newLockedCameraHeight, ref velocity, setCameraHeightSpeed);

                cameraPivotTransform.localRotation =
                    Quaternion.Slerp(cameraPivotTransform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
            }
            else
            {
                cameraPivotTransform.localPosition =
                    Vector3.SmoothDamp(cameraPivotTransform.localPosition, newUnlockedCameraHeight, ref velocity, setCameraHeightSpeed);
            }

            yield return null;
        }

        if (player.currentLockedOnTarget != null)
        {
            cameraPivotTransform.localPosition = newLockedCameraHeight;
            cameraPivotTransform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            cameraPivotTransform.localPosition = newUnlockedCameraHeight;
        }

        yield return null;
    }
}