using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public float Health = 100;
    public float currentHealth;
    
    Animator animator;
    Enemy enemy;

    private void Start()
    {
        currentHealth = Health;

        animator = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
    }

    public void TakeDamage(float weaponDamage)
    {
        currentHealth -= weaponDamage;
        animator.SetTrigger("damage");

        if (currentHealth <= 0)
        {
            Die();
        }
        Debug.Log("Enemy received damage");
    }

    private void Die()
    {
        if (enemy != null)
            enemy.isDead = true;

        animator.SetTrigger("isDead");
        Destroy(gameObject, 5f);
    }
}
