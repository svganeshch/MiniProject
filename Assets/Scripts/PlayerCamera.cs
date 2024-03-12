using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;

    public GameObject cameraObj;
    public Transform cameraPivotTransform;
    public Transform followTarget;
    public PlayerInput playerInput;

    [Header("Camera Settings")]
    [Range(0.0f, 10f)]
    public float cameraSmoothSpeed = 1.0f;
    [Range(0.0f, 220f)]
    public float upDownRotationSpeed = 220;
    [Range(0.0f, 220f)]
    public float leftRightRotationSpeed = 220;
    public float minimumPivot = -30;
    public float maximumPivot = 60;
    public float cameraCollisionRadius = 0.2f;
    public LayerMask obstacleLayers;

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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
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
    }

    public void CameraActions()
    {
        HandleFollowTarget();
        HandleRotations();
        //HandleCollisions();
    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, followTarget.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleRotations()
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

    private void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition;
        RaycastHit hit;
        Vector3 direction = cameraObj.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), obstacleLayers))
        {
            float distanceFromObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetCameraZPosition = -(distanceFromObject - cameraCollisionRadius);
        }

        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        cameraObjPosition.z = Mathf.Lerp(cameraObj.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObj.transform.localPosition = cameraObjPosition;
    }
}
