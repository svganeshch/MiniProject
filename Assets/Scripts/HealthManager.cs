using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    public float Health = 100;
    public float currentHealth;

    [HideInInspector] public UnityEvent onHealthManagerInitializedEvent = new();

    [HideInInspector] public Character character;

    Vector3 attackDirection;
    [HideInInspector] public bool isFacingAttacker;

    private void Start()
    {
        currentHealth = Health;
        attackDirection = Vector3.zero;

        character = GetComponent<Character>();

        onHealthManagerInitializedEvent.Invoke();
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void TakeDamage(float weaponDamage, Character attacker, Character receiver)
    {
        attackDirection = (attacker.transform.position - receiver.transform.position).normalized;
        if (Vector3.Dot(receiver.transform.forward, attackDirection) >= 0.8f)
        {
            isFacingAttacker = true;
            //Debug.Log("facing towards attacker : " + isFacingAttacker);
        }

        if (isFacingAttacker && character.characterStateMachine.currentState == character.blockState)
        {
            currentHealth -= weaponDamage / 2;

            character.characterStateMachine.ChangeState(character.blockBrokenState);
        }
        else
        {
            currentHealth -= weaponDamage;
            character.characterStateMachine.ChangeState(character.hitState);
        }

        character.hudManager.SetHealth();

        Debug.Log("damage received");
    }

    public virtual void Die()
    {
        character.isDead = true;
        Destroy(gameObject, 5f);
    }
}
