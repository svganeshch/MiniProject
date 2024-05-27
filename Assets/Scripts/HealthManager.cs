using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public float Health = 100;
    public float currentHealth;

    [HideInInspector] public Animator animator;
    [HideInInspector] public Character character;
    [HideInInspector] public Enemy enemy;

    private void Start()
    {
        currentHealth = Health;

        animator = GetComponent<Animator>();
        character = FindObjectOfType<Character>();
        enemy = FindObjectOfType<Enemy>();
    }

    public virtual void TakeDamage(float weaponDamage)
    {
        currentHealth -= weaponDamage;

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
