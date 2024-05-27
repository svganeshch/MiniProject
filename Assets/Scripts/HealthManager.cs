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
        character = GetComponent<Character>();
        enemy = GetComponent<Enemy>();
    }

    public virtual void TakeDamage(float weaponDamage)
    {
        currentHealth -= weaponDamage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject, 5f);
    }
}
