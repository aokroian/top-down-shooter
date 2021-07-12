using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ReloadBarVisualElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ReloadBarVisualElement> { }

    private VisualElement reloadEl;
    private bool shouldShow = true;

    private EventCallback<GeometryChangedEvent> initCallback;

    public ReloadBarVisualElement()
    {
        initCallback = e => Init();
        this.RegisterCallback(initCallback);
    }

    public void Init()
    {
        reloadEl = this.Q("ReloadBar");

        this.UnregisterCallback(initCallback);
    }

    public void SetReloadProgress(float value)
    {
        if (shouldShow && value == 0f)
        {
            this.style.display = DisplayStyle.None;
            shouldShow = false;
        }
        else if (value != 0f)
        {
            this.style.display = DisplayStyle.Flex;
            reloadEl.style.width = new StyleLength(new Length(value * 100f, LengthUnit.Percent));
            shouldShow = true;
        }
    }
}
