using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponScreenController : MonoBehaviour
{
    public ScrollRect scrollView;
    public GameObject container;
    public GameObject scrollItemPrefub;
    public Text amountText;
    public int amount = 500000;

    //TODO STUB DATA
    private List<Weapon> weapons = new List<Weapon>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            // TODO STUB!
            weapons.Add(new Weapon("Item " + i, "properties " + i, 100000 * i, false));
        }

        for (int i = 0; i < 10; i++)
        {
            GenerateItem(weapons[i]);
        }
        scrollView.verticalNormalizedPosition = 1;
        amountText.text = amount.ToString();
    }

    void GenerateItem(Weapon weapon)
    {
        GameObject scrollItem = Instantiate(scrollItemPrefub);
        scrollItem.GetComponent<BuyButtonController>().SetParent(this);
        Transform item = scrollItem.transform;
        item.SetParent(container.transform, false);
        item.Find("WeaponNameText").gameObject.GetComponent<Text>().text = weapon.Name;
        item.Find("PropertyText").gameObject.GetComponent<Text>().text = weapon.Properties;
        item.Find("CoastText").gameObject.GetComponent<Text>().text = weapon.Price.ToString();
        item.Find("IsPurchasedImage").gameObject.SetActive(weapon.IsPurchased);
    }

    public bool SetPurchased(int price)
    {
        if (price <= amount)
        {
            amount -= price;
            amountText.text = amount.ToString();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnMenuItemEnter(GameObject item)
    {
        item.GetComponent<Text>().color = Color.yellow;
    }

    public void OnMenuItemExit(GameObject item)
    {
        item.GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f);
    }

    public void OnBack(LevelLoader item) {
        item.LoadLevel("StartMenu");
    }

    public void OnNewGame(LevelLoader item)
    {
        item.LoadLevel("Main");
    }
}

public class Weapon
{
    public Weapon(string name, string properties, int price, bool isPurchased)
    {
        Name = name;
        Properties = properties;
        Price = price;
        IsPurchased = isPurchased;
    }

    public string Name { get; set; }
    public string Properties { get; set; }
    public int Price { get; set; }
    public bool IsPurchased { get; set; }
}
