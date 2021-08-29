using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreditsScreen : MonoBehaviour
{
    public MainUIController manager;

    private VisualElement rootEl;

    private void OnEnable()
    {
        rootEl = GetComponent<UIDocument>().rootVisualElement;
        rootEl.Q("Back").RegisterCallback<ClickEvent>(e => manager.ToTitleScreen());
    }
}
