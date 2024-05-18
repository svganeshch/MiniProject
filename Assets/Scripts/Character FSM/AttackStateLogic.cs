using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateLogic : State
{
    bool attack;
    bool dodge;
    private float timePassed;
    private float clipLength;
    private float clipSpeed;
    private float clipTime;
    private float attackCancelTreshold;
    private float attackComboTreshold;

    private Weapon currentWeapon;

    public AttackStateLogic(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        input = Vector2.zero;
        targetDirection = Vector3.zero;
        moveVelocity = character.playerVelocity;

        attack = false;
        dodge = false;
        character.animator.applyRootMotion = true;

        timePassed = 0f;

        attackCancelTreshold = character.attackCancelTreshold;
        attackComboTreshold = character.attackComboTreshold;

        currentWeapon = WeaponEquipment.Instance.GetCurrentWeapon();
        SFXManager.instance.PlayWeaponSound(currentWeapon.slashSound);

        character.animator.SetFloat("speedY", 0f);
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (dodgeAction.triggered)
        {
            dodge = true;
        }

        if (liteAttackWeaponAction.triggered)
        {
            attack = true;
        }

        input = moveAction.ReadValue<Vector2>();
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
            moveVelocity = PlayerCamera.instance.transform.forward * verticalInput;
            moveVelocity += PlayerCamera.instance.transform.right * horizontalInput;
            moveVelocity.Normalize();
            moveVelocity.y = 0;

            HandleRotation();

            if (dodge)
            {
                dodge = false;
                stateMachine.ChangeState(character.dodgeState);
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
            stateMachine.ChangeState(character.combatState);
        }
    }

    private void HandleRotation()
    {
        if (character.combatState.isLockedOn)
        {
            if (character.combatState.currentTarget == null)
                return;

            Vector3 lockedTargetDirection;
            lockedTargetDirection = character.combatState.currentTarget.transform.position - character.transform.position;
            lockedTargetDirection.y = 0f;
            lockedTargetDirection.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(lockedTargetDirection);
            character.transform.rotation = targetRotation;
        }
        else
        {
            targetDirection = Vector3.zero;
            targetDirection = PlayerCamera.instance.cameraObj.transform.forward * verticalInput;
            targetDirection += PlayerCamera.instance.cameraObj.transform.right * horizontalInput;
            targetDirection.Normalize();
            targetDirection.y = 0f;

            if (targetDirection == Vector3.zero)
            {
                targetDirection = character.transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetDirection);
            character.transform.rotation = newRotation;
        }
    }

    public override void Exit()
    {
        base.Exit();
        character.animator.applyRootMotion = false;

        if (character.combatState.isLockedOn)
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
