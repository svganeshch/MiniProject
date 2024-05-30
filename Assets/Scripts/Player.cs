using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
    public static Player Instance { get; private set; }

    [Header("Character controls")]
    public float walkingSpeed = 2.5f;
    public float runningSpeed = 5f;
    public float combatSpeed = 6f;
    public float sprintSpeed = 10f;
    public float jumpHeight = 0.8f;
    public float gravityMultiplier = 2f;
    public float attackCancelTreshold = 0.3f;
    public float attackComboTreshold = 0.9f;
    public float weaponSwapSlowTime = 0.5f;
    public Transform targetLockCast;
    public Transform playerFollow;

    [HideInInspector]
    public Vector3 playerVelocity;

    // Unity components
    [HideInInspector]
    public PlayerInput playerInput;

    // Player States
    [HideInInspector]
    public State jumpState;
    [HideInInspector]
    public State landState;
    [HideInInspector]
    public State sprintState;
    [HideInInspector]
    public State sprintJumpState;
    [HideInInspector]
    public State dodgeState;

    // Input Actions
    [HideInInspector]
    public InputAction moveAction;
    [HideInInspector]
    public InputAction jumpAction;
    [HideInInspector]
    public InputAction sprintAction;
    [HideInInspector]
    public InputAction dodgeAction;
    [HideInInspector]
    public InputAction drawWeaponAction;
    [HideInInspector]
    public InputAction blockAction;
    [HideInInspector]
    public InputAction liteAttackWeaponAction;
    [HideInInspector]
    public InputAction heavyAttackWeaponAction;
    [HideInInspector]
    public InputAction lockOnAction;
    [HideInInspector]
    public InputAction leftLockOnAction;
    [HideInInspector]
    public InputAction rightLockOnAction;
    [HideInInspector]
    public InputAction weapon1Action;
    [HideInInspector]
    public InputAction weapon2Action;
    [HideInInspector]
    public InputAction weapon3Action;
    [HideInInspector]
    public InputAction weapon4Action;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = this;
            Debug.Log($"{gameObject.name} instance created");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} instance already exists, destroying new instance");
            Destroy(gameObject);
        }
    }

    protected override void Start()
    {
        base.Start();

        // Unity components
        playerInput = GetComponent<PlayerInput>();

        // FSM
        idleState = new IdleState(this, characterStateMachine);
        jumpState = new JumpState(this, characterStateMachine);
        landState = new LandState(this, characterStateMachine);
        sprintState = new SprintState(this, characterStateMachine);
        sprintJumpState = new SprintJumpState(this, characterStateMachine);
        dodgeState = new DodgeState(this, characterStateMachine);
        combatState = new CombatState(this, characterStateMachine);
        blockState = new BlockState(this, characterStateMachine);
        blockBrokenState = new BlockBrokenState(this, characterStateMachine);
        liteAttackState = new LiteAttackState(this, characterStateMachine);
        heavyAttackState = new HeavyAttackState(this, characterStateMachine);
        hitState = new HitState(this, characterStateMachine);
        characterStateMachine.Initialize(idleState);

        // Input actions
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
        dodgeAction = playerInput.actions["Dodge"];
        drawWeaponAction = playerInput.actions["DrawWeapon"];
        blockAction = playerInput.actions["Block"];
        liteAttackWeaponAction = playerInput.actions["LiteAttack"];
        heavyAttackWeaponAction = playerInput.actions["HeavyAttack"];
        lockOnAction = playerInput.actions["LockOn"];
        leftLockOnAction = playerInput.actions["LeftLockOn"];
        rightLockOnAction = playerInput.actions["RightLockOn"];

        weapon1Action = playerInput.actions["Weapon1"];
        weapon2Action = playerInput.actions["Weapon2"];
        weapon3Action = playerInput.actions["Weapon3"];
        weapon4Action = playerInput.actions["Weapon4"];

        GRAVITY_VALUE *= gravityMultiplier;
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        PlayerCamera.Instance.CameraActions();
    }

    public override void OnGUI()
    {
        base.OnGUI();

        GUI.color = Color.red;
        GUI.Label(new Rect(0, 0, 200, 20), this.GetType().Name + " : " + characterStateMachine.currentState.ToString());
    }
}
