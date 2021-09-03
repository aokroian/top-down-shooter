using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerConfigurator : MonoBehaviour
{
    public ProgressionHolder progressionHolder;
    public GameObject[] glasses;
    public GameObject[] belts;
    public GameObject[] kneePads;
    public PlayerAmmoController playerAmmoController;

    public FloatVariable defaultHealth;
    public FloatVariable defaultStamina;

    private PlayerController playerController;
    private float addedHealth;
    private int countHealth;

    private float addedStamina;
    private int countStamina;

    private float addedAmmo;
    private int countAmmo;


    private void Start()
    {
        TryGetComponent<PlayerController>(out playerController);
        if (playerController == null)
        {
            ApplyAllUpgrades();
        }

        
    }

    public void ApplyAllUpgrades()
    {
        GetDataFromProgressionHolder();
        ApplyStaminaUpgrade();
        ApplyHealthUpgrade();
        ApplyAmmoUpgrade();
    }

    private void ApplyStaminaUpgrade()
    {
        SwitchModels(kneePads, countStamina);

        if (playerController == null) return;
        playerController.stamina = defaultStamina.value + addedStamina;
        playerController.maxStamina = defaultStamina.value + addedStamina;
    }
    private void ApplyHealthUpgrade()
    {
        SwitchModels(glasses, countHealth);

        if (playerController == null) return;
        Target target = playerController.GetComponent<Target>();
        target.maxHealth = defaultHealth.value + addedHealth;
        target.health = defaultHealth.value + addedHealth;
    }
    private void ApplyAmmoUpgrade()
    {
        SwitchModels(belts, countAmmo);

        if (playerController == null || playerAmmoController == null) return;
        int machinegunMax = playerAmmoController.machinegunMax;
        int rifleMax = playerAmmoController.rifleMax;
        int shotgunMax = playerAmmoController.shotgunMax;

        float additionalPercent = (1 + addedAmmo / 100);

        playerAmmoController.machinegunMax = Mathf.RoundToInt(machinegunMax * additionalPercent);
        playerAmmoController.rifleMax = Mathf.RoundToInt(rifleMax * additionalPercent);
        playerAmmoController.shotgunMax = Mathf.RoundToInt(shotgunMax * additionalPercent);

        playerAmmoController.machinegunDefault = Mathf.RoundToInt(machinegunMax * additionalPercent);
        playerAmmoController.rifleDefault = Mathf.RoundToInt(rifleMax * additionalPercent);
        playerAmmoController.shotgunDefault = Mathf.RoundToInt(shotgunMax * additionalPercent);

        playerAmmoController.CalcAmmo();
    }

    private void SwitchModels(GameObject[] arr, int index)
    {
        if (arr.Length <= 1) return;
        if (index >= arr.Length)
        {
            Debug.Log("Problem in upgrades for player");
            return;
        }
        for (int i = 0; i < arr.Length; i++)
        {
            if (i != index)
            {
                arr[i].SetActive(false);
            }
        }
        arr[index].SetActive(true);
    }

    private void GetDataFromProgressionHolder()
    {
        var filteredHealth = progressionHolder.GetPurchasedPlayerUpgrades().Where(v => v.playerUpgradeType == PlayerUpgradeType.LIFE);
        addedHealth = filteredHealth.Select(v => v.value).Sum(); // общая сумма дополнительных хп/процентов патронов/стамины
        countHealth = filteredHealth.Count(); // Количество вкачанных апгрейдов

        var filteredStamina = progressionHolder.GetPurchasedPlayerUpgrades().Where(v => v.playerUpgradeType == PlayerUpgradeType.STAMINA);
        addedStamina = filteredStamina.Select(v => v.value).Sum(); // общая сумма дополнительных хп/процентов патронов/стамины
        countStamina = filteredStamina.Count(); // Количество вкачанных апгрейдов

        var filteredAmmo = progressionHolder.GetPurchasedPlayerUpgrades().Where(v => v.playerUpgradeType == PlayerUpgradeType.AMMO);
        addedAmmo = filteredAmmo.Select(v => v.value).Sum(); // общая сумма дополнительных хп/процентов патронов/стамины
        countAmmo = filteredAmmo.Count(); // Количество вкачанных апгрейдов


        Debug.Log(addedAmmo);
    }
}
