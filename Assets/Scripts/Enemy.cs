using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    [Header("Enemy Controls")]
    [SerializeField] public float detectionRadius = 15;
    [SerializeField] public Transform targetLock;
    public Character currentTarget;

    // Unity components
    [HideInInspector] public NavMeshAgent navMesh;

    // Enemy Specefic States
    [HideInInspector] public EnemyAttackState attackState;

    // WeaponAttack scripts
    public WeaponAttack leftWeaponAttack;
    public WeaponAttack rightWeaponAttack;

    protected override void Start()
    {
        base.Start();

        navMesh = GetComponentInChildren<NavMeshAgent>();

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
