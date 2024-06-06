using UnityEngine;

public class EnemyAnimatorManager : CharacterAnimatorManager
{
    Enemy enemy;

    protected override void Awake()
    {
        base.Awake();

        enemy = GetComponent<Enemy>();
    }

    private void OnAnimatorMove()
    {
        if (enemy.applyRootMotion)
        {
            Vector3 velocity = enemy.animator.deltaPosition;

            enemy.controller.Move(velocity);
            enemy.transform.rotation *= enemy.animator.deltaRotation;
        }
    }
}
