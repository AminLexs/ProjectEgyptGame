/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.Inspectors
{
    using Opsive.Shared.Editor.UIElements;
    using Opsive.Shared.Editor.Utility;
    using Opsive.UltimateInventorySystem.Editor.Styles;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// The base inspector for components and scriptable object within Ultimate Inventory System.
    /// </summary>
    public abstract class InspectorBase : UIElementsInspector
    {
        /// <summary>
        /// Create a custom inspector by overriding the base one.
        /// </summary>
        /// <returns>The custom inspector.</returns>
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            container.styleSheets.Add(InspectorUtility.LoadAsset<StyleSheet>("e70f56fae2d84394b861a2013cb384d0"));//shared uss
            container.styleSheets.Add(CommonStyles.StyleSheet);
            container.styleSheets.Add(ManagerStyles.StyleSheet);
            container.styleSheets.Add(ControlTypeStyles.StyleSheet);

            DrawInOrder(container);
            
            Highlighter.HighlightIdentifier(container.contentRect, "CustomIdentifier"+target.GetInstanceID());

            return container;
        }

        /// <summary>
        /// Initialize the inspector when it is first selected.
        /// </summary>
        protected virtual void InitializeInspector() { }

        /// <summary>
        /// Draw the serialized fields that are not excluded and then draw the custom fields.
        /// </summary>
        /// <param name="parent">The parent element.</param>
        /// <param name="nested">Is the inspector nested?.</param>
        public virtual void DrawInOrder(VisualElement parent, bool nested = false)
        {
            InitializeInspector();
            
            if (target != null && nested == false) {
                parent.Add(new UnityEditor.UIElements.PropertyField(serializedObject.FindProperty("m_Script")));
            }

            if (target != null) {
                parent.Add(UIElementsUtility.CreateUIElementInspectorGUI(serializedObject, PropertiesToExclude));
            }

            CreateInspector(parent);
        }

        /// <summary>
        /// Create the Ultimate Inventory System inspector.
        /// </summary>
        /// <param name="container">The parent container.</param>
        protected abstract void CreateInspector(VisualElement container);
    }
}