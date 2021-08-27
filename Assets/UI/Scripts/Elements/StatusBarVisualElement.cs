using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StatusBarVisualElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<StatusBarVisualElement> { }

    private VisualElement healthEl;
    private VisualElement staminaEl;
    private Label creditEl;
    private Label bulletsInClipEl;
    private Label bulletsAmountEl;

    //private WeaponPreviewVisualElement weaponPreview;
    private VisualElement weaponPreview;

    private EventCallback<GeometryChangedEvent> initCallback;

    public StatusBarVisualElement()
    {
        initCallback = e => Init();
        this.RegisterCallback(initCallback);
    }

    public void Init()
    {
        healthEl = this.Q("HealthBar");
        staminaEl = this.Q("StaminaBar");
        creditEl = this.Q<Label>("CreditLabel");
        bulletsInClipEl = this.Q<Label>("BulletsInClip");
        bulletsAmountEl = this.Q<Label>("BulletsAmount");
        //weaponPreview = this.Q<WeaponPreviewVisualElement>();
        //weaponPreview.Init();
        weaponPreview = this.Q("WeaponPreview");

        this.UnregisterCallback(initCallback);
    }

    public void SetHealth(float health)
    {
        healthEl.style.width = new StyleLength(new Length(health * 100f, LengthUnit.Percent));
    }

    public void SetStamina(float stamina)
    {
        staminaEl.style.width = new StyleLength(new Length(stamina * 100f, LengthUnit.Percent));
    }

    public void SetCreditCount(int credit)
    {
        creditEl.text = credit.ToString();
    }
    
    public void SetAmmo(int inClip, int amount)
    {
        bulletsInClipEl.text = inClip.ToString();
        bulletsAmountEl.text = amount == -1 ? '\u221E'.ToString() : amount.ToString(); // infinity sign
    }

    public void SetWeaponPreviewTexture(Texture2D texture)
    {
        weaponPreview.style.backgroundImage = new StyleBackground(texture);
    }
}
