using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class WeaponList
{
    public GameObject weaponPrefab;
    public Transform weaponHolderPosition;
    public Transform weaponHolsterPosition;

    public int weaponSlot;
    public int weaponAnimLayerIndex;
    public int weaponAnimArmsLayerIndex;

    [HideInInspector] public GameObject weaponObj;
}

public class WeaponEquipment : MonoBehaviour
{
    public List<WeaponList> weapons = new List<WeaponList>();
    
    private GameObject currentWeapon;
    private int currentWeaponSlot = 1;

    private Animator animator;

    public bool weaponHolsterDone = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        foreach (var weapon in weapons)
        {
            GameObject weaponObj = Instantiate(weapon.weaponPrefab, weapon.weaponHolsterPosition);
            weapon.weaponObj = weaponObj;
        }
    }

    public void SetWeapon(int weaponSlot)
    {
        currentWeapon = weapons.Find(weapon => weapon.weaponSlot == weaponSlot).weaponObj;
        currentWeaponSlot = weapons.Find(weapon => weapon.weaponSlot == weaponSlot).weaponSlot;
        EnableWeaponAnimLayer(true);

        Debug.Log("Set weapon to " + weaponSlot);
    }

    public void DrawWeapon()
    {
        currentWeapon.transform.parent = GetCurrentWeapon().weaponHolderPosition.transform;
        currentWeapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void HolsterWeapon()
    {
        currentWeapon.transform.parent = GetCurrentWeapon().weaponHolsterPosition.transform;
        currentWeapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        EnableWeaponAnimLayer(false);
        weaponHolsterDone = true;
    }

    public void EnableWeaponAnimLayer(bool state)
    {
        animator.SetLayerWeight(GetCurrentWeapon().weaponAnimLayerIndex, state ? 1 : 0);
        animator.SetLayerWeight(GetCurrentWeapon().weaponAnimArmsLayerIndex, state ? 1 : 0);
    }

    public void StartDamage()
    {
        currentWeapon.GetComponentInChildren<WeaponAttack>().StartDealDamage();
    }

    public void StopDamage()
    {
        currentWeapon.GetComponentInChildren<WeaponAttack>().EndDealDamage();
    }

    public WeaponList GetCurrentWeapon()
    {
        return weapons.Find(weapon => weapon.weaponSlot == currentWeaponSlot);
    }
}
