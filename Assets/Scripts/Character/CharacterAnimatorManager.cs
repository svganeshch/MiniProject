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
    private static readonly int holsterSpeedHash = Animator.StringToHash("holsterSpeed");
    private static readonly int drawSpeedHash = Animator.StringToHash("drawSpeed");
    private static readonly int weaponDrawHash = Animator.StringToHash("weaponDraw");
    private static readonly int weaponHolsterHash = Animator.StringToHash("weaponHolster");
    private static readonly int weaponGSDrawHash = Animator.StringToHash("gs_draw1");
    private static readonly int weaponGSHolsterHash = Animator.StringToHash("gs_holster1");
    private static readonly int weaponDaggerDrawHash = Animator.StringToHash("WTD_draw");
    private static readonly int weaponDaggerHolsterHash = Animator.StringToHash("WTD_holster");

    // Lite attacks
    private static readonly int attackSpeedMultiplierHash = Animator.StringToHash("attackSpeed");
    private static readonly int liteAttack1Hash = Animator.StringToHash("lite_attack1");
    private static readonly int liteAttack2Hash = Animator.StringToHash("lite_attack2");
    private static readonly int liteAttack3Hash = Animator.StringToHash("lite_attack3");

    // Pivot
    private static readonly int turnL90Hash = Animator.StringToHash("Turn_L90");
    private static readonly int turnL180Hash = Animator.StringToHash("Turn_L180");
    private static readonly int turnR90Hash = Animator.StringToHash("Turn_R90");
    private static readonly int turnR180Hash = Animator.StringToHash("Turn_R180");

    // Death or Execution animations
    private static readonly int deathCommonHash = Animator.StringToHash("death");

    // Animation bools
    private static readonly int isCombatHash = Animator.StringToHash("isCombat");
    private static readonly int isGroundedHash = Animator.StringToHash("isGrounded");
    private static readonly int swapWeaponHash = Animator.StringToHash("swapWeapon");

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

    public bool SwapWeapon
    {
        get => character.animator.GetBool(swapWeaponHash);
        set => character.animator.SetBool(swapWeaponHash, value);
    }

    public float HolsterSpeed
    {
        set => character.animator.SetFloat(holsterSpeedHash, value);
    }

    public float DrawSpeed
    {
        set => character.animator.SetFloat(drawSpeedHash, value);
    }

    public float AttackSpeedMultiplier
    {
        set => character.animator.SetFloat(attackSpeedMultiplierHash, value);
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
            horizontalInput = HelperFunctions.instance.SnapInput(horizontalInput);
            verticalInput = HelperFunctions.instance.SnapInput(verticalInput);

            if (character.characterStateMachine.currentState == character.sprintState)
            {
                verticalInput = 1.5f;
            }
        }

        character.animator.SetFloat("speedX", horizontalInput, character.speedDampTime, Time.deltaTime);
        character.animator.SetFloat("speedY", verticalInput, character.speedDampTime, Time.deltaTime);
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

    public virtual void PlayWeaponDrawAction()
    {
        int weaponSlot = character.weaponEquipment.GetCurrentWeapon().weaponSlot;
        int weaponToDrawAnimHash;

        switch (weaponSlot)
        {
            case 1:
                weaponToDrawAnimHash = weaponDrawHash;
                break;
            case 2:
                weaponToDrawAnimHash = weaponGSDrawHash;
                break;
            case 3:
                weaponToDrawAnimHash = weaponDaggerDrawHash;
                break;
            default:
                weaponToDrawAnimHash = weaponDrawHash;
                break;
        }

        PlayCharacterActionAnimation(weaponToDrawAnimHash, true, true, false);
    }

    public virtual void PlayWeaponHolsterAction()
    {
        int weaponSlot = character.weaponEquipment.GetCurrentWeapon().weaponSlot;
        int weaponToHolsterAnimHash;

        switch (weaponSlot)
        {
            case 1:
                weaponToHolsterAnimHash = weaponHolsterHash;
                break;
            case 2:
                weaponToHolsterAnimHash = weaponGSHolsterHash;
                break;
            case 3:
                weaponToHolsterAnimHash = weaponDaggerHolsterHash;
                break;
            default:
                weaponToHolsterAnimHash = weaponHolsterHash;
                break;
        }

        PlayCharacterActionAnimation(weaponToHolsterAnimHash, true, true, false);
    }

    public void PlayHitAction()
    {
        PlayCharacterActionAnimation(hitHash, false, false, true);
    }

    public void PlayDeathCommonAnimation()
    {
        PlayCharacterActionAnimation(deathCommonHash, false, false, true);
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