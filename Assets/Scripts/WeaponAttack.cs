using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAttack : MonoBehaviour
{
    HealthManager healthManager;
    public Character characterCausingDamage;
    public Character characterTakingDamage;

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
        damageLayerMask = LayerMaskManager.Instance.damagableLayerMask;

        //characterCausingDamage = HelperFunctions.GetComponentFromTopParent<HealthManager>(transform).gameObject;
        characterCausingDamage = GetComponentInParent<Character>();
    }

    void Update()
    {
        if (canDealDamage)
        {
            if (Physics.Raycast(transform.position, -transform.up, out hit, weaponLength, damageLayerMask))
            {
                if (hit.transform != null)
                {
                    if (characterTakingDamage == null)
                    {
                        //characterTakingDamage = HelperFunctions.GetComponentFromTopParent<HealthManager>(hit.transform, out healthManager).gameObject;
                        characterTakingDamage = hit.transform.GetComponentInParent<Character>();
                        healthManager = characterTakingDamage.healthManager;
                    }
                    else
                    {
                        if (characterTakingDamage == characterCausingDamage)
                            return;

                        if (!hasDealtDamage.Contains(characterTakingDamage.gameObject))
                        {
                            healthManager.TakeDamage(weaponDamage, characterCausingDamage, characterTakingDamage);
                            hasDealtDamage.Add(characterTakingDamage.gameObject);
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
