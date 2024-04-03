using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    public static Character instance;

    [Header("Character controls")]
    public float health = 100;
    public float walkingSpeed = 2.5f;
    public float runningSpeed = 5f;
    public float combatSpeed = 6f;
    public float sprintSpeed = 10f;
    public float jumpHeight = 0.8f;
    public float gravityMultiplier = 2f;
    public float attackCancelTreshold = 0.3f;
    public float attackComboTreshold = 0.9f;
    public Transform targetLockCast;
    public Transform playerFollow;

    [Header("Layer Masks")]
    public LayerMask playerLayerMask;
    public LayerMask enemyLayerMask;
    public LayerMask obstaclesLayerMask;

    [Header("Animation Smoothing")]
    [Range(0, 1)]
    public float speedDampTime = 0.1f;
    [Range(0, 50)]
    public float rotationDampTime = 15f;
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
    public WeaponEquipment weaponEquipment;

    //FSM
    [HideInInspector]
    public StateMachine characterMovementSM;
    [HideInInspector]
    public IdleState idleState;
    [HideInInspector]
    public JumpState jumpState;
    [HideInInspector]
    public LandState landState;
    [HideInInspector]
    public SprintState sprintState;
    [HideInInspector]
    public SprintJumpState sprintJumpState;
    [HideInInspector]
    public DodgeState dodgeState;
    [HideInInspector]
    public CombatState combatState;
    [HideInInspector]
    public LiteAttackState liteAttackState;
    [HideInInspector]
    public HeavyAttackState heavyAttackState;
    [HideInInspector]
    public HitState hitState;

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
        Cursor.lockState = CursorLockMode.Locked;

        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        weaponEquipment = GetComponent<WeaponEquipment>();
        
        characterMovementSM = new StateMachine();
        idleState = new IdleState(this, characterMovementSM);
        jumpState = new JumpState(this, characterMovementSM);
        landState = new LandState(this, characterMovementSM);
        sprintState = new SprintState(this, characterMovementSM);
        sprintJumpState = new SprintJumpState(this, characterMovementSM);
        dodgeState = new DodgeState(this, characterMovementSM);
        combatState = new CombatState(this, characterMovementSM);
        liteAttackState = new LiteAttackState(this, characterMovementSM);
        heavyAttackState = new HeavyAttackState(this, characterMovementSM);
        hitState = new HitState(this, characterMovementSM);
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

    private void LateUpdate()
    {
        PlayerCamera.instance.CameraActions();
    }

    public void TakeDamage(float weaponDamage)
    {
        health -= weaponDamage;

        if (health <= 0)
        {
            //Die();
            Debug.Log("Player dead");
        }
        Debug.Log("Player received damage");

        characterMovementSM.ChangeState(hitState);
    }

    void OnGUI()
    {
        GUI.color = Color.red;
        GUI.Label(new Rect(0, 0, 200, 20), characterMovementSM.currentState.ToString());
    }
}
