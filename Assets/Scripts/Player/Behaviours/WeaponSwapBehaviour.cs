using UnityEngine;

public class WeaponSwapBehaviour : StateMachineBehaviour
{
    Character character;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (character == null)
        {
            character = animator.GetComponent<Character>();
        }

        if (character.characterAnimatorManager.SwapWeapon)
        {
            if (character.combatState.isSwappingWeapon)
            {
                character.combatState.isSwappingWeapon = false;
                character.characterAnimatorManager.SwapWeapon = false;

                // Reset timescale after swap
                Time.timeScale = 1f;
                Time.fixedDeltaTime = Time.deltaTime;

                // Reset to default draw/holster speeds
                character.characterAnimatorManager.HolsterSpeed = 1;
                character.characterAnimatorManager.DrawSpeed = 1;

                return;
            }

            character.combatState.SwapWeapon();
        }
    }
}
