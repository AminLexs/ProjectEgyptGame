/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------


using UnityEngine.UIElements;

namespace Opsive.UltimateInventorySystem.Editor.VisualElements
{
    using Opsive.UltimateInventorySystem.Editor.Inspectors;
    using System;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;

    public class NestedScriptableObjectInspector<To, Ti> : VisualElement where To : ScriptableObject where Ti : InspectorBase
    {
        protected Editor m_NestedInspectorEditor;
        protected Button m_CreateScriptableObjectButton;

        public NestedScriptableObjectInspector(Action<To> onObjectCreated)
        {
            if (EditorGUIUtility.isProSkin) {
                AddToClassList("nested-scriptable-object-inspector-dark");
            } else {
                AddToClassList("nested-scriptable-object-inspector-light");
            }

            var objectTypeName = typeof(To).Name;

            if (onObjectCreated != null) {
                m_CreateScriptableObjectButton = new Button();
                m_CreateScriptableObjectButton.text = "Create a " + objectTypeName;
                m_CreateScriptableObjectButton.clicked += () =>
                {
                    var path = EditorUtility.SaveFilePanel("Create a " + objectTypeName, "Assets", objectTypeName, "asset");
                    if (path.Length != 0 && Application.dataPath.Length < path.Length) {
                        var obj = ScriptableObject.CreateInstance<To>();

                        // Save the database.
                        path = string.Format("Assets/{0}", path.Substring(Application.dataPath.Length + 1));

                        AssetDatabase.DeleteAsset(path);
                        AssetDatabase.CreateAsset(obj, path);
                        AssetDatabase.ImportAsset(path);

                        onObjectCreated?.Invoke(obj);
                    }
                };
            }
        }

        /// <summary>
        /// Update the selection visual element.
        /// </summary>
        /// <param name="index">The index selected.</param>
        public void Refresh(To obj)
        {
            Clear();

            RemoveFromClassList("nested-scriptable-object-inspector");

            if (obj == null && m_CreateScriptableObjectButton != null) {

                AddToClassList("nested-scriptable-object-inspector");
                Add(m_CreateScriptableObjectButton);

                return;
            }

            Editor.CreateCachedEditor(obj, typeof(Ti), ref m_NestedInspectorEditor);

            if (m_NestedInspectorEditor is Ti customInspector) {
                AddToClassList("nested-scriptable-object-inspector");
                customInspector.DrawInOrder(this, true);
            }
        }
    }

    public class ObjectFieldWithNestedInspector<To, Ti> : VisualElement where To : ScriptableObject where Ti : InspectorBase
    {
        protected ObjectField m_ObjectField;
        protected NestedScriptableObjectInspector<To, Ti> m_NestedInspector;
        protected Action<To> m_OnValueChanged;
        
        public ObjectField ObjectField => m_ObjectField;

        public To value
        {
            get => m_ObjectField.value as To;
            set
            {
                if(value == m_ObjectField.value){ return; }
                
                m_ObjectField.SetValueWithoutNotify(value);
                m_OnValueChanged?.Invoke(value);
                m_NestedInspector?.Refresh(value);
            }
        }

        public ObjectFieldWithNestedInspector(string fieldLabel, To defaultValue, string tooltip, Action<To> onValueChanged, bool addCreateButton = true)
        {
            m_OnValueChanged = onValueChanged;
            
            m_ObjectField = new ObjectField(fieldLabel);
            m_ObjectField.tooltip = tooltip;
            m_ObjectField.objectType = typeof(To);
            m_ObjectField.RegisterValueChangedCallback(evt =>
            {
                m_OnValueChanged?.Invoke(evt.newValue as To);
                m_NestedInspector?.Refresh(evt.newValue as To);
            });
            m_ObjectField.value = defaultValue;
            Add(m_ObjectField);

            if (addCreateButton) {
                m_NestedInspector = new NestedScriptableObjectInspector<To, Ti>(
                    (obj) =>
                    {
                        m_ObjectField.SetValueWithoutNotify(obj);
                        m_OnValueChanged?.Invoke(obj);
                        m_NestedInspector?.Refresh(obj);
                    });
            } else {
                m_NestedInspector = new NestedScriptableObjectInspector<To, Ti>(null);
            }
           
            m_NestedInspector.Refresh(defaultValue);
            Add(m_NestedInspector);
        }
    }
}
