using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConfirmWindowVisualElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ConfirmWindowVisualElement> { }

    private Label textLabel;
    private Button confirmButton;
    private Button cancelButton;

    private LocalizationTableHolder localizer;

    private EventCallback<GeometryChangedEvent> initCallback;

    public ConfirmWindowVisualElement()
    {
        initCallback = e => OnGeometryChange();
        this.RegisterCallback(initCallback);
    }

    public void Init(LocalizationTableHolder localizer = null)
    {
        SetLocalizer(localizer);
        textLabel = this.Q<Label>("TextLabel");
        confirmButton = this.Q<Button>("ConfirmButton");
        cancelButton = this.Q<Button>("CancelButton");
    }

    private void OnGeometryChange()
    {
        textLabel = this.Q<Label>("TextLabel");
        confirmButton = this.Q<Button>("ConfirmButton");
        cancelButton = this.Q<Button>("CancelButton");

        confirmButton.text = Localize(confirmButton.text);
        cancelButton.text = Localize(cancelButton.text);

        if (localizer != null && localizer.currentTable.LocaleIdentifier.Code == "zh-Hans")
        {
            Debug.Log("BeforeStyleChange");
            ChangeStyleRecursively(this);
            Debug.Log("afterStyleChange");
        }

        this.UnregisterCallback(initCallback);
    }

    public void SetLocalizer(LocalizationTableHolder localizer)
    {
        this.localizer = localizer;
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
    

    public void SetText(string text)
    {
        textLabel.text = Localize(text);
    }

    public void SetConfirmButtonLabel(string text)
    {
        confirmButton.text = Localize(text);
    }

    public void SetCancelButtonLabel(string text)
    {
        cancelButton.text = Localize(text);
    }

    public void SetConfirmCallback(EventCallback<ClickEvent> callback)
    {
        confirmButton.RegisterCallback(callback);
    }

    public void SetCancelCallback(EventCallback<ClickEvent> callback)
    {
        cancelButton.RegisterCallback(callback);
    }

    private string Localize(string text)
    {
        string result = text;
        if (localizer != null)
        {
            result = localizer.Translate(text.TrimStart('#'));
        }
        return result;
    }
}
