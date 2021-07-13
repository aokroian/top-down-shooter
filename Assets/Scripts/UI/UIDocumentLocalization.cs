// void* src = https://gist.github.com/andrew-raphael-lukasik/72a4d3d14dd547a1d61ae9dc4c4513da
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

// NOTE: this class assumes that you designate StringTable keys in label fields (as seen in Label, Button, etc)
// and start them all with '#' char (so other labels will be left be)
// example: https://i.imgur.com/H5RUIej.gif

[DisallowMultipleComponent]
[RequireComponent(typeof(UIDocument))]
public class UIDocumentLocalization : MonoBehaviour
{

	[SerializeField] LocalizedStringTable _table = null;
	UIDocument _document;

	public bool shouldRebuild;

	private Action rebuildCallback;

	private StringTable currentTable;

	/// <summary> Executed after hierarchy is cloned fresh and translated. </summary>
	public event System.Action onCompleted = () => { };


	void OnEnable()
	{
		if (_document == null)
			_document = gameObject.GetComponentInParent<UIDocument>(includeInactive: true);

		_table.TableChanged += OnTableChanged;
		//Debug.Log("Enabled " + gameObject.name);
	}


	void OnDisable()
	{
		//_table.TableChanged -= OnTableChanged;
		//Debug.Log("Disabled " + gameObject.name);
	}


	void OnTableChanged(StringTable table)
	{
		var root = _document.rootVisualElement;

		if (root == null)
		{
			return;
		}

		if (shouldRebuild)
		{
			root.Clear();
			_document.visualTreeAsset.CloneTree(root);
		}

		var op = _table.GetTableAsync();
		op.Completed -= OnTableLoaded;
		op.Completed += OnTableLoaded;
	}

	void OnTableLoaded(AsyncOperationHandle<StringTable> op)
	{
		StringTable table = op.Result;
		currentTable = table;
		var root = _document.rootVisualElement;

		//Debug.Log("Document: " + _document + "; root: " + root);
		LocalizeChildrenRecursively(root, table);
		onCompleted();

		if (shouldRebuild)
        {
			rebuildCallback?.Invoke();
        }

		root.MarkDirtyRepaint();
	}

	void Localize(VisualElement next, StringTable table)
	{
		if (typeof(TextElement).IsInstanceOfType(next))
		{
			TextElement textElement = (TextElement)next;
			string key = textElement.text;
			if (!string.IsNullOrEmpty(key) && key[0] == '#')
			{
				key = key.TrimStart('#');
				StringTableEntry entry = table[key];
				if (entry != null)
				{
					//Debug.Log("Localize: " + textElement.text + " - " + entry.LocalizedValue);
					textElement.text = entry.LocalizedValue;
				}
				else
				{
					Debug.LogWarning($"No {table.LocaleIdentifier.Code} translation for key: '{key}'");
				}
			}
		}
	}

	void LocalizeChildrenRecursively(VisualElement element, StringTable table)
	{
		VisualElement.Hierarchy elementHierarchy = element.hierarchy;
		int numChildren = elementHierarchy.childCount;
		for (int i = 0; i < numChildren; i++)
		{
			VisualElement child = elementHierarchy.ElementAt(i);
			Localize(child, table);
		}
		for (int i = 0; i < numChildren; i++)
		{
			VisualElement child = elementHierarchy.ElementAt(i);
			VisualElement.Hierarchy childHierarchy = child.hierarchy;
			int numGrandChildren = childHierarchy.childCount;
			if (numGrandChildren != 0)
				LocalizeChildrenRecursively(child, table);
		}
	}

	public void SetRebuildCallback(Action callback)
    {
		Debug.Log("RET REBUILD CALLBACK");
		rebuildCallback = callback;
    }

	// Translate or return input
	public string Translate(string token)
    {
		var entry = currentTable?[token];
		return entry == null ? token : entry.LocalizedValue;
    }
}