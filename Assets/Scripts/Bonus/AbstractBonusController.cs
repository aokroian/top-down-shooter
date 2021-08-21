using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBonusController : MonoBehaviour
{
    public GameObject player;
    public float despawnDistance = 80f;
    public float pickUpDistance = 5f;
    public string pickupString;
    public float textTime = 2f;
    public float textMovement = 0.2f;
    public Font textFont;
    public Color textColor = Color.green;

    private bool flyToPlayer = false;
    private float currentSpeed = 2f;
    private float pickupTime = 0f;
    private Vector2 currentTextPosition;
    private GUIStyle textStyle;
    private Rect textRect;
    private string pickupText;

    private bool menuOpened;

    protected LocalizationTableHolder localizationTableHolder;

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (!flyToPlayer && distance < pickUpDistance && CanPickUp())
        {
            flyToPlayer = true;
        } else if (flyToPlayer)
        {
            Vector3 playerPosYFixed = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, playerPosYFixed, currentSpeed * Time.deltaTime);
            currentSpeed += Time.deltaTime * 10f;
        }

        if (distance >= despawnDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            if (OnPickUp())
            {
                PrepareTextProperties();
                pickupTime = Time.time;
            } else
            {
                flyToPlayer = false;
            }
        }
    }

    public abstract bool OnPickUp();

    public abstract bool CanPickUp();
    
    public abstract string GetPickupText();

    public void SetLocalizationTableHolder(LocalizationTableHolder localizationTableHolder)
    {
        this.localizationTableHolder = localizationTableHolder;
    }

    private void PrepareTextProperties()
    {
        int w = Screen.width, h = Screen.height;
        textStyle = new GUIStyle();
        textStyle.alignment = TextAnchor.UpperCenter;
        textStyle.fontSize = h * 4 / 100;
        textStyle.normal.textColor = textColor;
        textStyle.font = textFont;
        textRect = new Rect(0, 0, w / 3, h * 2 / 100);
        currentTextPosition = Camera.main.WorldToScreenPoint(player.transform.position);
        pickupText = GetPickupText();
    }

    private void OnGUI()
    {
        if (pickupTime == 0f || menuOpened)
        {
            if (menuOpened)
            {
                Debug.Log("Opened");
            }
            return;
        }

        transform.localScale = Vector3.zero;  // Or disable? Or disable MeshRenderer?
        transform.GetComponent<Collider>().enabled = false;

        if (Time.time < pickupTime + textTime)
        {
            currentTextPosition.y -= Time.deltaTime / textTime * textMovement * Screen.height;
            textRect.center = currentTextPosition;
            GUI.Label(textRect, pickupText, textStyle);
        }
        else
        {
            DestroyPickedUp();
        }
    }

    protected void DestroyPickedUp()
    {
        Destroy(gameObject);
    }

    public void OnMenuToggle(MenuToggleEventParam param)
    {
        menuOpened = param.showMenu;
        Debug.Log("Opened invoke");
    }
}
