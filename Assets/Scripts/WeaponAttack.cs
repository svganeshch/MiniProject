using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAttack : MonoBehaviour
{
    HealthManager healthManager;
    GameObject characterCausingDamage;
    GameObject characterTakingDamage;

    bool canDealDamage;
    List<GameObject> hasDealtDamage;

    public float weaponLength;
    public float weaponDamage;
    public LayerMask damageLayerMask;

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
            RaycastHit hit;

            if (Physics.Raycast(transform.position, -transform.up, out hit, weaponLength, damageLayerMask))
            {
                if (hit.transform != null)
                {
                    characterTakingDamage = HelperFunctions.GetComponentFromTopParent<HealthManager>(hit.transform, out healthManager).gameObject;

                    if (characterTakingDamage != null)
                    {
                        if (characterTakingDamage == characterCausingDamage)
                            return;

                        if (hit.transform.gameObject == gameObject) return;

                        if (!hasDealtDamage.Contains(hit.transform.gameObject))
                        {
                            healthManager.TakeDamage(weaponDamage);
                            hasDealtDamage.Add(hit.transform.gameObject);
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
