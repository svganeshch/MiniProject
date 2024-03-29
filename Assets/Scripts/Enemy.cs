using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Controls")]
    [SerializeField] float health = 100;
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
    WeaponAttack[] weaponAttacks;

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

    public void TakeDamage(float weaponDamage)
    {
        health -= weaponDamage;
        animator.SetTrigger("damage");

        if (health <= 0)
        {
            Die();
        }
        Debug.Log("Enemy received damage");
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("isDead");
        Destroy(gameObject, 5f);
    }

    void OnGUI()
    {
        GUI.color = Color.black;
        GUI.Label(new Rect(0, 20, 200, 20), enemyStateMachine.currentState.ToString());
    }
}
