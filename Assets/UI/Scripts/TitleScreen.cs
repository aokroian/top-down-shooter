using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class TitleScreen : MonoBehaviour
{
    public MainUIController manager;


    private VisualElement rootEl;

    private void OnEnable()
    {
        rootEl = GetComponent<UIDocument>().rootVisualElement;
        rootEl.Q("NewRun").RegisterCallback<ClickEvent>(e => manager.ToUpgradeScreen());
    }
}
