using UnityEngine;
using UnityEngine.UI;

public class PlayerHudManager : HudManager
{
    public Image healthbar;

    private float maxHealth;
    private float healthPercentage;

    public override void InitializeStatBars()
    {
        base.InitializeStatBars();

        maxHealth = healthManager.Health;
        healthbar.fillAmount = Mathf.Clamp01(maxHealth / 100f);
    }

    public override void SetHealth()
    {
        base.SetHealth();

        healthPercentage = healthManager.currentHealth / maxHealth;
        healthbar.fillAmount = healthPercentage;
    }
}
