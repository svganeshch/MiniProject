using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyAttackState : State
{
    bool attack;
    bool dodge;
    private float timePassed;
    private float clipLength;
    private float clipSpeed;
    private float clipPercentage;

    public HeavyAttackState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
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

        character.animator.SetTrigger("heavyAttack");
        character.animator.SetFloat("speedY", 0f);
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (dodgeAction.triggered)
        {
            dodge = true;
        }

        if (heavyAttackWeaponAction.triggered)
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
        clipPercentage = (clipLength / clipSpeed) * 0.3f;

        if (timePassed <= clipPercentage)
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

        if (timePassed >= clipLength / clipSpeed && attack)
        {
            stateMachine.ChangeState(character.heavyAttackState);
        }
        if (timePassed >= clipLength / clipSpeed)
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
