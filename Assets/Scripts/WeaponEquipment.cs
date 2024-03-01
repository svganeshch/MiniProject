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
    private int currentWeaponSlot = 0;

    private Animator animator;
    private PlayerInput playerInput;

    private InputAction weapon1Action;
    private InputAction weapon2Action;
    private InputAction weapon3Action;
    private InputAction weapon4Action;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        weapon1Action = playerInput.actions["Weapon1"];
        weapon2Action = playerInput.actions["Weapon2"];
        weapon3Action = playerInput.actions["Weapon3"];
        weapon4Action = playerInput.actions["Weapon4"];

        foreach (var weapon in weapons)
        {
            GameObject weaponObj = Instantiate(weapon.weaponPrefab, weapon.weaponHolsterPosition);
            weapon.weaponObj = weaponObj;
        }
    }

    private void Update()
    {
        if (weapon1Action.triggered)
        {
            SetWeapon(1);
        }
        else if (weapon2Action.triggered)
        {
            SetWeapon(2);
        }
        else if (weapon3Action.triggered)
        {
            SetWeapon(3);
        }
        else if(weapon4Action.triggered)
        {
            SetWeapon(4);
        }
    }

    private void SetWeapon(int weaponSlot)
    {
        //foreach (var weapon in weapons)
        //{
        //    if (weapon.weaponSlot == weaponSlot)
        //    {
        //        currentWeaponSlot = weaponSlot;
        //    }
        //}

        if (weaponSlot == currentWeaponSlot)
        {
            return;
        }
        else if (weaponSlot != currentWeaponSlot)
        {
            if (currentWeaponSlot != 0)
                EnableWeaponAnimLayer(false);

            currentWeapon = weapons.Find(weapon => weapon.weaponSlot == weaponSlot).weaponObj;
            currentWeaponSlot = weapons.Find(weapon => weapon.weaponSlot == weaponSlot).weaponSlot;
            EnableWeaponAnimLayer(true);
        }
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
