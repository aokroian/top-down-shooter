using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradeVisualElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<UpgradeVisualElement> { }

    public UpgradeVisualElement()
    {
    }

    public void SetIcon(Texture2D icon)
    {
        this.Q("Image").style.backgroundImage = new StyleBackground(icon);
    }

    public void SetName(string name)
    {
        this.Q<Label>("Name").text = name;
    }

    public void SetDescription(string description)
    {
        this.Q<Label>("Description").text = description;
    }

    // Add type enum
    public void SetUpgrade(bool value)
    {
        this.Q<Label>("ButtonLabel").text = value ? "Upgrade" : "Unlock";
        this.Q("ProgressBar").style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void SetProgressValue(int value, int max)
    {
        //Debug.Log("value " + (value / (float) max) * 100);
        this.Q("UpgradeProgress").style.width = new StyleLength(new Length((float)(value / (float)max) * 100, LengthUnit.Percent));
        this.Q<Label>("ProgressLabel").text = value + "/" + max;
    }

    public void SetCost(int cost)
    {
        this.Q<Label>("Cost").text = cost.ToString();
    }

    public void SetSelectable(bool selectable)
    {
        this.Q<Button>("EquipButton").style.display = selectable ? DisplayStyle.Flex : DisplayStyle.None;
        this.Q<Label>("EquipPlaceholder").style.display = selectable ? DisplayStyle.None : DisplayStyle.Flex;
    }

    public void SetSelected(bool selected)
    {
        var button = this.Q<Button>("EquipButton");
        button.text = selected ? "Unequip" : "Equip";
    }

    public void SetUpgradeButtonCallback(EventCallback<ClickEvent> callback)
    {
        this.Q<Button>("UpgradeButton").RegisterCallback(callback);
    }

    public void SetEquipButtonCallback(EventCallback<ClickEvent> callback)
    {
        this.Q<Button>("EquipButton").RegisterCallback(callback);
    }
}
