using UnityEngine;

public class PlayerAnimatorManager : CharacterAnimatorManager
{
    Player player;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<Player>();
    }

    private void OnAnimatorMove()
    {
        if (player.applyRootMotion)
        {
            Vector3 velocity = player.animator.deltaPosition;

            player.controller.Move(velocity);
            player.transform.rotation *= player.animator.deltaRotation;
        }
    }

    // Animation event calls
    public void ApplyJumpVelocity()
    {
        player.playerMovementManager.yVelocity.y = Mathf.Sqrt(player.jumpHeight * -2 * player.playerMovementManager.gravityForce);
    }
}
