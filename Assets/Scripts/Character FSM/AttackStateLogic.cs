using UnityEngine;

public class AttackStateLogic : State
{
    bool attack;
    bool dodge;
    bool block;
    private float timePassed;
    private float clipLength;
    private float clipSpeed;
    private float clipTime;
    private float attackCancelTreshold;
    private float attackComboTreshold;

    public AttackStateLogic(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        input = Vector2.zero;
        targetDirection = Vector3.zero;
        moveVelocity = player.playerVelocity;

        attack = false;
        dodge = false;
        block = false;

        timePassed = 0f;

        attackCancelTreshold = player.attackCancelTreshold;
        attackComboTreshold = player.attackComboTreshold;

        //SFXManager.instance.PlayWeaponSound(WeaponEquipment.Instance.GetCurrentWeapon().slashSound);

        character.animator.applyRootMotion = true;
        character.animator.SetFloat("speedY", 0f);
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (player.dodgeAction.triggered)
        {
            dodge = true;
        }

        if (player.blockAction.triggered)
        {
            block = true;
        }

        if (player.liteAttackWeaponAction.triggered)
        {
            attack = true;
        }

        input = player.moveAction.ReadValue<Vector2>();
        verticalInput = input.y;
        horizontalInput = input.x;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        timePassed += Time.deltaTime;

        //Debug.Log(character.animator.GetCurrentAnimatorClipInfo(1)[0].clip.name);

        clipLength = character.animator.GetCurrentAnimatorClipInfo(1)[0].clip.length;
        clipSpeed = character.animator.GetCurrentAnimatorStateInfo(1).speed * character.animator.GetCurrentAnimatorStateInfo(1).speedMultiplier;
        clipTime = clipLength / clipSpeed;

        if (timePassed <= clipTime * attackCancelTreshold)
        {
            moveVelocity = PlayerCamera.Instance.transform.forward * verticalInput;
            moveVelocity += PlayerCamera.Instance.transform.right * horizontalInput;
            moveVelocity.Normalize();
            moveVelocity.y = 0;

            HandleRotation();

            if (dodge)
            {
                dodge = false;
                stateMachine.ChangeState(player.dodgeState);
            }

            if (block)
            {
                block = false;
                stateMachine.ChangeState(player.blockState);
            }
        }

        if (timePassed >= clipTime * attackComboTreshold)
        {
            if (attack)
                stateMachine.ChangeState(this);
        }

        if (timePassed >= clipTime)
        {
            character.animator.SetTrigger("move");
            stateMachine.ChangeState(player.combatState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        character.animator.applyRootMotion = false;

        if (character.isLockedOn)
        {
            if (moveVelocity == Vector3.zero)
            {
                moveVelocity = character.transform.forward;
            }
            character.transform.rotation = Quaternion.LookRotation(moveVelocity);
        }
        else
        {
            character.transform.rotation = Quaternion.LookRotation(targetDirection);
        }
    }
}
