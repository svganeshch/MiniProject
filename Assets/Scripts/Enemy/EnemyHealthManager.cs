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
            Debug.Log("weapon is " + weapon.weaponObj.name);
            Debug.Log("weapon slot : " + weapon.weaponSlot);

            if (weapon.isDagger)
            {
                enemy.currentTarget.weaponEquipment.EnableWeapon(enemy.currentTarget.weaponEquipment.GetWeaponWithSlot(weapon.weaponSlot + 1));
                Debug.Log("dagger2 enabled");
            }
        }

        Destroy(character.gameObject, 10f);
    }
}
