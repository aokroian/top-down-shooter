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
        this.UnregisterCallback(initCallback);
    }

    public void SetLocalizer(LocalizationTableHolder localizer)
    {
        this.localizer = localizer;
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
