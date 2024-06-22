using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    public float Health = 100;
    public float currentHealth;

    [HideInInspector] public UnityEvent onHealthManagerInitializedEvent = new();

    [HideInInspector] public Character character;

    private Vector3 attackDirection;
    [HideInInspector] public bool isFacingAttacker;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        currentHealth = Health;
        attackDirection = Vector3.zero;
        onHealthManagerInitializedEvent.Invoke();
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            if (!character.isDead)
                Die();
        }
    }

    public virtual void TakeDamage(float weaponDamage, Character attacker, Character receiver)
    {
        attackDirection = (attacker.transform.position - receiver.transform.position).normalized;
        isFacingAttacker = Vector3.Dot(receiver.transform.forward, attackDirection) >= 0.8f;

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
        Debug.Log(character.name + " damage received");
    }

    public virtual void Die()
    {
        character.isDead = true;
        character.gameObject.GetComponent<Character>().enabled = false;
        character.gameObject.GetComponent<CharacterMovementManager>().enabled = false;
        character.characterAnimatorManager.PlayDeathCommonAnimation();

        Destroy(character.gameObject, 10f);
    }
}