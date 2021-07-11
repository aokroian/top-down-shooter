using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectorVisualElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<SelectorVisualElement> { }

    private List<string> values = new List<string>();
    private int selectedInd;

    private Label selectedLabel;

    public SelectorVisualElement()
    {
        this.RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    public void OnGeometryChange(GeometryChangedEvent evt)
    {
        selectedLabel = this.Q<Label>("SelectedLabel");
        Debug.Log("SelectedLabel " + selectedLabel);
        this.Q<Button>("Prev").RegisterCallback<ClickEvent>(e => Prev());
        this.Q<Button>("Next").RegisterCallback<ClickEvent>(e => Next());

        Redraw();

        this.UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }


    public void SetValues(List<string> values)
    {
        this.values = values;
        Redraw();
    }

    public bool SetSelected(string selected)
    {
        int ind = values.IndexOf(selected);
        this.selectedInd = ind >= 0 ? ind : 0;
        Redraw();
        return ind >= 0;
    }

    public void SetSelectedInd(int selectedInd)
    {
        this.selectedInd = selectedInd;
        Redraw();
    }

    public void Prev()
    {
        int indTmp = selectedInd - 1;
        selectedInd = indTmp < 0 ? values.Count - 1 : indTmp;
        Redraw();
    }

    public void Next()
    {
        int indTmp = selectedInd + 1;
        selectedInd = indTmp > values.Count - 1 ? 0 : indTmp;
        Redraw();
    }

    private void Redraw()
    {
        if (selectedLabel == null)
        {
            return;
        }
        if (values.Count <= selectedInd)
        {
            selectedInd = 0;
        }
        if (values.Count > 0)
        {
            selectedLabel.text = values[selectedInd];
        }
    }
}
