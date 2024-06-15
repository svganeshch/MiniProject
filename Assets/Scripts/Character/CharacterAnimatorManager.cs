using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
    Character character;

    private int previousActionHash;

    // Actions
    private static readonly int dodgeHash = Animator.StringToHash("dodge");
    private static readonly int jumpHash = Animator.StringToHash("jump");
    private static readonly int combatDodgeHash = Animator.StringToHash("combat_dodge");
    private static readonly int blockHash = Animator.StringToHash("block");
    private static readonly int blockBrokenHash = Animator.StringToHash("block_broken");
    private static readonly int hitHash = Animator.StringToHash("hit");

    // Weapon Actions
    private static readonly int weaponDrawHash = Animator.StringToHash("weaponDraw");
    private static readonly int weaponHolsterHash = Animator.StringToHash("weaponHolster");

    // Lite attacks
    private static readonly int liteAttack1Hash = Animator.StringToHash("lite_attack1");
    private static readonly int liteAttack2Hash = Animator.StringToHash("lite_attack2");
    private static readonly int liteAttack3Hash = Animator.StringToHash("lite_attack3");

    // Pivot
    private static readonly int turnL90Hash = Animator.StringToHash("Turn_L90");
    private static readonly int turnL180Hash = Animator.StringToHash("Turn_L180");
    private static readonly int turnR90Hash = Animator.StringToHash("Turn_R90");
    private static readonly int turnR180Hash = Animator.StringToHash("Turn_R180");

    // Animation bools
    private static readonly int isCombatHash = Animator.StringToHash("isCombat");
    private static readonly int isGroundedHash = Animator.StringToHash("isGrounded");

    // Animation floats
    private static readonly int inAirTimeHash = Animator.StringToHash("inAirTime");

    public bool CombatBool
    {
        get => character.animator.GetBool(isCombatHash);
        set => character.animator.SetBool(isCombatHash, value);
    }

    public bool IsGrounded
    {
        get => character.animator.GetBool(isGroundedHash);
        set => character.animator.SetBool(isGroundedHash, value);
    }

    public float InAirTime
    {
        set => character.animator.SetFloat(inAirTimeHash, value);
    }

    protected virtual void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        previousActionHash = 0;
    }

    protected virtual void PlayCharacterActionAnimation(
        int animationClipHash,
        bool canRotate = false,
        bool canMove = false,
        bool applyRootMotion = true)
    {
        previousActionHash = animationClipHash;

        character.animator.CrossFade(animationClipHash, character.animationFadeTime);

        character.applyRootMotion = applyRootMotion;
        character.canRotate = canRotate;
        character.canMove = canMove;
    }

    public void SetAnimatorParameters(float horizontalInput, float verticalInput, bool ignoreSnapping = false)
    {
        if (!ignoreSnapping)
        {
            horizontalInput = SnapInput(horizontalInput);
            verticalInput = SnapInput(verticalInput);

            if (character.characterStateMachine.currentState == character.sprintState)
            {
                verticalInput = 1.5f;
            }
        }

        character.animator.SetFloat("speedX", horizontalInput, character.speedDampTime, Time.deltaTime);
        character.animator.SetFloat("speedY", verticalInput, character.speedDampTime, Time.deltaTime);
    }

    private float SnapInput(float input)
    {
        if (input > 0 && input <= 0.5f)
        {
            return 0.5f;
        }
        if (input > 0.5f && input <= 1)
        {
            return 1;
        }
        if (input < 0 && input >= -0.5f)
        {
            return -0.5f;
        }
        if (input < -0.5f && input >= -1)
        {
            return -1;
        }
        return 0;
    }

    public void PlayDodgeAction()
    {
        PlayCharacterActionAnimation(dodgeHash);
    }

    public void PlayCombatDodgeAction()
    {
        PlayCharacterActionAnimation(combatDodgeHash);
    }

    public void PlayBlockAction()
    {
        PlayCharacterActionAnimation(blockHash, true, true, false);
    }

    public void PlayBlockBrokenAction()
    {
        PlayCharacterActionAnimation(blockBrokenHash);
    }

    public void PlayJumpAction()
    {
        PlayCharacterActionAnimation(jumpHash);
    }

    public void PlayLiteAttackAction(bool canCombo, bool canRotate = false)
    {
        int nextAttackHash = liteAttack1Hash;

        if (canCombo)
        {
            if (previousActionHash == liteAttack1Hash)
                nextAttackHash = liteAttack2Hash;
            else if (previousActionHash == liteAttack2Hash)
                nextAttackHash = liteAttack3Hash;
        }

        character.isAttacking = true;
        PlayCharacterActionAnimation(nextAttackHash, canRotate);
        character.characterSfxManager.PlayWeaponSlashSound();
    }

    public void PlayPivotAction(int pivotAngle)
    {
        int pivotHash = 0;

        switch (pivotAngle)
        {
            case 90: pivotHash = turnR90Hash; break;
            case 180: pivotHash = turnR180Hash; break;
            case -90: pivotHash = turnL90Hash; break;
            case -180: pivotHash = turnL180Hash; break;
        }

        PlayCharacterActionAnimation(pivotHash, false, pivotAngle == 180);
        character.isPivoting = true;
    }

    public void PlayWeaponDrawAction()
    {
        PlayCharacterActionAnimation(weaponDrawHash, true, true, false);
    }

    public void PlayWeaponHolsterAction()
    {
        PlayCharacterActionAnimation(weaponHolsterHash, true, true, false);
    }

    public void PlayHitAction()
    {
        PlayCharacterActionAnimation(hitHash);
    }

    // Common animation event calls
    public void EnablePerformAction()
    {
        character.canPerformAction = true;
    }

    public void DisablePerformAction()
    {
        character.canPerformAction = false;
    }

    public void EnableCombo()
    {
        character.canCombo = true;
    }

    public void DisableCombo()
    {
        character.canCombo = false;
    }
}