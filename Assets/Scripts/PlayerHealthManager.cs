public class PlayerHealthManager : HealthManager
{
    public override void TakeDamage(float weaponDamage)
    {
        base.TakeDamage(weaponDamage);

        character.characterMovementSM.ChangeState(character.hitState);
    }
}
