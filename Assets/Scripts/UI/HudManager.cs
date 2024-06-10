using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    public HealthManager healthManager;
    public Slider healthSlider;

    protected virtual void Start()
    {
        healthManager.onHealthManagerInitializedEvent.AddListener(OnHealthManagerInitialized);
    }

    protected virtual void Update() { }

    public void OnHealthManagerInitialized()
    {
        InitializeStatBars();
        SetHealth();
    }

    public void InitializeStatBars()
    {
        healthSlider.maxValue = healthManager.Health;
    }

    public void SetHealth()
    {
        healthSlider.value = healthManager.currentHealth;
    }
}
