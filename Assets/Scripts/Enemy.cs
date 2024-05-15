using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Controls")]
    [SerializeField] public float detectionRadius = 15;
    [SerializeField] public Transform targetLock;
    [HideInInspector] public bool isDead = false;

    // Unity components
    [HideInInspector] public Animator animator;
    [HideInInspector] public NavMeshAgent navMesh;

    // Enemy FSM
    [HideInInspector] public EnemyStateMachine enemyStateMachine;
    [HideInInspector] public EnemyIdleState idleState;
    [HideInInspector] public EnemyPursueState pursueState;
    [HideInInspector] public EnemyAttackState attackState;

    // WeaponAttack scripts
    public WeaponAttack[] weaponAttacks;

    private void Start()
    {
        animator = GetComponent<Animator>();
        navMesh = GetComponentInChildren<NavMeshAgent>();

        enemyStateMachine = new EnemyStateMachine();
        idleState = new EnemyIdleState(this, enemyStateMachine);
        pursueState = new EnemyPursueState(this, enemyStateMachine);
        attackState = new EnemyAttackState(this, enemyStateMachine);
        enemyStateMachine.Initialize(idleState);

        weaponAttacks = GetComponentsInChildren<WeaponAttack>();

        IgnoreMyOwnColliders();
    }

    private void Update()
    {
        enemyStateMachine.currentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        enemyStateMachine.currentState.PhysicsUpdate();
    }

    public void StartDamage()
    {
        foreach (var weapon in weaponAttacks)
        {
            weapon.StartDealDamage();
        }
    }

    public void StopDamage()
    {
        foreach (var weapon in weaponAttacks)
        {
            weapon.EndDealDamage();
        }
    }

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

    void OnGUI()
    {
        GUI.color = Color.black;
        GUI.Label(new Rect(0, 20, 200, 20), enemyStateMachine.currentState.ToString());
    }
}
