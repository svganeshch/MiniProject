using UnityEngine;

public class WeaponSwapBehaviour : StateMachineBehaviour
{
    public static bool skippedLayer = false;

    private bool isWeaponSwap = false;
    private float speedY;

    Character character;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (character == null)
        {
            character = animator.GetComponent<Character>();
        }

        isWeaponSwap = animator.GetBool("swapWeapon");
        speedY = animator.GetFloat("speedY");

        if (isWeaponSwap)
        {
            if (!skippedLayer)
            {
                if (speedY > 0.01f && layerIndex == 1)
                {
                    skippedLayer = true;
                    return;
                }

                if (speedY < 0.01f && layerIndex == 2)
                {
                    skippedLayer = true;
                    return;
                }
            }
        }

        skippedLayer = false;

        if (isWeaponSwap)
        {
            //character.combatState.SwapWeapon();
        }
    }
}
