using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEquipment : MonoBehaviour
{
    [SerializeField] GameObject weapon;
    [SerializeField] GameObject weaponHolderPosition;
    [SerializeField] GameObject weaponHolsterPosition;

    private GameObject currentWeapon;

    private void Start()
    {
        currentWeapon = Instantiate(weapon, weaponHolsterPosition.transform);
    }

    public void DrawWeapon()
    {
        currentWeapon.transform.parent = weaponHolderPosition.transform;
        currentWeapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void HolsterWeapon()
    {
        currentWeapon.transform.parent = weaponHolsterPosition.transform;
        currentWeapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void StartDamage()
    {
        currentWeapon.GetComponentInChildren<Weapon>().StartDealDamage();
    }

    public void StopDamage()
    {
        currentWeapon.GetComponentInChildren<Weapon>().EndDealDamage();
    }
}
