using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : HealthManager
{
    Enemy enemy;
    Player player;

    static int bossDeathCount = 0;

    protected override void Awake()
    {
        base.Awake();

        enemy = GetComponent<Enemy>();
        player = FindObjectOfType<Player>();
    }

    public override void Die()
    {
        base.Die();

        if (enemy.isBoss)
        {
            player.hudManager.SetInfoMessage(enemy.bossDefeatMessage);
            bossDeathCount++;

            Weapon weapon = player.weaponEquipment.weapons.Find(weapon => weapon.animatorOverrideController == enemy.weaponEquipment.weapons[0].animatorOverrideController);
            player.weaponEquipment.EnableWeapon(weapon);
            Debug.Log("weapon is " + weapon.weaponObj.name);
            Debug.Log("weapon slot : " + weapon.weaponSlot);

            if (weapon.isDagger)
            {
                player.weaponEquipment.EnableWeapon(player.weaponEquipment.GetWeaponWithSlot(weapon.weaponSlot + 1));
                Debug.Log("dagger2 enabled");
            }

            if (bossDeathCount == 2)
            {
                Debug.Log("bosses cleared");
                MenuManager.Instance.SetVictoryMenu();
            }
        }

        Destroy(character.gameObject, 10f);
    }
}
