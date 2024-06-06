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
    public bool reverseHolster = false;
    public bool reverseDraw = false;
    public bool isDagger = false;

    [HideInInspector] public GameObject weaponObj;
    [HideInInspector] public WeaponAttack weaponAttackScript;
}

public class WeaponEquipment : MonoBehaviour
{
    public List<Weapon> weapons = new List<Weapon>();
    public float weaponSwapSpeed = 4f;

    private GameObject currentWeaponObj;
    private Weapon currentWeapon;
    private int currentWeaponSlot = 1;

    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        foreach (var weapon in weapons)
        {
            GameObject weaponObj = Instantiate(weapon.weaponPrefab, weapon.weaponHolsterPosition);
            weapon.weaponObj = weaponObj;
            weapon.weaponAttackScript = weaponObj.GetComponentInChildren<WeaponAttack>();

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
        currentWeaponSlot = weapons.Find(weapon => weapon.weaponSlot == weaponSlot).weaponSlot;
        currentWeapon = GetWeaponWithSlot(currentWeaponSlot);

        if (!currentWeapon.Enabled)
        {
            return false;
        }

        currentWeaponObj = currentWeapon.weaponObj;
        character.animator.runtimeAnimatorController = currentWeapon.animatorOverrideController;

        AnimatorOverrideController animatorOverrideController = currentWeapon.animatorOverrideController;
        List<KeyValuePair<AnimationClip, AnimationClip>> overrideClips;

        overrideClips = new List<KeyValuePair<AnimationClip, AnimationClip>>(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(overrideClips);

        foreach (KeyValuePair<AnimationClip, AnimationClip> overrideCl in overrideClips)
        {
            if (overrideCl.Key.name.CompareTo("draw_2") == 0 && overrideCl.Value != null)
            {
                character.animator.SetBool("hasDraw2", true);
            }
            else if (overrideCl.Key.name.CompareTo("holster_2") == 0 && overrideCl.Value != null)
            {
                character.animator.SetBool("hasHolster2", true);
            }

            //Debug.Log("clip : " +  overrideCl);
        }

        character.animator.SetBool("reverseDraw", currentWeapon.reverseDraw);
        character.animator.SetBool("reverseHolster", currentWeapon.reverseHolster);
        character.animator.SetFloat("attackSpeed", currentWeapon.attackSpeedMultiplier);

        Debug.Log("Set weapon to " + weaponSlot);

        return true;
    }

    public void DrawWeapon()
    {
        character.characterAnimatorManager.CombatBool = true;
        character.animator.SetLayerWeight(1, 1);
        
        currentWeaponObj.transform.parent = currentWeapon.weaponHolderPosition.transform;
        currentWeaponObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        if (currentWeapon.isDagger)
        {
            Weapon dagger2 = weapons.FindLast(dagger => dagger.isDagger);

            dagger2.weaponObj.transform.parent = dagger2.weaponHolderPosition.transform;
            dagger2.weaponObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }

    public void HolsterWeapon()
    {
        character.characterAnimatorManager.CombatBool = false;
        character.animator.SetLayerWeight(1, 0);
        
        currentWeaponObj.transform.parent = currentWeapon.weaponHolsterPosition.transform;
        currentWeaponObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        if (currentWeapon.isDagger)
        {
            Weapon dagger2 = weapons.FindLast(dagger => dagger.isDagger);

            dagger2.weaponObj.transform.parent = dagger2.weaponHolsterPosition.transform;
            dagger2.weaponObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        character.animator.SetBool("hasDraw2", false);
        character.animator.SetBool("hasHolster2", false);
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
        return weapons.Find(weapon => weapon.weaponSlot == currentWeaponSlot);
    }

    public Weapon GetWeaponWithSlot(int weaponSlot)
    {
        return weapons.Find(weapon => weapon.weaponSlot == weaponSlot);
    }
}
