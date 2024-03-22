using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDrawBehaviour : StateMachineBehaviour
{
    public static bool skippedLayer = false;

    private Character character;
    private bool isWeaponDraw = false;
    private float speedY;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isCombat", true);

        speedY = animator.GetFloat("speedY");

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

        skippedLayer = false;
        animator.SetBool("swapWeapon", false);
        animator.SetFloat("holsterSpeed", 1);
        animator.SetFloat("drawSpeed", 1);

        character = animator.GetComponent<Character>();
        isWeaponDraw = animator.GetBool("isWeaponDraw");

        if (isWeaponDraw)
        {
            animator.SetBool("isWeaponDraw", false);
            character.characterMovementSM.ChangeState(character.combatState);
        }
    }
}
