namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.ItemActions;
    using Opsive.UltimateInventorySystem.UI.DataContainers;
    using UnityEngine;

    public class ItemViewSlotsContainerCategoryItemActionSetBinding : ItemViewSlotsContainerItemActionBindingBase
    {
        [Tooltip("The categories item actions. Specifies the actions that can be performed on each item. Can be null.")]
        [SerializeField] public CategoryItemActionSet m_CategoryItemActionSet;
        [Tooltip("The maximum amount of actions displayed at one time to set the action array size.")]
        [SerializeField] protected int m_MaxNumberOfActions = 5;

        public override void Initialize(bool force)
        {
            if(m_IsInitialized && force == false){ return; }
            base.Initialize(force);
            
            m_ItemActions = new ItemAction[m_MaxNumberOfActions];
        }

        /// <summary>
        /// Use an item from the hot bar.
        /// </summary>
        /// <param name="itemSlotIndex">The item slot index of the item to use.</param>
        /// <param name="itemUser">The item user.</param>
        public override void UseAllItemActions(int itemSlotIndex)
        {
            if (CanItemUsAction(itemSlotIndex) == false) {
                return;
            }
            
            var itemInfo = m_ItemViewSlotsContainer.ItemViewSlots[itemSlotIndex].ItemInfo;
            var itemActions = m_CategoryItemActionSet.GetItemActionsForItem(itemInfo.Item, ref m_ItemActions);
            
            for (int i = 0; i < itemActions.Count; i++) {
                InvokeActionInternal(itemSlotIndex, i);
            }
        }
        
        /// <summary>
        /// Use an item from the hot bar.
        /// </summary>
        /// <param name="itemSlotIndex">The item slot index of the item to use.</param>
        /// <param name="itemActionIndex">The item action index.</param>
        public override void UseItemAction(int itemSlotIndex, int itemActionIndex)
        {
            if (CanItemUsAction(itemSlotIndex) == false) {
                return;
            }
            
            var itemInfo = m_ItemViewSlotsContainer.ItemViewSlots[itemSlotIndex].ItemInfo;

            var itemActions = m_CategoryItemActionSet.GetItemActionsForItem(itemInfo.Item, ref m_ItemActions);

            InvokeActionInternal(itemSlotIndex, itemActionIndex);
        }
        
        /// <summary>
        /// Open the item action panel.
        /// </summary>
        /// <param name="itemInfo">The item info selected.</param>
        /// <param name="index">The index.</param>
        protected override void OpenItemAction(ItemInfo itemInfo, int index)
        {
            if (CanOpenItemActionPanel(itemInfo)) {
                return;
            }
            
            if (m_ItemViewSlotsContainer.Panel == null) {
                Debug.LogWarning("Parent Panel is not set.");
            }

            var itemActions = m_CategoryItemActionSet.GetItemActionsForItem(itemInfo.Item, ref m_ItemActions);
            m_ActionPanel.AssignActions(itemActions, itemInfo, m_ItemUser, m_ItemViewSlotsContainer, index);
            
            m_ActionPanel.Open(m_ItemViewSlotsContainer.Panel, m_ItemViewSlotsContainer.GetItemViewSlot(index));
        }
        
        public override string ToString()
        {
            var actionsName = m_CategoryItemActionSet == null ? "NULL" : m_CategoryItemActionSet.name;
            return GetType().Name + ": "+ actionsName;
        }
    }
}