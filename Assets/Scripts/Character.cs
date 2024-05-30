using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [HideInInspector]
    public bool isDead = false;
    [HideInInspector]
    public float GRAVITY_VALUE = -9.81f;
    [HideInInspector]
    public bool isLockedOn = false;
    [HideInInspector]
    public Enemy currentLockedOnTarget;

    [Header("Animation Smoothing")]
    [Range(0, 1)]
    public float speedDampTime = 0.1f;
    [Range(0, 50)]
    public float rotationDampTime = 15f;
    [Range(0, 1)]
    public float airControl = 0.5f;

    //Unity Components
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public CharacterController controller;
    [HideInInspector]
    public HealthManager healthManager;
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

    protected virtual void Awake()
    {
        Debug.Log($"{gameObject.name} instance created");
    }

    protected virtual void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        healthManager = GetComponent<HealthManager>();
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
