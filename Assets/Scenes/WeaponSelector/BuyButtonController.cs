using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyButtonController : MonoBehaviour
{
    private bool isPurchased;
    public Text coastText;
    public Image isPurchasedImage;

    private WeaponScreenController controller;

    private void Update()
    {
        isPurchased = isPurchasedImage.IsActive();
    }

    public void SetParent(WeaponScreenController controller)
    {
        this.controller = controller;
    }

    public void ButtonClick()
    {
        if (!isPurchased)
        {

            if (controller.SetPurchased(int.Parse(coastText.text)))
            {
                isPurchasedImage.gameObject.SetActive(true);
            }
        }
    }

}
