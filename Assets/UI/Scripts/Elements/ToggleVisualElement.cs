using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ToggleVisualElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ToggleVisualElement> { }

    public delegate void ClickCallback(bool value);

    private bool value;
    private ClickCallback callback;

    public ToggleVisualElement()
    {
        this.RegisterCallback<ClickEvent>(e => HandleClick());
    }

    public void SetLabels(string offText, string onText)
    {
        this.Q<Label>("OffLabel").text = offText;
        this.Q<Label>("OnLabel").text = onText;
    }

    public void SetToggleCallback(ClickCallback callback)
    {
        this.callback = callback;
    }

    public void SetValue(bool value)
    {
        this.value = value;
        Redraw();
    }

    private void HandleClick()
    {
        value = !value;
        Redraw();
        this.callback?.Invoke(value);
    }

    private void Redraw()
    {
        if (value)
        {
            RemoveFromClassList("unchecked");
            AddToClassList("checked");
        } else
        {
            RemoveFromClassList("checked");
            AddToClassList("unchecked");
        }
    }
}
