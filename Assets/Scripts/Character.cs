using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    [Header("Character controls")]
    public float playerSpeed = 5f;
    public float jumpHeight = 0.8f;
    public float gravityMultiplier = 2f;

    [Header("Animation Smoothing")]
    [Range(0, 1)]
    public float speedDampTime = 0.1f;
    [Range(0, 1)]
    public float velocityDampTime = 0.5f;
    [Range(0, 1)]
    public float rotationDampTime = 0.2f;
    [Range(0, 1)]
    public float airControl = 0.5f;

    [HideInInspector]
    public float GRAVITY_VALUE = -9.81f;
    [HideInInspector]
    public Vector3 playerVelocity;

    //Unity Components
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public CharacterController controller;
    [HideInInspector]
    public PlayerInput playerInput;
    [HideInInspector]
    public Transform cameraTransform;

    //FSM
    [HideInInspector]
    public StateMachine characterMovementSM;
    [HideInInspector]
    public IdleState idleState;
    [HideInInspector]
    public JumpState jumpState;
    [HideInInspector]
    public SprintState sprintState;
    [HideInInspector]
    public SprintJumpState sprintJumpState;

    private void Start()
    {
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        
        characterMovementSM = new StateMachine();
        idleState = new IdleState(this, characterMovementSM);
        jumpState = new JumpState(this, characterMovementSM);
        sprintState = new SprintState(this, characterMovementSM);
        sprintJumpState = new SprintJumpState(this, characterMovementSM);

        characterMovementSM.Initialize(idleState);

        GRAVITY_VALUE *= gravityMultiplier;
    }

    private void Update()
    {
        characterMovementSM.currentState.HandleInput();
        characterMovementSM.currentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        characterMovementSM.currentState.PhysicsUpdate();
    }
}
