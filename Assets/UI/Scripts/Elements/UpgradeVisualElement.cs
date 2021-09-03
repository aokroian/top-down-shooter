using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradeVisualElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<UpgradeVisualElement> { }

    private LocalizationTableHolder localizer;

    public UpgradeVisualElement()
    {
    }

    public void SetLocalizer(LocalizationTableHolder localizer)
    {
        this.localizer = localizer;
        if (localizer.currentTable.LocaleIdentifier.Code == "zh-Hans")
        {
            ChangeStyleRecursively(this);
        }
    }

    void ChangeStyleRecursively(VisualElement element)
    {
        VisualElement.Hierarchy elementHierarchy = element.hierarchy;
        int numChildren = elementHierarchy.childCount;
        for (int i = 0; i < numChildren; i++)
        {
            VisualElement child = elementHierarchy.ElementAt(i);
            child.RemoveFromClassList("primary-text");
            child.AddToClassList("secondary-text");
        }
        for (int i = 0; i < numChildren; i++)
        {
            VisualElement child = elementHierarchy.ElementAt(i);
            VisualElement.Hierarchy childHierarchy = child.hierarchy;
            int numGrandChildren = childHierarchy.childCount;
            if (numGrandChildren != 0)
                ChangeStyleRecursively(child);
        }
    }

    public void SetIcon(Texture2D icon)
    {
        this.Q("Image").style.backgroundImage = new StyleBackground(icon);
    }

    public void SetName(string name)
    {
        this.Q<Label>("Name").text = localizer.Translate(name);
    }

    /*
    public void SetDescription(string description)
    {
        //this.Q<Label>("Description").text = localizer.Translate(description);
        this.description = description;
    }
    */

    // Add type enum
    public void SetUpgrade(bool value, string description)
    {
        //this.Q<Label>("ButtonLabel").text = value ? localizer.Translate("Upgrade") : localizer.Translate("Unlock");
        this.Q<Label>("ButtonLabel").text = value ? localizer.Translate(description) : localizer.Translate("Unlock");
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
        button.text = selected ? localizer.Translate("Unequip") : localizer.Translate("Equip");
        this.Q("Image").style.backgroundColor = new StyleColor(selected ? new Color(0f, 0.09803922f, 0.2509804f) : new Color(0f, 0.04705882f, 0.1215686f));
    }

    public void SetUpgradeButtonCallback(EventCallback<ClickEvent> callback)
    {
        this.Q<Button>("UpgradeButton").RegisterCallback(callback);
    }

    public void SetEquipButtonCallback(EventCallback<ClickEvent> callback)
    {
        this.Q<Button>("EquipButton").RegisterCallback(callback);
    }

    public void SetNeedUpgradeButton(bool needButton)
    {
        //this.Q<Button>("UpgradeButton").style.display = needButton ? DisplayStyle.Flex : DisplayStyle.None;
        this.Q<Button>("UpgradeButton").style.visibility = needButton ? Visibility.Visible : Visibility.Hidden;
    }
}
