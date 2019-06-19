using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class AddComponentWindow: EditorWindow
{
    private List<Type> _components;
    private string[] _dropdownOptions;
    private int _selectedIndex;

    private ScriptableObject _selectedComponent;

    private Action<ScriptableObject> _callback;
 
    public void SetSelection<T>(Action<T> onCreatedCallback) where T : ScriptableObject
    {
        _components = GetComponentList<T>();
        
        _dropdownOptions = new[] { "None" }
            .Concat(_components.Select(p => p.Name))
            .ToArray();
        
        _callback = selected =>  onCreatedCallback((T) selected);
    }

    private void OnGUI()
    {
        if (_components == null) return;
        
        GUILayout.BeginVertical();
        GUILayout.Label("Add Component", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        _selectedIndex = EditorGUILayout.Popup("Select Component: ", _selectedIndex, _dropdownOptions);
        GUILayout.Space(10);
        
        if (_selectedIndex != 0)
        {
            
            if (_selectedComponent == null || _selectedComponent.GetType() != _components[_selectedIndex - 1])
            {
                _selectedComponent = CreateInstance(_components[_selectedIndex - 1]);
            }
                
            Editor.CreateEditor(_selectedComponent).OnInspectorGUI();
        }
        else
        {
            _selectedComponent = null;
        }
        
        if (_selectedComponent != null && GUILayout.Button("Create"))
        {
            _callback?.Invoke(_selectedComponent);
            _callback = null;
            Close();
        }
        GUILayout.EndVertical();
    }
    
    private List<Type> GetComponentList<T>() where T : ScriptableObject
    {
        return
            AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(T)))
                .ToList();
    }
}