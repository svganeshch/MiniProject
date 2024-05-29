using UnityEngine;

public abstract class HealthManager : MonoBehaviour
{
    public abstract void HandleDamage(float damage);

    public float Health = 100;
    public float currentHealth;

    [HideInInspector] public Character character;
    [HideInInspector] public Enemy enemy;

    Vector3 attackDirection;
    [HideInInspector] public bool isFacingAttacker;

    private void Start()
    {
        currentHealth = Health;
        attackDirection = Vector3.zero;

        character = GetComponent<Character>();
        enemy = GetComponent<Enemy>();
    }

    public virtual void TakeDamage(float weaponDamage, GameObject attacker, GameObject receiver)
    {
        if (currentHealth <= 0)
        {
            Die();
        }

        attackDirection = (attacker.transform.position - receiver.transform.position).normalized;
        if (Vector3.Dot(receiver.transform.forward, attackDirection) >= 0.8f)
        {
            isFacingAttacker = true;
            //Debug.Log("facing towards attacker : " + isFacingAttacker);
        }

        HandleDamage(weaponDamage);
    }

    public virtual void Die()
    {
        Destroy(gameObject, 5f);
    }
}
