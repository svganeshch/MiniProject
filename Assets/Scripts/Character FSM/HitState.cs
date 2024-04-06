using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : State
{
    public bool hitDone = false;

    public HitState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        hitDone = false;

        //character.animator.Play("hit_f");
        character.animator.SetTrigger("damage");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (hitDone)
        {
            stateMachine.ChangeState(stateMachine.previousState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        input = moveAction.ReadValue<Vector2>();
        verticalInput = input.y;
        horizontalInput = input.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5f && moveAmount <= 1)
        {
            moveAmount = 1;
        }

        moveVelocity = PlayerCamera.instance.transform.forward * verticalInput;
        moveVelocity += PlayerCamera.instance.transform.right * horizontalInput;
        moveVelocity.Normalize();
        moveVelocity.y = 0;

        if (moveAmount > 0.5f)
        {
            // running speed
            character.controller.Move(character.runningSpeed * Time.fixedDeltaTime * moveVelocity + gravityVelocity * Time.fixedDeltaTime);
        }
        else if (moveAmount <= 0.5f)
        {
            // walking speed
            character.controller.Move(character.walkingSpeed * Time.fixedDeltaTime * moveVelocity + gravityVelocity * Time.fixedDeltaTime);
        }

        HandleRotation();
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
            Quaternion finalRotation = Quaternion.Slerp(character.transform.rotation, targetRotation, character.rotationDampTime * Time.fixedDeltaTime);
            character.transform.rotation = finalRotation;
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
            Quaternion targetRotation = Quaternion.Slerp(character.transform.rotation, newRotation, character.rotationDampTime * Time.fixedDeltaTime);
            character.transform.rotation = targetRotation;
        }
    }
}
