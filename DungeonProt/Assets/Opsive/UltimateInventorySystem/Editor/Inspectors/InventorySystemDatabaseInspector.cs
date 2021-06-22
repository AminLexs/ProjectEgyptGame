/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.Inspectors
{
    using Opsive.UltimateInventorySystem.Editor.Managers;
    using Opsive.UltimateInventorySystem.Storage;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine.UIElements;

    /// <summary>
    /// Custom inspector for the Inventory System Database.
    /// </summary>
    [CustomEditor(typeof(InventorySystemDatabase))]
    public class InventorySystemDatabaseInspector : InspectorBase
    {
        protected override List<string> PropertiesToExclude { get; }

        /// <summary>
        /// Draw the visual elements in order.
        /// </summary>
        /// <param name="parent">The parent Container.</param>
        /// <param name="nested">Is the inspector nested?.</param>
        public override void DrawInOrder(VisualElement parent, bool nested = false)
        {
            CreateInspector(parent);
        }

        /// <summary>
        /// Creates the inspector.
        /// </summary>
        /// <param name="container">The parent container.</param>
        protected override void CreateInspector(VisualElement container)
        {
            var button = new Button();
            button.clickable.clicked += () =>
            {
                var window = MainManagerWindow.ShowWindow();
                window.Open(typeof(SetupManager));
                window.Database = target as InventorySystemDatabase;
            };
            button.text = $"Open Inventory Manager";
            container.Add(button);
        }
    }
}