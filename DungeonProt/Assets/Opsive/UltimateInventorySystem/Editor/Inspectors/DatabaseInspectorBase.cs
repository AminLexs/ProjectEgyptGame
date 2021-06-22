/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.Inspectors
{
    using Opsive.Shared.Editor.UIElements;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Editor.Managers;
    using Opsive.UltimateInventorySystem.Editor.VisualElements;
    using Opsive.UltimateInventorySystem.Storage;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// The base inspector for any component that requires the inventory database to be drawn correctly.
    /// </summary>
    public abstract class DatabaseInspectorBase : InspectorBase
    {
        protected ObjectField m_DatabaseField;
        protected InventorySystemDatabase m_Database;
        private VisualElement m_Content;

        /// <summary>
        /// Draw the database field at the very top of the inspector.
        /// </summary>
        /// <param name="parent">The parent container.</param>
        /// <param name="nested">Is the inspector nested?.</param>
        public override void DrawInOrder(VisualElement parent, bool nested = false)
        {
            MainManagerWindow.OnLostFocusEvent -= DatabaseChanged;
            MainManagerWindow.OnLostFocusEvent += DatabaseChanged;

            m_Content = new VisualElement();

            if (target != null && nested == false) {
                parent.Add(new PropertyField(serializedObject.FindProperty("m_Script")));
            }

            parent.Add(CreateDatabase());

            InitializeInspector();

            parent.Add(m_Content);

            DatabaseChanged();
        }

        /// <summary>
        /// Initialize the component when the database changes.
        /// </summary>
        protected virtual void DatabaseChanged()
        {
            if (target == null) { return; }

            m_Database = m_DatabaseField.value as InventorySystemDatabase;
            if (m_Database == null) {
                Debug.LogWarning("This component requires a database. Please create and assign a database to the Inventory System Manager.");
            }

            m_Database?.Initialize(false);
            DrawContent();
        }

        /// <summary>
        /// Draw the database field at the very top of the inspector.
        /// </summary>
        /// <param name="parent">The parent container.</param>
        private void DrawContent()
        {
            m_Content.Clear();

            var component = target as IDatabaseSwitcher;

            if (component == null) {
                Debug.LogWarning($"This component {target.GetType()} uses a Database Inspector but it does not inherit the IDatabaseSwitcher interface.");
            }

            if (m_Database == null || component == null || component.IsComponentValidForDatabase(m_Database) == false) {
                m_Content.Add(DrawObjectsDoNotMatchDatabase());
                return;
            }

            if (target != null) {
                m_Content.Add(UIElementsUtility.CreateUIElementInspectorGUI(serializedObject, PropertiesToExclude));
            }

            CreateInspector(m_Content);
        }

        /// <summary>
        /// Create the database field.
        /// </summary>
        /// <returns>The database field.</returns>
        protected virtual VisualElement CreateDatabase()
        {
            m_DatabaseField = new ObjectField("Database");
            m_DatabaseField.objectType = typeof(InventorySystemDatabase);

            if (m_DatabaseField.value == null) {
                var manager = FindObjectOfType<InventorySystemManager>();
                m_DatabaseField.value = manager?.Database ?? MainManagerWindow.InventorySystemDatabase;
            }

            m_DatabaseField.RegisterValueChangedCallback(evt => { DatabaseChanged(); });

            m_Database = m_DatabaseField.value as InventorySystemDatabase;
            m_Database?.Initialize(false);

            return m_DatabaseField;
        }

        /// <summary>
        /// Displays the message that the database does not match.
        /// </summary>
        /// <returns>The displayed helpbox.</returns>
        protected virtual VisualElement DrawObjectsDoNotMatchDatabase()
        {
            var message = "An inventory object referenced by this component is not part of the selected database. " +
                          "Select the correct database for this object or replace the referenced object by matching ones for the currently selected database." +
                          "This process cannot be done at runtime.";

            var helpBox = new InventoryHelpBox(message);

            var button = new Button();
            button.SetEnabled(!Application.isPlaying);
            button.text = "Convert Inventory Objects";
            button.clicked += ReplaceInventoryObjectsBySelectedDatabaseEquivalents;

            helpBox.Add(button);

            return helpBox;
        }

        /// <summary>
        /// Initialize the component when the database changes.
        /// </summary>
        protected void ReplaceInventoryObjectsBySelectedDatabaseEquivalents()
        {
            var component = target as IDatabaseSwitcher;

            m_Database?.Initialize(false);

            InventorySystemManagerInspector.ReplaceInventoryObjectsBySelectedDatabaseEquivalents(component, m_Database, true);

            DrawContent();
        }

        /// <summary>
        /// Remove the event if the inspector is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            MainManagerWindow.OnLostFocusEvent -= DatabaseChanged;
        }
    }
}