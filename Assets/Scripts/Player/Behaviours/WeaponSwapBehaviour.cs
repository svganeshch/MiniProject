using UnityEngine;

public class WeaponSwapBehaviour : StateMachineBehaviour
{
    private bool isWeaponSwap = false;

    Player player;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (player == null)
        {
            player = animator.GetComponent<Player>();
        }

        isWeaponSwap = animator.GetBool("swapWeapon");

        if (isWeaponSwap)
        {
            player.combatState.SwapWeapon();
        }
    }
}
