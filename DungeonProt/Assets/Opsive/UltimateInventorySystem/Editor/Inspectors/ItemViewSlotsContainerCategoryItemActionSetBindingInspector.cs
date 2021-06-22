namespace Opsive.UltimateInventorySystem.Editor.Inspectors
{
    using Opsive.UltimateInventorySystem.Editor.VisualElements;
    using Opsive.UltimateInventorySystem.UI.DataContainers;
    using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine.UIElements;

    /// <summary>
    /// A custom inspector for the inventory grid component.
    /// </summary>
    [CustomEditor(typeof(ItemViewSlotsContainerCategoryItemActionSetBinding), true)]
    public class ItemViewSlotsContainerCategoryItemActionSetBindingInspector : InspectorBase
    {
        protected override List<string> PropertiesToExclude => new List<string>() { "m_CategoryItemActionSet" };

        protected ItemViewSlotsContainerCategoryItemActionSetBinding m_ItemViewSlotsContainerCategoryItemActionSetBinding;
        protected VisualElement m_InnerContainer;
        protected ObjectFieldWithNestedInspector<CategoryItemActionSet, CategoryItemActionSetInspector>
            m_CategoryItemActionSet;

        /// <summary>
        /// Initialize the inspector when it is first selected.
        /// </summary>
        protected override void InitializeInspector()
        {
            m_ItemViewSlotsContainerCategoryItemActionSetBinding = target as ItemViewSlotsContainerCategoryItemActionSetBinding;

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
                <CategoryItemActionSet, CategoryItemActionSetInspector>(
                    "Category Item Action Set",
                    m_ItemViewSlotsContainerCategoryItemActionSetBinding.m_CategoryItemActionSet,
                    "The categories item actions. Specifies the actions that can be performed on each item. Can be null.",
                    (newValue) =>
                    {
                        m_ItemViewSlotsContainerCategoryItemActionSetBinding.m_CategoryItemActionSet = newValue;
                        Shared.Editor.Utility.InspectorUtility.SetDirty(m_ItemViewSlotsContainerCategoryItemActionSetBinding);
                    });
            
            m_InnerContainer.Add(m_CategoryItemActionSet);
        }
    }
}