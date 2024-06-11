using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    [HideInInspector] public float recallTimer = 0;

    [Header("Enemy Controls")]
    public float strafeSpeed = 0.75f;
    public float detectionRadius = 15;
    public float recallDistance = 10;
    public float sprintDistance = 15;
    public Transform targetLock;
    public Player currentTarget;

    public Transform[] patrolPoints;

    // Unity components
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public EnemyAnimatorManager enemyAnimatorManager;
    [HideInInspector] public EnemyMovementManager enemyMovementManager;
    [HideInInspector] public EnemySfxManager enemySfxManager;

    // Enemy Specefic States
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

        enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
        enemyMovementManager = GetComponent<EnemyMovementManager>();
        enemySfxManager = GetComponent<EnemySfxManager>();

        idleState = new EnemyIdleState(this, characterStateMachine);
        combatState = new EnemyCombatState(this, characterStateMachine);
        attackState = new EnemyAttackState(this, characterStateMachine);
        liteAttackState = new EnemyLiteAttackState(this, characterStateMachine);
        blockState = new BlockState(this, characterStateMachine);
        blockBrokenState = new BlockBrokenState(this, characterStateMachine);
        hitState = new EnemyHitState(this, characterStateMachine);
        sprintState = new EnemySprintState(this, characterStateMachine);
        strafeState = new StrafeState(this, characterStateMachine);
        characterStateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        characterStateMachine.currentState.LogicUpdate();
    }

    public void CheckRecallDistance()
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
