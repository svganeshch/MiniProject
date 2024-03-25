using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class WeaponSwapBehaviour : StateMachineBehaviour
{
    public static bool skippedLayer = false;

    private Character character;
    private bool isWeaponSwap = false;
    private float speedY;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

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
        character = animator.GetComponent<Character>();

        if (isWeaponSwap)
        {
            character.combatState.SwapWeapon();
        }
    }
}
