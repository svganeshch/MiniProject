using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHudManager : HudManager
{
    public Image healthbar;
    public Image hand;
    public Image katana;
    public Image greatSword;
    public Image daggers;

    private float maxHealth;
    private float healthPercentage;

    private int previousWeaponSlot;
    private int currentWeaponSlot;

    private float transitionDuration = 0.25f;

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

    public override void SetWeaponWheel(int weaponSlot)
    {
        previousWeaponSlot = currentWeaponSlot;
        currentWeaponSlot = weaponSlot;

        StartCoroutine(WheelChange(previousWeaponSlot, currentWeaponSlot));
    }

    private IEnumerator WheelChange(int previousSlot, int currentSlot)
    {
        Image previousImage = GetImageBySlot(previousSlot);
        Image currentImage = GetImageBySlot(currentSlot);

        float elapsedTime = 0f;

        if (previousImage != null)
        {
            float startFill = previousImage.fillAmount;
            while (elapsedTime < transitionDuration)
            {
                previousImage.fillAmount = Mathf.Lerp(startFill, 0, elapsedTime / transitionDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            previousImage.fillAmount = 0;
        }

        elapsedTime = 0f;

        if (currentImage != null)
        {
            float startFill = currentImage.fillAmount;
            while (elapsedTime < transitionDuration)
            {
                currentImage.fillAmount = Mathf.Lerp(startFill, 1, elapsedTime / transitionDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            currentImage.fillAmount = 1;
        }
    }

    private Image GetImageBySlot(int slot)
    {
        switch (slot)
        {
            case 0: return hand;
            case 1: return katana;
            case 2: return greatSword;
            case 3: return daggers;
            default: return hand;
        }
    }
}