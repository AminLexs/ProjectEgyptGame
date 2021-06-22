namespace Opsive.UltimateInventorySystem.ItemActions
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Item;
    using UnityEngine;

    /// <summary>
    /// Simple item action used to move an item from one slot to another within the same grid.
    /// </summary>
    [System.Serializable]
    public class MoveItemAction : ItemViewSlotsContainerItemAction
    {
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MoveItemAction()
        {
            m_Name = "Move";
        }

        /// <summary>
        /// Check if the action can be invoked.
        /// </summary>
        /// <param name="itemInfo">The item.</param>
        /// <param name="itemUser">The item user (can be null).</param>
        /// <returns>True if the action can be invoked.</returns>
        protected override bool CanInvokeInternal(ItemInfo itemInfo, ItemUser itemUser)
        {
            var canInvoke = base.CanInvokeInternal(itemInfo, itemUser);
            return canInvoke;
        }

        /// <summary>
        /// Invoke the action.
        /// </summary>
        /// <param name="itemInfo">The item.</param>
        /// <param name="itemUser">The item user (can be null).</param>
        protected override void InvokeActionInternal(ItemInfo itemInfo, ItemUser itemUser)
        {
            var moveCursor = m_ItemViewSlotsContainer.GetComponent<ItemViewSlotMoveCursor>();
            moveCursor.StartMove(m_ItemViewSlotIndex);
        }
    }
}