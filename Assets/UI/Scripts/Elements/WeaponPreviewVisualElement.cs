using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponPreviewVisualElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<WeaponPreviewVisualElement> { }

    private Image image;

    private EventCallback<GeometryChangedEvent> initCallback;

    public WeaponPreviewVisualElement()
    {
        //initCallback = e => Init();
        //this.RegisterCallback(initCallback);
    }

    public void Init()
    {
        image = new Image();
        this.Add(image);
        this.UnregisterCallback(initCallback);
    }

    public void SetTexture(Texture texture)
    {
        image.image = texture;
    }
}
