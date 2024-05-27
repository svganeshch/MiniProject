public class EnemyHealthManager : HealthManager
{
    public override void TakeDamage(float weaponDamage)
    {
        base.TakeDamage(weaponDamage);

        enemy.animator.SetTrigger("damage");
    }

    public override void Die()
    {
        enemy.isDead = true;
        animator.SetTrigger("isDead");

        base.Die();
    }
}
