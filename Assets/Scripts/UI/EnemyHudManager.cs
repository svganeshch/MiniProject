using UnityEngine;
using UnityEngine.UI;

public class EnemyHudManager : HudManager
{
    public Slider healthSlider;

    protected override void Update()
    {
        LookCamera();
    }

    private void LookCamera()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - PlayerCamera.Instance.playerCameraObjTransform.position);
    }

    public override void InitializeStatBars()
    {
        base.InitializeStatBars();

        healthSlider.maxValue = healthManager.Health;
    }

    public override void SetHealth()
    {
        base.SetHealth();

        healthSlider.value = healthManager.currentHealth;
    }
}
