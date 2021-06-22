namespace Opsive.UltimateInventorySystem.Editor.Inspectors
{
    using Opsive.UltimateInventorySystem.Editor.VisualElements;
    using Opsive.UltimateInventorySystem.ItemActions;
    using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine.UIElements;

    /// <summary>
    /// A custom inspector for the inventory grid component.
    /// </summary>
    [CustomEditor(typeof(ItemViewSlotsContainerItemActionBinding), true)]
    public class ItemViewSlotsContainerItemActionBindingInspector : InspectorBase
    {
        protected override List<string> PropertiesToExclude => new List<string>() { "m_CategoryItemActions" };

        protected ItemViewSlotsContainerItemActionBinding m_ItemViewSlotsContainerItemActionBinding;
        protected VisualElement m_InnerContainer;
        protected ObjectFieldWithNestedInspector<ItemActionSet, CategoryItemActionsInspector>
            m_CategoryItemActionSet;

        /// <summary>
        /// Initialize the inspector when it is first selected.
        /// </summary>
        protected override void InitializeInspector()
        {
            m_ItemViewSlotsContainerItemActionBinding = target as ItemViewSlotsContainerItemActionBinding;

            base.InitializeInspector();
        }

        /// <summary>
        /// Create the inspector.
        /// </summary>
        /// <param name="container">The parent container.</param>
        protected override void CreateInspector(VisualElement container)
        {
            m_InnerContainer = new VisualElement();
            container.Add(m_InnerContainer);

            Refresh();
        }

        /// <summary>
        /// Draw the fields again when the database changes.
        /// </summary>
        protected void Refresh()
        {
            m_InnerContainer.Clear();

            m_CategoryItemActionSet = new ObjectFieldWithNestedInspector
                <ItemActionSet, CategoryItemActionsInspector>(
                    "Category Item Actions",
                    m_ItemViewSlotsContainerItemActionBinding.m_ItemActionSet,
                    "The categories item actions. Specifies the actions that can be performed on each item. Can be null.",
                    (newValue) =>
                    {
                        m_ItemViewSlotsContainerItemActionBinding.m_ItemActionSet = newValue;
                        Shared.Editor.Utility.InspectorUtility.SetDirty(m_ItemViewSlotsContainerItemActionBinding);
                    });
            
            m_InnerContainer.Add(m_CategoryItemActionSet);
        }
    }
}