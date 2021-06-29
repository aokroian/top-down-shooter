using UnityEngine;
using UnityEngine.UI;

public class WeaponButtonController : MonoBehaviour
{
    public Text coastText;
    public Image purchasedImage;
    public Image equipImage;

    private WeaponScreenController controller;
    private bool isPurchased;
    private bool isEquiped;

    private void Update()
    {
        isPurchased = purchasedImage.IsActive();
        isEquiped = equipImage.IsActive();
    }

    public void SetParent(WeaponScreenController controller)
    {
        this.controller = controller;
    }

    public void BuyButtonClick()
    {
        if (!isPurchased)
        {

            if (controller.SetPurchased(int.Parse(coastText.text)))
            {
                purchasedImage.gameObject.SetActive(true);
            }
        }
    }

    public void EquipButtonClick()
    {
        if (isPurchased)
        {
            if (isEquiped)
            {
                controller.DeEquip();
                equipImage.gameObject.SetActive(false);
            }
            else
            {
                if (controller.CanBeEquiped())
                {
                    controller.Equip();
                    equipImage.gameObject.SetActive(true);
                }
            }
        }
    }

}
