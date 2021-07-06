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

    public Image movementJoystick;
    public Image cameraJoystick;
    public Image shootButton;
    public Image reloadButton;
    public Image nextWeaponButton;
    public Image dodgeButton;
    public Image menuButton;

    void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        movementJoystick.gameObject.SetActive(true);
        cameraJoystick.gameObject.SetActive(true);
        shootButton.gameObject.SetActive(true);
        reloadButton.gameObject.SetActive(true);
        nextWeaponButton.gameObject.SetActive(true);
        dodgeButton.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
#endif
    }


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
