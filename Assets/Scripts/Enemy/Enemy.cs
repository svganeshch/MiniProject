using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    float moveAmount;

    [Header("Enemy Controls")]
    public float detectionRadius = 15;
    public Transform targetLock;
    public Character currentTarget;

    // Unity components
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public EnemyAnimatorManager enemyAnimatorManager;
    [HideInInspector] public EnemySfxManager enemySfxManager;

    // Enemy Specefic States
    [HideInInspector] public EnemyAttackState attackState;

    // WeaponAttack scripts
    public WeaponAttack leftWeaponAttack;
    public WeaponAttack rightWeaponAttack;

    protected override void Start()
    {
        base.Start();

        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
        enemySfxManager = GetComponent<EnemySfxManager>();

        idleState = new EnemyIdleState(this, characterStateMachine);
        combatState = new EnemyCombatState(this, characterStateMachine);
        attackState = new EnemyAttackState(this, characterStateMachine);
        liteAttackState = new EnemyLiteAttackState(this, characterStateMachine);
        blockState = new BlockState(this, characterStateMachine);
        blockBrokenState = new BlockBrokenState(this, characterStateMachine);
        hitState = new EnemyHitState(this, characterStateMachine);
        characterStateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        characterStateMachine.currentState.LogicUpdate();
        HandleMovement();
    }

    public void HandleMovement()
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(controller.velocity.z) + Mathf.Abs(controller.velocity.x));

        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5f && moveAmount <= 1)
        {
            moveAmount = 1;
        }

        characterAnimatorManager.SetAnimatorParameters(0, moveAmount);
    }

    public void StartDamage(string attackHand)
    {
        if (attackHand != null)
        {
            switch (attackHand)
            {
                case "left":
                    leftWeaponAttack.StartDealDamage();
                    break;
                case "right":
                    rightWeaponAttack.StartDealDamage();
                    break;
            }
        }

        //Debug.Log("attackHand " +  attackHand);
    }

    public void StopDamage(string attackHand)
    {
        if (attackHand != null)
        {
            switch (attackHand)
            {
                case "left":
                    leftWeaponAttack.StopDealDamage();
                    break;
                case "right":
                    rightWeaponAttack.StopDealDamage();
                    break;
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
