public class EnemyHealthManager : HealthManager
{
    public override void TakeDamage(float weaponDamage)
    {
        base.TakeDamage(weaponDamage);

        enemy.animator.SetTrigger("damage");
    }
}
