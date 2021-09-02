using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUpgradeController : MonoBehaviour
{
    public MainUIController manager;
    public UIDocument uiDocument;
    public ProgressionHolder progressionHolder;
    public GameEvent progressionSaveEvent;
    public AudioSource audioSource;
    public FloatVariable defaultHealth;
    public FloatVariable defaultStamina;

    private VisualElement rootEl;

    private void OnEnable()
    {
        rootEl = uiDocument.rootVisualElement;

        rootEl.Q("BackToTitle").RegisterCallback<ClickEvent>(e => manager.ToUpgradeScreen());
        rootEl.Q("StartGame").RegisterCallback<ClickEvent>(e => manager.StartGame());

        FillScreen();
    }

    private void FillScreen()
    {
        SetExp();
        FillUpgrade(PlayerUpgradeType.LIFE, "Life", (int) defaultHealth.value);
        FillUpgrade(PlayerUpgradeType.AMMO, "Ammo", 100);
        FillUpgrade(PlayerUpgradeType.STAMINA, "Stamina", (int) defaultStamina.value);
    }

    private void FillUpgrade(PlayerUpgradeType type, string elPrefix, int startValue) {
        var upgrades = progressionHolder.GetPurchasedPlayerUpgrades();
        var filtered = upgrades.Where(v => v.playerUpgradeType == type);
        var added = filtered.Select(v => v.value).Sum();
        rootEl.Q<Label>("Current" + elPrefix).text = (startValue + added).ToString();

        var root = progressionHolder.GetAllPlayerUpgrades().Where(v => v.playerUpgradeType == type && v.isRoot).First();
        var next = root;
        while (filtered.Contains(next))
        {
            next = (PlayerUpgrade) (next.children.Length == 0 ? null : next.children[0]);
        }

        if (next != null)
        {
            rootEl.Q<Label>(elPrefix + "Cost").text = next.cost.ToString();
            rootEl.Q<Label>(elPrefix + "Description").text = next.description.ToString();

            EventCallback<ClickEvent> clickHandler = null;
            clickHandler = (e) => {
                if (CheckCanUpgrade(next)) {
                    rootEl.Q<Button>("Upgrade" + elPrefix).UnregisterCallback(clickHandler);
                    Purchase(next);
                    FillUpgrade(type, elPrefix, startValue);
                }
            };
            rootEl.Q<Button>("Upgrade" + elPrefix).RegisterCallback(clickHandler);
        }
        else
        {
            rootEl.Q<Label>(elPrefix + "Cost").style.display = DisplayStyle.None;
            rootEl.Q<Label>(elPrefix + "Description").text = "Max";
        }
    }

    private bool CheckCanUpgrade(PlayerUpgrade upgrade)
    {
        return progressionHolder.exp >= upgrade.cost;
    }

    private void Purchase(PlayerUpgrade upgrade)
    {
        //sound
        if (audioSource != null)
        {
            audioSource.Play();
        }
        
        progressionHolder.AddPurchasedPlayerUpgrade(upgrade);
        progressionHolder.exp -= upgrade.cost;
        SetExp();
        progressionSaveEvent.Raise();
    }

    private void SetExp()
    {
        rootEl.Q<Label>("ExpLabel").text = progressionHolder.exp.ToString();
    }
}
