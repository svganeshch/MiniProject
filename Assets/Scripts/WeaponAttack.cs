using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAttack : MonoBehaviour
{
    private Transform weaponRayTransform;
    private Character characterCausingDamage;
    private Character characterTakingDamage;
    private HealthManager healthManager;
    private Character hitCharacter;

    public float weaponLength;
    [HideInInspector] public float weaponDamage;
    private LayerMask damageLayerMask;

    private RaycastHit hit;
    private bool canDealDamage;
    private HashSet<GameObject> hasDealtDamage;

    private void Awake()
    {
        weaponRayTransform = transform;
        characterCausingDamage = GetComponentInParent<Character>();
    }

    private void Start()
    {
        canDealDamage = false;
        hasDealtDamage = new HashSet<GameObject>();
        damageLayerMask = LayerMaskManager.Instance.damagableLayerMask;
    }

    private void Update()
    {
        if (!canDealDamage)
            return;

        if (Physics.Raycast(weaponRayTransform.position, -weaponRayTransform.up, out hit, weaponLength, damageLayerMask))
        {
            if (hit.transform == null) return;

            hitCharacter = GetHitCharacter();

            if (hitCharacter == null || hitCharacter == characterCausingDamage)
                return;

            if (healthManager == null || hitCharacter != characterTakingDamage)
            {
                characterTakingDamage = hitCharacter;
                healthManager = characterTakingDamage.healthManager;
            }

            if (hasDealtDamage.Add(characterTakingDamage.gameObject))
            {
                healthManager.TakeDamage(weaponDamage, characterCausingDamage, characterTakingDamage);
            }
        }
    }

    private Character GetHitCharacter()
    {
        if (hitCharacter != null && hit.transform.root == hitCharacter.transform)
            return hitCharacter;

        return hit.transform.GetComponentInParent<Character>();
    }

    public void StartDealDamage()
    {
        canDealDamage = true;
        hasDealtDamage.Clear();
    }

    public void StopDealDamage()
    {
        canDealDamage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLength);
    }
}
