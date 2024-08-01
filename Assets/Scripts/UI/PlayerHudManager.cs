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
    public Image infoMessage;

    public RectTransform crossHairObj;
    private Transform crosshairTarget;

    private float maxHealth;
    private float healthPercentage;

    private int previousWeaponSlot;
    private int currentWeaponSlot;

    private float transitionDuration = 0.25f;
    private float infoMsgAlphaDuration = 2.5f;
    private float infoMsgDuration = 5f;

    private bool crosshairTargetSet = false;

    private Camera cam;

    private void OnDisable()
    {
        infoMessage.sprite = null;
        infoMessage.color = new Color(infoMessage.color.r, infoMessage.color.g, infoMessage.color.b, 0);
    }

    protected override void Start()
    {
        base.Start();

        cam = Camera.main;
    }

    protected override void Update()
    {
        base.Update();

        if (crosshairTargetSet)
            UpdateCrosshairPosition();
    }

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

    public override void SetLockedOnTargetCrosshair(Character target)
    {
        if (target == null)
        {
            crosshairTargetSet = false;
            crossHairObj.anchoredPosition = Vector2.zero;
            crossHairObj.gameObject.SetActive(false);

            return;
        }

        crosshairTarget = target.targetLockCast.transform;
        crosshairTargetSet = true;
        crossHairObj.gameObject.SetActive(true);
    }

    private void UpdateCrosshairPosition()
    {
        crossHairObj.position = RectTransformUtility.WorldToScreenPoint(Camera.main, crosshairTarget.position);
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

    public override void SetInfoMessage(Sprite msg)
    {
        base.SetInfoMessage(msg);

        infoMessage.sprite = msg;

        StartCoroutine(DisplayMessageAlpha());
    }

    private IEnumerator DisplayMessageAlpha()
    {
        float elapsedTime = 0f;
        float startAlpha = infoMessage.color.a;

        while (elapsedTime < infoMsgAlphaDuration)
        {
            infoMessage.color = new Color(infoMessage.color.r, infoMessage.color.g, infoMessage.color.b, Mathf.Lerp(startAlpha, 1, elapsedTime / infoMsgAlphaDuration));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        yield return new WaitForSeconds(infoMsgDuration);
        StartCoroutine(FadeDisplaymessage());
    }

    private IEnumerator FadeDisplaymessage()
    {
        float elapsedTime = 0f;
        float startAlpha = infoMessage.color.a;

        while (elapsedTime < infoMsgAlphaDuration)
        {
            infoMessage.color = new Color(infoMessage.color.r, infoMessage.color.g, infoMessage.color.b, Mathf.Lerp(startAlpha, 0, elapsedTime / infoMsgAlphaDuration));
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}