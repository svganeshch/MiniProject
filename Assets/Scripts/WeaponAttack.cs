using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAttack : MonoBehaviour
{
    HealthManager healthManager;
    GameObject characterCausingDamage;
    GameObject characterTakingDamage;

    public float weaponLength;
    [HideInInspector] public float weaponDamage;
    LayerMask damageLayerMask;

    RaycastHit hit;
    bool canDealDamage;
    List<GameObject> hasDealtDamage;

    void Start()
    {
        canDealDamage = false;
        hasDealtDamage = new List<GameObject>();
        damageLayerMask = Character.instance.damagableLayerMask;

        characterCausingDamage = HelperFunctions.GetComponentFromTopParent<HealthManager>(transform).gameObject;
    }

    void Update()
    {
        if (canDealDamage)
        {
            if (Physics.Raycast(transform.position, -transform.up, out hit, weaponLength, damageLayerMask))
            {
                if (hit.transform != null)
                {
                    characterTakingDamage = HelperFunctions.GetComponentFromTopParent<HealthManager>(hit.transform, out healthManager).gameObject;

                    if (characterTakingDamage != null)
                    {
                        if (characterTakingDamage == characterCausingDamage)
                            return;

                        if (!hasDealtDamage.Contains(characterTakingDamage))
                        {
                            healthManager.TakeDamage(weaponDamage);
                            hasDealtDamage.Add(characterTakingDamage);
                        }
                    }
                }
            }
        }
    }
    public void StartDealDamage()
    {
        canDealDamage = true;

        hasDealtDamage.Clear();
        characterTakingDamage = null;
        healthManager = null;
    }
    public void EndDealDamage()
    {
        canDealDamage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLength);
    }
}
