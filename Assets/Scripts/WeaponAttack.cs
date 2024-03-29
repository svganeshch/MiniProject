using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAttack : MonoBehaviour
{
    Character targetPlayer;
    Enemy targetEnemy;

    bool isPlayer;
    bool isEnemy;
    bool canDealDamage;
    List<GameObject> hasDealtDamage;

    public float weaponLength;
    public float weaponDamage;
    public LayerMask damageLayerMask;

    void Start()
    {
        canDealDamage = false;
        hasDealtDamage = new List<GameObject>();

        if (damageLayerMask == Character.instance.playerLayerMask)
        {
            isEnemy = true;
        }
        else if (damageLayerMask == Character.instance.enemyLayerMask)
        {
            isPlayer = true;
        }
    }

    void Update()
    {
        if (canDealDamage)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, -transform.up, out hit, weaponLength, damageLayerMask))
            {
                if (isPlayer)
                {
                    if (hit.transform.TryGetComponent(out targetEnemy) && !hasDealtDamage.Contains(hit.transform.gameObject))
                    {
                        targetEnemy.TakeDamage(weaponDamage);
                        hasDealtDamage.Add(hit.transform.gameObject);
                    }
                }
                else if (isEnemy)
                {
                    if (hit.transform.TryGetComponent(out targetPlayer) && !hasDealtDamage.Contains(hit.transform.gameObject))
                    {
                        targetPlayer.TakeDamage(weaponDamage);
                        hasDealtDamage.Add(hit.transform.gameObject);
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
