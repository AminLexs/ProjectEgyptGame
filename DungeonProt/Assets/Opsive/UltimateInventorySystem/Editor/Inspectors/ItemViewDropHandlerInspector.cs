namespace Opsive.UltimateInventorySystem.Editor.Inspectors
{
    using Opsive.UltimateInventorySystem.Editor.VisualElements;
    using Opsive.UltimateInventorySystem.UI.DataContainers;
    using Opsive.UltimateInventorySystem.UI.Item.DragAndDrop;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine.UIElements;

    /// <summary>
    /// A custom inspector for the inventory grid component.
    /// </summary>
    [CustomEditor(typeof(ItemViewDropHandler), true)]
    public class ItemViewDropHandlerInspector : InspectorBase
    {
        protected override List<string> PropertiesToExclude => new List<string>() { "m_ItemViewSlotDropActionSet" };

        protected ItemViewDropHandler m_ItemViewDropHandler;
        protected VisualElement m_InnerContainer;
        protected ObjectFieldWithNestedInspector<ItemViewSlotDropActionSet, ItemViewSlotDropActionSetInspector>
            m_CategoryItemActionSet;

        /// <summary>
        /// Initialize the inspector when it is first selected.
        /// </summary>
        protected override void InitializeInspector()
        {
            m_ItemViewDropHandler = target as ItemViewDropHandler;

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
                <ItemViewSlotDropActionSet, ItemViewSlotDropActionSetInspector>(
                    "Item View Slot Drop Action Set",
                    m_ItemViewDropHandler.m_ItemViewSlotDropActionSet,
                    "The item view drop action set. Specifies the actions that should be performed when dropping an item in a field.",
                    (newValue) =>
                    {
                        m_ItemViewDropHandler.m_ItemViewSlotDropActionSet = newValue;
                        Shared.Editor.Utility.InspectorUtility.SetDirty(m_ItemViewDropHandler);
                    });
            
            m_InnerContainer.Add(m_CategoryItemActionSet);
        }
    }
}