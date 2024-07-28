using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : HealthManager
{
    public override void Die()
    {
        base.Die();

        Destroy(character.gameObject, 10f);
    }
}
