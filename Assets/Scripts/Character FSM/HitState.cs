using UnityEngine;

public class HitState : State
{
    public bool hitDone = false;

    private bool dodge = false;

    public HitState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        hitDone = false;
        dodge = false;

        //character.animator.Play("hit_f");
        character.animator.SetTrigger("damage");
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (dodgeAction.triggered)
        {
            dodge = true;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        CharacterMovement();

        if (dodge)
        {
            dodge = false;
            stateMachine.ChangeState(character.dodgeState);
        }

        if (hitDone)
        {
            stateMachine.ChangeState(character.combatState);
        }
    }

    private void CharacterMovement()
    {
        input = moveAction.ReadValue<Vector2>();
        verticalInput = input.y;
        horizontalInput = input.x;

        HandleRotation();
    }

    private void HandleRotation()
    {
        if (character.combatState.isLockedOn)
        {
            if (character.currentLockedOnTarget == null)
                return;

            Vector3 lockedTargetDirection;
            lockedTargetDirection = character.currentLockedOnTarget.transform.position - character.transform.position;
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
