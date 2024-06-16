using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
    public static Player Instance { get; private set; }

    public float jumpHeight = 0.8f;
    public float jumpForwardVelocity = 5f;
    public float freeFallControlVelocity = 2f;
    public float attackCancelTreshold = 0.3f;
    public float attackComboTreshold = 0.9f;
    public float weaponSwapSlowTime = 0.5f;
    public Transform targetLockCast;

    // Input values
    [HideInInspector] public Vector2 inputValues;
    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float verticalInput;

    // Unity components
    [HideInInspector] public PlayerInput playerInput;
    [HideInInspector] public PlayerMovementManager playerMovementManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;

    // Input Actions
    [HideInInspector] public InputAction moveAction;
    [HideInInspector] public InputAction jumpAction;
    [HideInInspector] public InputAction sprintAction;
    [HideInInspector] public InputAction dodgeAction;
    [HideInInspector] public InputAction drawWeaponAction;
    [HideInInspector] public InputAction blockAction;
    [HideInInspector] public InputAction liteAttackWeaponAction;
    [HideInInspector] public InputAction heavyAttackWeaponAction;
    [HideInInspector] public InputAction lockOnAction;
    [HideInInspector] public InputAction leftLockOnAction;
    [HideInInspector] public InputAction rightLockOnAction;
    [HideInInspector] public InputAction weapon1Action;
    [HideInInspector] public InputAction weapon2Action;
    [HideInInspector] public InputAction weapon3Action;
    [HideInInspector] public InputAction weapon4Action;

    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected override void Start()
    {
        base.Start();

        // Unity components
        hudManager = FindObjectOfType<PlayerHudManager>();
        playerInput = GetComponent<PlayerInput>();
        playerMovementManager = GetComponent<PlayerMovementManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();

        // Initialize states
        InitializeStates();

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

        moveAction.performed += input => inputValues = input.ReadValue<Vector2>();
    }

    protected override void Update()
    {
        base.Update();

        // Always get movement values
        HandlePlayerMovementInput();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        PlayerCamera.Instance.CameraActions();
    }

    protected override void InitializeStates()
    {
        idleState = new IdleState(this, characterStateMachine);
        jumpState = new JumpState(this, characterStateMachine);
        sprintState = new SprintState(this, characterStateMachine);
        dodgeState = new DodgeState(this, characterStateMachine);
        combatState = new CombatState(this, characterStateMachine);
        blockState = new BlockState(this, characterStateMachine);
        blockBrokenState = new BlockBrokenState(this, characterStateMachine);
        liteAttackState = new LiteAttackState(this, characterStateMachine);
        heavyAttackState = new HeavyAttackState(this, characterStateMachine);
        hitState = new HitState(this, characterStateMachine);

        // Set initial state
        characterStateMachine.Initialize(idleState);
    }

    private void HandlePlayerMovementInput()
    {
        horizontalInput = inputValues.x;
        verticalInput = inputValues.y;

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5f && moveAmount <= 1)
        {
            moveAmount = 1;
        }

        if (!isLockedOn)
        {
            if (characterStateMachine.currentState == sprintState)
            {
                playerAnimatorManager.SetAnimatorParameters(0, 1.5f);
            }
            else
            {
                playerAnimatorManager.SetAnimatorParameters(0, moveAmount);
            }
        }
        else
        {
            playerAnimatorManager.SetAnimatorParameters(horizontalInput, verticalInput);
        }
    }

    public override void OnGUI()
    {
        base.OnGUI();

        GUI.color = Color.red;
        GUI.Label(new Rect(0, 0, 200, 20), this.GetType().Name + " : " + characterStateMachine.currentState.ToString());
    }
}