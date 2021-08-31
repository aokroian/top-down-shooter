using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfigurator : MonoBehaviour
{
    public GameObject[] glasses;
    public GameObject[] belts;
    public GameObject[] kneePads;

    private PlayerController playerController;

    private void Start()
    {
        TryGetComponent<PlayerController>(out playerController);
    }

    public void ApplyStaminaUpgrade(int index, float value)
    {
        SwitchModels(kneePads, index);

        if (playerController == null) return;
        playerController.stamina += value;
    }
    public void ApplyHealthUpgrade(int index, float value)
    {
        SwitchModels(glasses, index);

        if (playerController == null) return;
        playerController.maxHealth += value;
        playerController.health += value;
    }
    public void ApplyAmmoUpgrade(int index, float value)
    {
        SwitchModels(belts, index);

        if (playerController == null) return;
        int machinegunMax = playerController.ammoController.machinegunMax;
        int rifleMax = playerController.ammoController.rifleMax;
        int shotgunMax = playerController.ammoController.shotgunMax;

        playerController.ammoController.machinegunMax = Mathf.RoundToInt(machinegunMax * value / 100);
        playerController.ammoController.rifleMax = Mathf.RoundToInt(rifleMax * value / 100); ;
        playerController.ammoController.shotgunMax = Mathf.RoundToInt(shotgunMax * value / 100); ;

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
                kneePads[i].SetActive(false);
            }
        }
        arr[index].SetActive(true);
    }
}
