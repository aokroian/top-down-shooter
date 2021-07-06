using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject hpScroll;
    public GameObject staminaScroll;
    public GameObject reloadScroll;
    public Text bulletsInClip;
    public Text amountOfBullets;

    public PlayerController playerController;
    public Target target;
    public PlayerAmmoController ammoController;

    void Update()
    {
        var reload = playerController.GetReloadTimerPercent();
        reloadScroll.GetComponent<Scrollbar>().value = reload;
        reloadScroll.GetComponent<Image>().color = Color.Lerp(Color.blue, Color.red, reload);

        var stamina = playerController.GetStaminaPercent();
        staminaScroll.GetComponent<Scrollbar>().value = stamina;
        staminaScroll.GetComponent<Image>().color = Color.Lerp(Color.blue, Color.red, stamina);

        AmmoType ammoType = ammoController.GetCurrentAmmoType();
        bulletsInClip.text = ammoController.GetCurrentAmmoInClip().ToString();
        amountOfBullets.text = ammoController.GetAmmoLeft(ammoType).ToString();

        var hp = target.GetHPPercent();
        hpScroll.GetComponent<Scrollbar>().value = hp;
        hpScroll.GetComponent<Image>().color = Color.Lerp(Color.black, Color.red, hp);
    }

    public void StartShooting()
    {
        playerController.StartShooting();
    }

    public void StopShooting()
    {
        playerController.StopShooting();
    }

    public void Reload()
    {
        playerController.Reload();
    }

    public void NextWeapon()
    {
        playerController.NextWeapon();
    }

    public void Dodge()
    {
        playerController.Dodge();
    }
}
