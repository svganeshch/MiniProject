using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAttack : MonoBehaviour
{
    Character character;

    bool canDealDamage;
    List<GameObject> hasDealtDamage;

    [SerializeField] protected float weaponLength;
    [HideInInspector]public float weaponDamage;
    protected LayerMask damageLayerMask;

    void Start()
    {
        character = FindObjectOfType<Character>();
        damageLayerMask = character.enemyLayerMask;

        canDealDamage = false;
        hasDealtDamage = new List<GameObject>();
    }

    void Update()
    {
        if (canDealDamage)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, -transform.up, out hit, weaponLength, damageLayerMask))
            {
                if (hit.transform.TryGetComponent(out Enemy enemy) && !hasDealtDamage.Contains(hit.transform.gameObject))
                {
                    enemy.TakeDamage(weaponDamage);
                    hasDealtDamage.Add(hit.transform.gameObject);
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
