using UnityEngine;
using UnityEngine.UI;

public class WeaponScreenController : MonoBehaviour
{
    public ScrollRect scrollView;
    public GameObject container;
    public GameObject scrollItemPrefub;
    public Text amountText;
    public int amount = 500000;
    public int maxEquipedGun = 2;

    private int equipedNow = 0;

    // Получение оружий и вывод их на экран
    void Start()
    {
        AbstractUpgrade[] weapons = Resources.LoadAll<AbstractUpgrade>("UpgradesSO");

        foreach (AbstractUpgrade weapon in weapons)
        {
            GenerateItem(weapon);
        }
        scrollView.verticalNormalizedPosition = 1;
        amountText.text = amount.ToString();
    }

    void GenerateItem(AbstractUpgrade weapon)
    {
        GameObject scrollItem = Instantiate(scrollItemPrefub);
        scrollItem.GetComponent<WeaponButtonController>().SetParent(this);
        scrollItem.transform.SetParent(container.transform, false);

        scrollItem.GetComponent<Image>().sprite = Sprite.Create(weapon.image, new Rect(0.0f, 0.0f, weapon.image.width, weapon.image.height), new Vector2(0.5f, 0.5f));
        FindItemById<Text>(scrollItem, "WeaponNameText").text = weapon.name;
        FindItemById<Text>(scrollItem, "PropertyText").text = weapon.description;
        FindItemById<Text>(scrollItem, "CostText").text = weapon.cost.ToString();
        FindItemById<Text>(scrollItem, "WeaponNameText").text = weapon.name;
        // TODO STUB!
        //FindItemById<Image>(scrollItem, "IsPurchasedImage").gameObject.SetActive(false);
        //FindItemById<Image>(scrollItem, "IsEquipedImage").gameObject.SetActive(false);
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

    public bool CanBeEquiped()
    {
        if (equipedNow < maxEquipedGun)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Equip()
    {
        equipedNow++;
    }

    public void DeEquip()
    {
        equipedNow--;
    }

    //Интерфейсная дрочь
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

    private T FindItemById<T>(GameObject parent, string id) {
        return parent.transform.Find(id).GetComponent<T>();
    }
}
