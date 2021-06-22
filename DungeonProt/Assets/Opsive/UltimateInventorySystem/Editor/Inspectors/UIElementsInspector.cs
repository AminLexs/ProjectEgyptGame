/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.Inspectors
{
    using Opsive.Shared.Editor.UIElements;
    using System.Collections.Generic;
    using UnityEngine.UIElements;

    /// <summary>
    /// The base inspector that converts the default IMGUI inspector to a UIElements inspector.
    /// </summary>
    public abstract class UIElementsInspector : UnityEditor.Editor
    {
        protected abstract List<string> PropertiesToExclude { get; }

        /// <summary>
        /// Create a custom inspector by drawing all serialized fields as UIElements.
        /// </summary>
        /// <returns>The custom inspector.</returns>
        public override VisualElement CreateInspectorGUI()
        {
            return UIElementsUtility.CreateUIElementInspectorGUI(serializedObject, PropertiesToExclude);
        }
    }
}
