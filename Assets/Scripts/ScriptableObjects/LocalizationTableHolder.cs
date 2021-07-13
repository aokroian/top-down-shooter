using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

[CreateAssetMenu(menuName = "Custom/LocalizationTableHolder", order = 56)]
public class LocalizationTableHolder : ScriptableObject
{
    [SerializeField]
    public LocalizedStringTable localizedStringTable;

    public StringTable currentTable { get; private set; }

    public void Init()
    {
        localizedStringTable.TableChanged += OnTableChanged;
    }

    private void OnTableChanged (StringTable table)
    {
        this.currentTable = table;
    }

    // Translate or return input
    public string Translate(string token)
    {
        var entry = currentTable[token];
        return entry == null ? token : entry.LocalizedValue;
    }
}
