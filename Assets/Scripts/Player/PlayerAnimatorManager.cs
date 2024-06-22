using UnityEngine;

public class PlayerAnimatorManager : CharacterAnimatorManager
{
    Player player;
    Transform playerTransform;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<Player>();
        playerTransform = player.transform;
    }

    private void OnAnimatorMove()
    {
        if (player.applyRootMotion)
        {
            Vector3 velocity = player.animator.deltaPosition;

            player.controller.Move(velocity);
            playerTransform.rotation *= player.animator.deltaRotation;
        }
    }

    public override void PlayWeaponDrawAction()
    {
        base.PlayWeaponDrawAction();

        player.hudManager.SetWeaponWheel(player.weaponEquipment.GetCurrentWeapon().weaponSlot);
    }

    public override void PlayWeaponHolsterAction()
    {
        base.PlayWeaponHolsterAction();

        if (!SwapWeapon)
            player.hudManager.SetWeaponWheel(0);
    }

    // Animation event calls
    public void ApplyJumpVelocity()
    {
        player.playerMovementManager.yVelocity.y = Mathf.Sqrt(player.jumpHeight * -2 * player.playerMovementManager.gravityForce);
    }
}
