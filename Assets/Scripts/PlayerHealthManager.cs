public class PlayerHealthManager : HealthManager
{
    public override void TakeDamage(float weaponDamage)
    {
        base.TakeDamage(weaponDamage);

        character.characterMovementSM.ChangeState(character.hitState);
    }

    public override void Die()
    {
        // Trigger dead state

        base.Die();
    }
}
