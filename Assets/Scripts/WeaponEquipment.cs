using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

[System.Serializable]
public class Weapon
{
    public AnimatorOverrideController animatorOverrideController;
    public GameObject weaponPrefab;
    public Transform weaponHolderPosition;
    public Transform weaponHolsterPosition;

    public int weaponSlot;
    public bool reverseHolster = false;
    public bool reverseDraw = false;
    public bool isDagger = false;

    [HideInInspector] public float drawSpeed = 1;
    [HideInInspector] public float holsterSpeed = 1;

    [HideInInspector] public GameObject weaponObj;
}

public class WeaponEquipment : MonoBehaviour
{
    public List<Weapon> weapons = new List<Weapon>();
    
    private GameObject currentWeaponObj;
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

            //weaponObj.SetActive(false);
        }
    }

    public void SetWeapon(int weaponSlot)
    {
        currentWeaponSlot = weapons.Find(weapon => weapon.weaponSlot == weaponSlot).weaponSlot;
        Weapon currentWeapon = GetCurrentWeapon();

        currentWeaponObj = currentWeapon.weaponObj;
        animator.runtimeAnimatorController = currentWeapon.animatorOverrideController;

        AnimatorOverrideController animatorOverrideController = currentWeapon.animatorOverrideController;
        List<KeyValuePair<AnimationClip, AnimationClip>> overrideClips;

        overrideClips = new List<KeyValuePair<AnimationClip, AnimationClip>>(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(overrideClips);

        foreach (KeyValuePair<AnimationClip, AnimationClip> overrideCl in overrideClips)
        {
            if (overrideCl.Key.name.CompareTo("draw_2") == 0 && overrideCl.Value != null)
            {
                animator.SetBool("hasDraw2", true);
            }
            else if (overrideCl.Key.name.CompareTo("holster_2") == 0 && overrideCl.Value != null)
            {
                animator.SetBool("hasHolster2", true);
            }
            else if (overrideCl.Key.name.CompareTo("lite_attack1_rec") == 0 && overrideCl.Value != null)
            {
                animator.SetBool("hasRecovery", true);
            }

            Debug.Log("clip : " +  overrideCl);
        }

        animator.SetBool("reverseDraw", currentWeapon.reverseDraw);
        animator.SetBool("reverseHolster", currentWeapon.reverseHolster);

        Debug.Log("Set weapon to " + weaponSlot);
    }

    public void DrawWeapon()
    {
        currentWeaponObj.transform.parent = GetCurrentWeapon().weaponHolderPosition.transform;
        currentWeaponObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        if (GetCurrentWeapon().isDagger)
        {
            Weapon dagger2 = weapons.FindLast(dagger => dagger.isDagger);

            dagger2.weaponObj.transform.parent = dagger2.weaponHolderPosition.transform;
            dagger2.weaponObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }

    public void HolsterWeapon()
    {
        currentWeaponObj.transform.parent = GetCurrentWeapon().weaponHolsterPosition.transform;
        currentWeaponObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        if (GetCurrentWeapon().isDagger)
        {
            Weapon dagger2 = weapons.FindLast(dagger => dagger.isDagger);

            dagger2.weaponObj.transform.parent = dagger2.weaponHolsterPosition.transform;
            dagger2.weaponObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        weaponHolsterDone = true;
        animator.SetBool("hasDraw2", false);
        animator.SetBool("hasHolster2", false);
        animator.SetBool("hasRecovery", false);
    }

    public void StartDamage()
    {
        currentWeaponObj.GetComponentInChildren<WeaponAttack>().StartDealDamage();
    }

    public void StopDamage()
    {
        currentWeaponObj.GetComponentInChildren<WeaponAttack>().EndDealDamage();
    }

    public Weapon GetCurrentWeapon()
    {
        return weapons.Find(weapon => weapon.weaponSlot == currentWeaponSlot);
    }
}
