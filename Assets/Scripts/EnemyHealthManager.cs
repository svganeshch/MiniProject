public class EnemyHealthManager : HealthManager
{
    public override void HandleDamage(float damage)
    {
        currentHealth -= damage;
        enemy.animator.SetTrigger("damage");
    }

    public override void Die()
    {
        enemy.isDead = true;
        enemy.animator.SetTrigger("isDead");

        base.Die();
    }
}
