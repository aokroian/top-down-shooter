using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        
    }

    
    void Update()
    {
        var health = playerController.GetHealthPercent();
        hpScroll.GetComponent<Scrollbar>().value = health;
        hpScroll.GetComponent<Image>().color = Color.Lerp(Color.blue, Color.red, health);

        var reload = playerController.GetReloadTimerPercent();
        reloadScroll.GetComponent<Scrollbar>().value = reload;
        reloadScroll.GetComponent<Image>().color = Color.Lerp(Color.blue, Color.red, reload);

        var stamina = playerController.GetStaminaPercent();
        staminaScroll.GetComponent<Scrollbar>().value = stamina;
        staminaScroll.GetComponent<Image>().color = Color.Lerp(Color.blue, Color.red, stamina);

        bulletsInClip.text = playerController.GetBulletsInClip().ToString();
        amountOfBullets.text = playerController.GetAmountOfBullets().ToString();
    }
}
