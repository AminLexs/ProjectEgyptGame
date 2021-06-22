namespace Opsive.UltimateInventorySystem.ItemActions
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI;
    using Opsive.UltimateInventorySystem.UI.Item;
    using Opsive.UltimateInventorySystem.UI.Panels;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Simple item action used to move an item from one slot to another within the same grid.
    /// </summary>
    [System.Serializable]
    public abstract class ItemViewSlotsContainerItemAction : ItemAction, IActionWithPanel
    {
        protected ItemViewSlotsContainerBase m_ItemViewSlotsContainer;
        protected int m_ItemViewSlotIndex;

        protected DisplayPanel m_OriginDisplayPanel;
        protected Selectable m_OriginSelectable;
        
        /// <summary>
        /// Check if the action can be invoked.
        /// </summary>
        /// <param name="itemInfo">The item.</param>
        /// <param name="itemUser">The item user (can be null).</param>
        /// <returns>True if the action can be invoked.</returns>
        protected override bool CanInvokeInternal(ItemInfo itemInfo, ItemUser itemUser)
        {
            return m_ItemViewSlotsContainer != null;
        }

        public virtual void SetViewSlotsContainer(ItemViewSlotsContainerBase itemViewSlotsContainer, int index)
        {
            m_ItemViewSlotsContainer = itemViewSlotsContainer;
            m_ItemViewSlotIndex = index;
        }
        
        public virtual void SetParentPanel(DisplayPanel parentDisplayPanel, Selectable previousSelectable, Transform parentTransform)
        {
            m_OriginDisplayPanel = parentDisplayPanel;
            m_OriginSelectable = previousSelectable;
        }
    }
}