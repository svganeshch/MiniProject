using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    [HideInInspector] public bool isInCoolDown = false;
    [HideInInspector] public bool isInstantAttack = false;
    [HideInInspector] public float attackCoolDownTimer = 0;
    [HideInInspector] public float recallTimer = 0;

    [Header("Enemy Controls")]
    public float detectionRadius = 15;
    public float recallDistance = 10;
    public float minimumFOV = -35;
    public float maximumFOV = 35;
    public Player currentTarget;

    [Header("Enemy Movement Smoothing Controls")]
    public float slowDownThreshold = 5;
    public float minSpeedFactor = 0.3f;

    [Header("Enemy Attack Controls")]
    public float attackSpeed = 8;
    public float attackCoolDownDuration = 5;
    public float attackRange = 5f;
    public float instantAttackRange = 2f;
    public float attackProbability = 0.5f;
    public float attackComboProbability = 0.35f;

    [Header("Enemy Sprint Controls")]
    public float sprintDistance = 15;
    public float sprintSlowDownThreshold = 8;

    [Header("Enemy Strafe Controls")]
    public float strafeSpeed = 0.75f;
    public float strafeSlowDownThreshold = 4;
    public float strafeAngle = 45;
    public float strafeAttackDuration = 5f;

    [Header("Enemy Patrol Points")]
    public List<Vector3> patrolPoints = new List<Vector3>();

    // Unity components
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public EnemyAnimatorManager enemyAnimatorManager;
    [HideInInspector] public EnemyMovementManager enemyMovementManager;
    [HideInInspector] public EnemySfxManager enemySfxManager;

    // Enemy Specific States
    [HideInInspector] public EnemyAttackState attackState;
    [HideInInspector] public StrafeState strafeState;

    protected override void Awake()
    {
        base.Awake();

        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected override void Start()
    {
        base.Start();

        hudManager = GetComponentInChildren<EnemyHudManager>();

        enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
        enemyMovementManager = GetComponent<EnemyMovementManager>();
        enemySfxManager = GetComponent<EnemySfxManager>();

        InitializeStates();

        StartCoroutine(UpdateNavMeshDestinationCoroutine());
    }

    //protected override void Update()
    //{
    //    base.Update();

    //    if (characterStateMachine.currentState != idleState && currentTarget != null)
    //        navMeshAgent.destination = currentTarget.transform.position;
    //}

    private IEnumerator UpdateNavMeshDestinationCoroutine()
    {
        while (true)
        {
            if (characterStateMachine.currentState != idleState && currentTarget != null)
            {
                navMeshAgent.destination = currentTarget.transform.position;

                HandleInstantAttack();
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    protected override void InitializeStates()
    {
        idleState = new EnemyIdleState(this, characterStateMachine);
        combatState = new EnemyCombatState(this, characterStateMachine);
        attackState = new EnemyAttackState(this, characterStateMachine);
        liteAttackState = new EnemyLiteAttackState(this, characterStateMachine);
        blockState = new BlockState(this, characterStateMachine);
        blockBrokenState = new BlockBrokenState(this, characterStateMachine);
        hitState = new EnemyHitState(this, characterStateMachine);
        sprintState = new EnemySprintState(this, characterStateMachine);
        strafeState = new StrafeState(this, characterStateMachine);

        // Set initial state
        characterStateMachine.Initialize(idleState);
    }

    public void HandleInstantAttack()
    {
        if (isInCoolDown) return;

        if (!isInstantAttack)
        {
            if (navMeshAgent.remainingDistance <= instantAttackRange)
            {
                isInstantAttack = true;
                characterStateMachine.ChangeState(attackState);
                return;
            }
        }
    }

    public void HandleAttackCoolDown()
    {
        attackCoolDownTimer += Time.deltaTime;

        if (attackCoolDownTimer >= attackCoolDownDuration)
        {
            isInCoolDown = false;
        }
    }

    public void HandleRecallDistanceChecks()
    {
        recallTimer += Time.deltaTime;

        if (recallTimer >= 5)
        {
            if (navMeshAgent.remainingDistance >= recallDistance)
            {
                enemyAnimatorManager.PlayWeaponHolsterAction();

                currentTarget = null;
                characterStateMachine.ChangeState(idleState);
            }
        }
    }

    public override void OnGUI()
    {
        base.OnGUI();

        GUI.color = Color.black;
        GUI.Label(new Rect(0, 20, 200, 20), this.GetType().Name + " : " + characterStateMachine.currentState.ToString());
    }
}