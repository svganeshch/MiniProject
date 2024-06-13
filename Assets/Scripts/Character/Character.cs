using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [HideInInspector]
    public bool isDead = false;
    [HideInInspector]
    public bool isLockedOn = false;
    [HideInInspector]
    public Enemy currentLockedOnTarget;
    [HideInInspector]
    public float moveAmount;

    [Header("Animation Control Flags")]
    public bool applyRootMotion = false;
    public bool isPivoting = false;
    public bool canRotate = true;
    public bool canMove = true;

    [Header("Animation Smoothing")]
    [Range(0, 1)]
    public float speedDampTime = 0.1f;
    [Range(0, 50)]
    public float rotationDampTime = 15f;
    [Range(0, 0.5f)]
    public float animationFadeTime = 0.2f;
    [Range(0, 1)]
    public float airControl = 0.5f;

    [Header("Character controls")]
    public float walkingSpeed = 2.5f;
    public float runningSpeed = 5f;
    public float combatSpeed = 6f;
    public float sprintSpeed = 10f;

    //Unity Components
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public CharacterController controller;
    [HideInInspector]
    public CharacterAnimatorManager characterAnimatorManager;
    [HideInInspector]
    public CharacterMovementManager characterMovementManager;
    [HideInInspector]
    public CharacterSfxManager characterSfxManager;
    [HideInInspector]
    public HealthManager healthManager;
    [HideInInspector]
    public HudManager hudManager;
    [HideInInspector]
    public WeaponEquipment weaponEquipment;

    //Character States
    [HideInInspector]
    public StateMachine characterStateMachine;
    [HideInInspector]
    public State idleState;
    [HideInInspector]
    public State combatState;
    [HideInInspector]
    public State liteAttackState;
    [HideInInspector]
    public State heavyAttackState;
    [HideInInspector]
    public State blockState;
    [HideInInspector]
    public State blockBrokenState;
    [HideInInspector]
    public State hitState;
    [HideInInspector]
    public State jumpState;
    [HideInInspector]
    public State sprintState;
    [HideInInspector]
    public State dodgeState;

    protected virtual void Awake()
    {
        Debug.Log($"{gameObject.name} instance created");
    }

    protected virtual void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
        characterMovementManager = GetComponent<CharacterMovementManager>();
        characterSfxManager = GetComponent<CharacterSfxManager>();
        healthManager = GetComponent<HealthManager>();
        hudManager = GetComponentInChildren<HudManager>();
        weaponEquipment = GetComponent<WeaponEquipment>();

        characterStateMachine = new StateMachine();

        IgnoreMyOwnColliders();
    }

    protected virtual void Update()
    {
        characterStateMachine.currentState.HandleInput();
        characterStateMachine.currentState.LogicUpdate();
    }

    protected virtual void FixedUpdate()
    {
        characterStateMachine.currentState.PhysicsUpdate();
    }

    protected virtual void LateUpdate() { }

    private void IgnoreMyOwnColliders()
    {
        Collider characterControllerCollider = GetComponent<Collider>();
        Collider[] damagableCharacterColliders = GetComponentsInChildren<Collider>();
        List<Collider> ignoreColliders = new List<Collider>();

        foreach (var collider in damagableCharacterColliders)
        {
            ignoreColliders.Add(collider);
        }
        ignoreColliders.Add(characterControllerCollider);

        foreach (var collider in ignoreColliders)
        {
            foreach (var otherCollider in ignoreColliders)
            {
                Physics.IgnoreCollision(collider, otherCollider, true);
            }
        }
    }

    public virtual void OnGUI() { }
}
