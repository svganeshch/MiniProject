using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    public HealthManager healthManager;


    protected virtual void Start()
    {
        healthManager.onHealthManagerInitializedEvent.AddListener(OnHealthManagerInitialized);
    }

    private void OnEnable()
    {
        OnHealthManagerInitialized();
    }

    protected virtual void Update() { }

    public void OnHealthManagerInitialized()
    {
        InitializeStatBars();
        SetHealth();
    }

    public virtual void InitializeStatBars() { }

    public virtual void SetHealth() { }

    public virtual void SetLockedOnTargetCrosshair(Character target) { }

    public virtual void SetWeaponWheel(int weaponSlot) { }

    public virtual void SetInfoMessage(Sprite msg) { }
}
