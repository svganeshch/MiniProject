using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class WeaponSwapBehaviour : StateMachineBehaviour
{
    private Character character;
    private bool isWeaponSwap = false;
    private float speedY;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        speedY = animator.GetFloat("speedY");

        isWeaponSwap = animator.GetBool("swapWeapon");
        character = animator.GetComponent<Character>();

        if (speedY > 0.01f && layerIndex == 1)
            return;
        else if (speedY < 0.01f && layerIndex == 2)
            return;

        if (isWeaponSwap)
        {
            character.combatState.SwapWeapon();
            animator.SetBool("swapWeapon", false);

            character.animator.SetFloat("holsterSpeed", 1);
            character.animator.SetFloat("drawSpeed", 1);
        }
    }
}
