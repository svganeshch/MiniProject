public class PlayerHealthManager : HealthManager
{
    public override void HandleDamage(float damage)
    {
        if (isFacingAttacker && character.characterMovementSM.currentState == character.blockState)
        {
            currentHealth -= damage / 2;

            character.characterMovementSM.ChangeState(character.blockBrokenState);
        }
        else
        {
            currentHealth -= damage;
            character.characterMovementSM.ChangeState(character.hitState);
        }
    }

    public override void Die()
    {
        // Trigger dead state

        base.Die();
    }
}
