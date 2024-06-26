using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon
{
    public bool Enabled = false;

    [Header("Weapon Components")]
    public AnimatorOverrideController animatorOverrideController;
    public GameObject weaponPrefab;
    public Transform weaponHolderPosition;
    public Transform weaponHolsterPosition;

    [Header("Weapon SFX")]
    public AudioClip slashSound;
    public AudioClip hitSound;

    [Header("Weapon Settings")]
    public int weaponSlot;
    public float weaponDamage = 50f;
    public float attackSpeedMultiplier = 1f;
    public bool isDagger = false;

    [HideInInspector] public GameObject weaponObj;
    [HideInInspector] public Transform weaponObjTransform;
    [HideInInspector] public WeaponAttack weaponAttackScript;
}

public class WeaponEquipment : MonoBehaviour
{
    public List<Weapon> weapons = new List<Weapon>();
    public float weaponSwapSpeed = 4f;

    private GameObject currentWeaponObj;
    private Weapon currentWeapon;
    protected int currentWeaponSlot = 1;

    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    protected virtual void Start()
    {
        foreach (var weapon in weapons)
        {
            GameObject weaponObj = Instantiate(weapon.weaponPrefab, weapon.weaponHolsterPosition);
            weapon.weaponObj = weaponObj;
            weapon.weaponAttackScript = weaponObj.GetComponentInChildren<WeaponAttack>();
            weapon.weaponObjTransform = weaponObj.transform;

            if (weapon.weaponAttackScript != null)
            {
                weapon.weaponAttackScript.weaponDamage = weapon.weaponDamage;
            }

            if (!weapon.Enabled)
                weaponObj.SetActive(false);
        }
    }

    public bool SetWeapon(int weaponSlot)
    {
        Weapon selectedWeapon = GetWeaponWithSlot(weaponSlot);

        if (selectedWeapon == null || !selectedWeapon.Enabled)
        {
            return false;
        }

        currentWeapon = selectedWeapon;
        currentWeaponSlot = weaponSlot;
        currentWeaponObj = currentWeapon.weaponObj;
        character.animator.runtimeAnimatorController = currentWeapon.animatorOverrideController;

        character.characterAnimatorManager.AttackSpeedMultiplier = currentWeapon.attackSpeedMultiplier;

        Debug.Log("Set weapon to " + weaponSlot);

        return true;
    }

    public virtual void DrawWeapon()
    {
        SetWeaponTransform(currentWeapon.weaponHolderPosition);

        if (currentWeapon.isDagger)
        {
            Weapon dagger2 = GetDagger2();
            SetWeaponTransform(dagger2.weaponHolderPosition, dagger2);
        }

        character.characterAnimatorManager.CombatBool = true;
        character.animator.SetLayerWeight(1, 1);
    }

    public virtual void HolsterWeapon()
    {
        SetWeaponTransform(currentWeapon.weaponHolsterPosition);

        if (currentWeapon.isDagger)
        {
            Weapon dagger2 = GetDagger2();
            SetWeaponTransform(dagger2.weaponHolsterPosition, dagger2);
        }

        character.characterAnimatorManager.CombatBool = false;
        character.animator.SetLayerWeight(1, 0);
    }

    private void SetWeaponTransform(Transform parentTransform, Weapon weapon = null)
    {
        if (weapon == null)
        {
            weapon = currentWeapon;
        }

        Transform weaponTransform = weapon.weaponObjTransform;
        weaponTransform.SetParent(parentTransform);
        weaponTransform.localPosition = Vector3.zero;
        weaponTransform.localRotation = Quaternion.identity;
    }

    private Weapon GetDagger2()
    {
        return weapons.FindLast(dagger => dagger.isDagger);
    }

    public void StartDamage()
    {
        currentWeapon.weaponAttackScript.StartDealDamage();
    }

    public void StopDamage()
    {
        currentWeapon.weaponAttackScript.StopDealDamage();
    }

    public Weapon GetCurrentWeapon()
    {
        return GetWeaponWithSlot(currentWeaponSlot);
    }

    public Weapon GetWeaponWithSlot(int weaponSlot)
    {
        return weapons.Find(weapon => weapon.weaponSlot == weaponSlot);
    }
}