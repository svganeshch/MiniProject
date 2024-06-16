using UnityEngine;

public class HudManager : MonoBehaviour
{
    public HealthManager healthManager;


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

    public virtual void InitializeStatBars() { }

    public virtual void SetHealth() { }
}
