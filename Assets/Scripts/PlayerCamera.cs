using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera Instance;

    public GameObject cameraObj;
    public Transform cameraPivotTransform;
    public Transform followTarget;
    public Player player;
    public PlayerInput playerInput;

    private InputDevice currentDevice;
    private InputDevice sensDevice;

    [Header("Camera Settings")]
    private float upDownRotationSpeed = 0;
    private float leftRightRotationSpeed = 0;

    [Range(0.0f, 10f)]
    public float cameraSmoothSpeed = 1.0f;
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
    Vector3 cameraVelocity;
    Vector3 cameraObjPosition;
    float leftRightLookAngle;
    float upDownLookAngle;
    float cameraZPosition;
    float targetCameraZPosition;

    [Header("Camera Input")]
    private InputAction lookAction;
    private Vector2 lookInput;
    private float cameraVerticalInput;
    private float cameraHorizontalInput;

    [Header("Lock On")]
    [Range(0.0f, 50f)]
    [SerializeField] float lockOnRadius = 20f;
    [Range(0.0f, -180f)]
    [SerializeField] float minimumViewableAngle = -50;
    [Range(0.0f, 180f)]
    [SerializeField] float maximumViewableAngle = 50;
    [Range(0.0f, 50f)]
    [SerializeField] float lockOnTargetFollowSpeed = 15f;
    [Range(0.0f, 1f)]
    [SerializeField] float setCameraHeightSpeed = 0.05f;
    [Range(0, 150)]
    [SerializeField] public float mouseLockOnSwitchTreshold = 50;
    [Range(0.0f, 10f)]
    [SerializeField] float unlockedCameraHeight = 1.5f;
    [Range(0.0f, 10f)]
    [SerializeField] float lockedCameraHeight = 2.0f;

    Coroutine cameraLockHeightCoroutine;
    List<Enemy> availableTargets = new List<Enemy>();
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

        cameraZPosition = cameraObj.transform.localPosition.z;
        lookAction = playerInput.actions["Look"];
    }

    private void Update()
    {
        if (lookAction != null)
        {
            lookInput = lookAction.ReadValue<Vector2>();
            cameraVerticalInput = lookInput.y;
            cameraHorizontalInput = lookInput.x;

            //Debug.Log(lookInput);
        }

        SetInputDeviceSensitivity();
    }

    private void SetInputDeviceSensitivity()
    {
        currentDevice = playerInput.devices[0];

        if (currentDevice == sensDevice)
        {
            return;
        }

        if (currentDevice != null)
        {
            if (currentDevice is Keyboard)
            {
                Debug.Log("setting mouse sens");
                leftRightRotationSpeed = MouseXSensitivity;
                upDownRotationSpeed = MouseYSensitivity;

                sensDevice = currentDevice;
            }
            else if (currentDevice is Gamepad)
            {
                Debug.Log("setting gamepad sens");
                leftRightRotationSpeed = GamepadXSensitivity;
                upDownRotationSpeed = GamepadYSensitivity;

                sensDevice = currentDevice;
            }
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
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, followTarget.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleRotations()
    {
        if (player.isLockedOn)
        {
            // Right Left Pivot
            Vector3 rotationDirection = player.currentLockedOnTarget.targetLock.position - transform.position;
            rotationDirection.Normalize();
            rotationDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);

            // Up Down Pivot
            rotationDirection = player.currentLockedOnTarget.targetLock.position - cameraPivotTransform.position;
            rotationDirection.Normalize();
            targetRotation = Quaternion.LookRotation(rotationDirection);
            cameraPivotTransform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

            leftRightLookAngle = transform.eulerAngles.y;
            upDownLookAngle = transform.eulerAngles.x;
        }
        else
        {
            leftRightLookAngle += (cameraHorizontalInput * leftRightRotationSpeed) * Time.deltaTime;
            upDownLookAngle -= (cameraVerticalInput * upDownRotationSpeed) * Time.deltaTime;
            upDownLookAngle = Mathf.Clamp(upDownLookAngle, minimumPivot, maximumPivot);

            Vector3 cameraRotation = Vector3.zero;
            Quaternion targetRotation;

            cameraRotation.y = leftRightLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            transform.rotation = targetRotation;

            cameraRotation = Vector3.zero;
            cameraRotation.x = upDownLookAngle;
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }
    }

    private void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition;
        RaycastHit hit;
        Vector3 direction = cameraObj.transform.position - cameraPivotTransform.position;
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

        cameraObjPosition.z = Mathf.Lerp(cameraObj.transform.localPosition.z, targetCameraZPosition, 0.2f * Time.deltaTime);
        cameraObj.transform.localPosition = cameraObjPosition;
    }

    public void FindLockOnTarget()
    {
        float shortestDistance = Mathf.Infinity;
        float shortestDistanceOfRightTarget = Mathf.Infinity;
        float shortestDistanceOfLeftTarget = -Mathf.Infinity;

        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, LayerMaskManager.Instance.enemyLayerMask);

        for (int i = 0; i < colliders.Length; i++)
        {

            if (colliders[i].TryGetComponent<Enemy>(out var lockOnTarget))
            {
                Vector3 lockOnTargetDirection = lockOnTarget.transform.position - player.transform.position;
                float viewableAngle = Vector3.Angle(lockOnTargetDirection, PlayerCamera.Instance.transform.forward);

                if (lockOnTarget.isDead)
                    continue;

                if (viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)
                {
                    RaycastHit hit;

                    if (Physics.Linecast(player.targetLockCast.position, lockOnTarget.targetLock.transform.position, out hit, LayerMaskManager.Instance.obstaclesLayerMask))
                    {
                        continue;
                    }
                    else
                    {
                        availableTargets.Add(lockOnTarget);
                        //Debug.Log("available target : " + lockOnTarget.name);
                    }
                }
            }
        }

        for (int i = 0; i < availableTargets.Count; i++)
        {
            if (availableTargets[i] != null)
            {
                float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[i].transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestTarget = availableTargets[i];

                    //Debug.Log("nearest target : " + nearestTarget.name);
                }

                if (player.isLockedOn)
                {
                    Vector3 relativeTargetPosition = transform.InverseTransformPoint(availableTargets[i].transform.position);

                    var distanceFromLeftTarget = relativeTargetPosition.x;
                    var distanceFromRightTarget = relativeTargetPosition.x;

                    if (availableTargets[i] == player.currentLockedOnTarget)
                        continue;

                    if (relativeTargetPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockOnTarget = availableTargets[i];
                    }
                    else if (relativeTargetPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockOnTarget = availableTargets[i];
                    }
                }
            }
            else
            {
                ClearLockOnTargets();
                player.isLockedOn = false;
            }
        }
    }

    public void SetLockOnCameraHeight()
    {
        if (cameraLockHeightCoroutine != null)
            StopCoroutine(cameraLockHeightCoroutine);

        cameraLockHeightCoroutine = StartCoroutine(SetCameraHeight());
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

    private IEnumerator SetCameraHeight()
    {
        float duration = 1;
        float timer = 0;

        Vector3 velocity = Vector3.zero;
        Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight);
        Vector3 newUnlockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, unlockedCameraHeight);

        while (timer < duration)
        {
            timer += Time.deltaTime;

            if (player.currentLockedOnTarget != null)
            {
                cameraPivotTransform.transform.localPosition =
                    Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedCameraHeight, ref velocity, setCameraHeightSpeed);

                //cameraPivotTransform.transform.localRotation =
                //    Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed * Time.deltaTime);
            }
            else
            {
                cameraPivotTransform.transform.localPosition =
                    Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedCameraHeight, ref velocity, setCameraHeightSpeed);
            }

            yield return null;
        }
    }
}
