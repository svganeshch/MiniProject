using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : HealthManager
{
    Enemy enemy;

    protected override void Awake()
    {
        base.Awake();

        enemy = GetComponent<Enemy>();
    }

    public override void Die()
    {
        base.Die();

        if (enemy.isBoss)
        {
            Weapon weapon = enemy.currentTarget.weaponEquipment.weapons.Find(weapon => weapon.animatorOverrideController == enemy.weaponEquipment.GetCurrentWeapon().animatorOverrideController);
            enemy.currentTarget.weaponEquipment.EnableWeapon(weapon);
        }

        Destroy(character.gameObject, 10f);
    }
}
