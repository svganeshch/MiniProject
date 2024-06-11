using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
    Character character;

    private int previousActionHash;

    // Actions
    private static int dodgeHash;
    private static int jumpHash;
    private static int combat_dodge_Hash;
    private static int blockHash;
    private static int blockBrokenHash;
    private static int hitHash;

    // Weapon Actions
    private static int weaponDrawHash;
    private static int weaponHolsterHash;

    // Lite attacks
    private static int lite_attack1_hash;
    private static int lite_attack2_hash;
    private static int lite_attack3_hash;

    // Animation bools
    private static int isCombatHash;
    private static int isGroundedHash;

    // Animation floats
    private static int inAirTimeHash;

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

    protected virtual void Awake() { }

    private void Start()
    {
        character = GetComponent<Character>();

        dodgeHash = Animator.StringToHash("dodge");
        jumpHash = Animator.StringToHash("jump");
        combat_dodge_Hash = Animator.StringToHash(("combat_dodge"));
        blockHash = Animator.StringToHash("block");
        blockBrokenHash = Animator.StringToHash("block_broken");
        hitHash = Animator.StringToHash("hit");

        weaponDrawHash = Animator.StringToHash(("weaponDraw"));
        weaponHolsterHash = Animator.StringToHash(("weaponHolster"));

        lite_attack1_hash = Animator.StringToHash("lite_attack1");
        lite_attack2_hash = Animator.StringToHash("lite_attack2");
        lite_attack3_hash = Animator.StringToHash("lite_attack3");

        isCombatHash = Animator.StringToHash("isCombat");
        isGroundedHash = Animator.StringToHash("isGrounded");

        inAirTimeHash = Animator.StringToHash("inAirTime");
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

    public void SetAnimatorParameters(float horizontalInput, float verticalInput)
    {
        //float snappedHorizontal = 0f;
        //float snappedVertical = 0f;

        //if (horizontalInput > 0 && horizontalInput <= 0.5f)
        //{
        //    snappedHorizontal = 0.5f;
        //}
        //else if (horizontalInput > 0.5f && horizontalInput <= 1)
        //{
        //    snappedHorizontal = 1;
        //}
        //else if (horizontalInput < 0 && horizontalInput >= -0.5f)
        //{
        //    snappedHorizontal = -0.5f;
        //}
        //else if (horizontalInput < -0.5f && horizontalInput >= -1)
        //{
        //    snappedHorizontal = -1;
        //}
        //else
        //{
        //    snappedHorizontal = 0;
        //}

        //if (verticalInput > 0 && verticalInput <= 0.5f)
        //{
        //    snappedVertical = 0.5f;
        //}
        //else if (verticalInput > 0.5f && verticalInput <= 1)
        //{
        //    snappedVertical = 1;
        //}
        //else if (verticalInput < 0 && verticalInput >= -0.5f)
        //{
        //    snappedVertical = -0.5f;
        //}
        //else if (verticalInput < -0.5f && verticalInput >= -1)
        //{
        //    snappedVertical = -1;
        //}
        //else
        //{
        //    snappedVertical = 0;
        //}

        character.animator.SetFloat("speedX", horizontalInput, character.speedDampTime, Time.deltaTime);
        character.animator.SetFloat("speedY", verticalInput, character.speedDampTime, Time.deltaTime);
    }

    public void PlayDodgeAction()
    {
        PlayCharacterActionAnimation(dodgeHash);
    }

    public void PlayCombatDodgeAction()
    {
        PlayCharacterActionAnimation(combat_dodge_Hash);
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
        if (canCombo)
        {
            if (previousActionHash == lite_attack1_hash)
            {
                PlayCharacterActionAnimation(lite_attack2_hash);
            }
            else if (previousActionHash == lite_attack2_hash)
            {
                PlayCharacterActionAnimation(lite_attack3_hash);
            }
            else
            {
                PlayCharacterActionAnimation(lite_attack1_hash);
            }
        }
        else
        {
            PlayCharacterActionAnimation(lite_attack1_hash, canRotate);
        }

        Debug.Log("is combo : " + canCombo);
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
}
