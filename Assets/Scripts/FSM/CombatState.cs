using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CombatState : State
{
    float gravityValue;
    float playerSpeed;

    int swapWeaponTo;

    bool isGrounded;
    bool holsterWeapon;
    bool attackState;
    bool heavyAttackState;
    bool swapWeapon;
    bool swapTrigger;

    Vector3 currentVelocity;
    Vector3 smoothVelocity;

    public CombatState(Character _character, StateMachine _stateMachine) : base(_character, _stateMachine)
    {
        character = _character;
        stateMachine = _stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        holsterWeapon = false;
        attackState = false;
        heavyAttackState = false;
        swapWeapon = false;
        swapTrigger = false;
        input = Vector2.zero;
        currentVelocity = Vector3.zero;
        gravityVelocity.y = 0;

        velocity = character.playerVelocity;
        playerSpeed = character.playerSpeed;
        isGrounded = character.controller.isGrounded;
        gravityValue = character.GRAVITY_VALUE;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        if (drawWeaponAction.triggered)
        {
            holsterWeapon = true;
        }

        if (heavyAttackWeaponAction.triggered)
        {
            heavyAttackState = true;
        }

        if (attackWeaponAction.triggered && !heavyAttackState)
        {
            attackState = true;
        }

        if (weapon1Action.triggered)
        {
            swapWeaponTo = 1;
            swapWeapon = true;
        }
        if (weapon2Action.triggered)
        {
            swapWeaponTo = 2;
            swapWeapon = true;
        }
        if (weapon3Action.triggered)
        {
            swapWeaponTo = 3;
            swapWeapon = true;
        }
        if (weapon4Action.triggered)
        {
            swapWeaponTo = 4;
            swapWeapon = true;
        }

        input = moveAction.ReadValue<Vector2>();
        velocity = new Vector3(input.x, 0, input.y);
        velocity = velocity.x * character.cameraTransform.right.normalized + velocity.z * character.cameraTransform.forward.normalized;
        velocity.y = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        character.animator.SetFloat("speed", input.magnitude, character.speedDampTime, Time.deltaTime);

        if (holsterWeapon)
        {
            character.animator.SetTrigger("holsterWeapon");
            stateMachine.ChangeState(character.idleState);
        }

        if (heavyAttackState)
        {
            character.animator.SetTrigger("heavyAttack");
            stateMachine.ChangeState(character.heavyAttackState);
        }

        if (attackState)
        {
            character.animator.SetTrigger("attack");
            stateMachine.ChangeState(character.attackState);
        }

        if (swapWeapon)
        {
            if (character.weaponEquipment.GetCurrentWeapon().weaponSlot == swapWeaponTo)
            {
                swapWeapon = false;
                character.animator.SetTrigger("holsterWeapon");
                return;
            }

            character.animator.SetFloat("holsterSpeed", 2);
            character.animator.SetFloat("drawSpeed", 2);

            character.animator.SetTrigger("holsterWeapon");

            swapTrigger = true;
            swapWeapon = false;
        }

        if (swapTrigger)
        {
            if (character.weaponEquipment.weaponHolsterDone)
            {
                if (character.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("idle"))
                {
                    swapTrigger = false;
                    character.weaponEquipment.weaponHolsterDone = false;
                    character.StartCoroutine(SwapWeapon());
                }
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        gravityVelocity.y += gravityValue * Time.deltaTime;
        isGrounded = character.controller.isGrounded;

        if (isGrounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        currentVelocity = Vector3.SmoothDamp(currentVelocity, velocity, ref smoothVelocity, character.velocityDampTime);
        character.controller.Move(currentVelocity * Time.deltaTime * playerSpeed + gravityVelocity * Time.deltaTime);

        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.LookRotation(velocity), character.rotationDampTime);
        }

    }

    public override void Exit()
    {
        base.Exit();

        gravityVelocity.y = 0f;
        character.playerVelocity = new Vector3(input.x, 0, input.y);

        if (velocity.sqrMagnitude > 0)
        {
            character.transform.rotation = Quaternion.LookRotation(velocity);
        }

    }

    private IEnumerator SwapWeapon()
    {
        yield return new WaitForSeconds(2f);
        character.animator.SetTrigger("drawWeapon");
        character.weaponEquipment.SetWeapon(swapWeaponTo);
    }
}
