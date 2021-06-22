namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.ItemActions;
    using Opsive.UltimateInventorySystem.UI.Item;
    using Opsive.UltimateInventorySystem.UI.Panels.ActionPanels;
    using UnityEngine;

    public abstract class ItemViewSlotsContainerItemActionBindingBase : ItemViewSlotsContainerBinding
    {
        [Tooltip("The item user. Defaults to the Inventory Item User if null.")]
        [SerializeField] protected ItemUser m_ItemUser;
        [Tooltip("The action panel will open when clicking an item, displaying the actions that can be used on it. Can be null.")]
        [SerializeField] internal ItemActionPanel m_ActionPanel;
        [Tooltip("Use the item on click.")]
        [SerializeField] protected bool m_UseItemOnClick = true;
        [Tooltip("Which item action to use when triggered, -1 will use all item actions.")]
        [SerializeField] protected int m_UseItemActionIndex = -1;
        [Tooltip("Allow item actions to be called on empty slots.")]
        [SerializeField] protected bool m_DisableActionOnEmptySlots;
        
        protected ItemAction[] m_ItemActions;

        public ItemUser ItemUser => m_ItemUser;

        public override void Initialize(bool force)
        {
            if(m_IsInitialized && force == false){ return; }
            base.Initialize(force);

            if (m_ActionPanel != null) { m_ActionPanel.Close(); }
        }
        
        protected override void OnBind()
        {
            if (m_ItemUser == null && m_ItemViewSlotsContainer is IItemViewSlotsContainerController withInventory) {
                SetItemUser(withInventory.Inventory?.ItemUser);
            } else {
                SetItemUser(m_ItemUser);
            }
            
            m_ItemViewSlotsContainer.OnItemViewSlotClicked += HandleItemClicked;
            if (m_ActionPanel != null) {
                m_ActionPanel.OnAfterAnyItemActionInvoke += HandleItemActionInvoked;
            }
            
        }

        protected override void OnUnBind()
        {
            m_ItemViewSlotsContainer.OnItemViewSlotClicked -= HandleItemClicked;
            if (m_ActionPanel != null) {
                m_ActionPanel.OnAfterAnyItemActionInvoke -= HandleItemActionInvoked;
            }
        }

        /// <summary>
        /// Set the item user.
        /// </summary>
        /// <param name="itemUser">The item user.</param>
        public void SetItemUser(ItemUser itemUser)
        {
            m_ItemUser = itemUser;
        }

        private void HandleItemClicked(ItemViewSlotEventData eventdata)
        {
            if(m_UseItemOnClick == false){return;}

            TriggerItemAction();
        }

        private void HandleItemActionInvoked(int itemActionIndex)
        {
            m_ItemViewSlotsContainer.Draw();
        }

        public void TriggerItemAction()
        {
            TriggerItemAction(m_ItemViewSlotsContainer.GetSelectedSlot());
        }
        
        public void TriggerItemAction(int slotIndex)
        {
            var slotCount = m_ItemViewSlotsContainer.GetItemViewSlotCount();
            if (slotIndex < 0 && slotIndex >= slotCount) {
                Debug.LogWarning("The slot index you are trying to use is out of range "+slotIndex+" / "+slotCount);
                return;
            }
            
            TriggerItemAction(m_ItemViewSlotsContainer.ItemViewSlots[slotIndex]);
        }
        
        public void TriggerItemAction(ItemViewSlot itemViewSlot)
        {
            if (itemViewSlot == null) { return; }
            
            if (m_ActionPanel != null) {
                OpenItemAction(itemViewSlot.ItemInfo, itemViewSlot.Index);
                return;
            }

            if (m_UseItemActionIndex == -1) {
                UseAllItemActions(itemViewSlot.Index);
            } else {
                UseItemAction(itemViewSlot.Index, m_UseItemActionIndex);
            }
        }

        public virtual bool CanItemUsAction(int itemSlotIndex)
        {
            if (itemSlotIndex < 0 && itemSlotIndex >= m_ItemViewSlotsContainer.GetItemViewSlotCount()) { return false; }

            var itemInfo = m_ItemViewSlotsContainer.ItemViewSlots[itemSlotIndex].ItemInfo;
            
            if (m_DisableActionOnEmptySlots && (itemInfo.Item == null || itemInfo.Amount <= 0)) { return false; }

            return true;
        }
        
        /// <summary>
        /// Use an item from the hot bar.
        /// </summary>
        /// <param name="itemSlotIndex">The item slot index of the item to use.</param>
        /// <param name="itemUser">The item user.</param>
        public virtual void UseAllItemActions(int itemSlotIndex)
        {
            if (CanItemUsAction(itemSlotIndex) == false) {
                return;
            }

            for (int i = 0; i < m_ItemActions.Length; i++) {
                InvokeActionInternal(itemSlotIndex, i);
            }
        }

        /// <summary>
        /// Use an item from the hot bar.
        /// </summary>
        /// <param name="itemSlotIndex">The item slot index of the item to use.</param>
        /// <param name="itemActionIndex">The item action index.</param>
        public virtual void UseItemAction(int itemSlotIndex, int itemActionIndex)
        {
            if (CanItemUsAction(itemSlotIndex) == false) {
                return;
            }

            InvokeActionInternal(itemActionIndex, itemActionIndex);
        }

        /// <summary>
        /// Use an item from the hot bar.
        /// </summary>
        /// <param name="itemSlotIndex">The item slot index of the item to use.</param>
        /// <param name="itemActionIndex">The item action index.</param>
        protected virtual void InvokeActionInternal(int itemSlotIndex, int itemActionIndex)
        {
            var itemAction = m_ItemActions[itemActionIndex];
            var itemViewSlot = m_ItemViewSlotsContainer.ItemViewSlots[itemSlotIndex];
            var itemInfo = itemViewSlot.ItemInfo;
            
            itemAction.Initialize(false);
            
            if (itemAction is IActionWithPanel actionWithPanel) {
                actionWithPanel.SetParentPanel(m_ItemViewSlotsContainer.Panel,itemViewSlot, m_ItemViewSlotsContainer.transform);
            }
            
            if (itemAction is ItemViewSlotsContainerItemAction actionViewSlotsContainer) {
                actionViewSlotsContainer.SetViewSlotsContainer(m_ItemViewSlotsContainer, itemSlotIndex);
            }
            
            itemAction.InvokeAction(itemInfo, m_ItemUser);
        }
        
        /// <summary>
        /// Open the item action panel.
        /// </summary>
        /// <param name="itemInfo">The item info selected.</param>
        /// <param name="index">The index.</param>
        protected virtual void OpenItemAction(ItemInfo itemInfo, int index)
        {
            if (CanOpenItemActionPanel(itemInfo)) {
                return;
            }

            m_ActionPanel.AssignActions(m_ItemActions, itemInfo, m_ItemUser, m_ItemViewSlotsContainer, index);

            if (m_ItemViewSlotsContainer.Panel == null) {
                Debug.LogWarning("Parent Panel is not set.");
            }
            m_ActionPanel.Open(m_ItemViewSlotsContainer.Panel, m_ItemViewSlotsContainer.GetItemViewSlot(index));
        }

        protected virtual bool CanOpenItemActionPanel(ItemInfo itemInfo)
        {
            if (m_ActionPanel == null) { return true; }

            if (m_DisableActionOnEmptySlots && (itemInfo.Item == null || itemInfo.Amount <= 0)) { return true; }

            return false;
        }

        /// <summary>
        /// Close the item action panel.
        /// </summary>
        public virtual void CloseItemAction(bool selectPrevious)
        {
            if (m_ActionPanel == null) { return; }

            m_ActionPanel.Close(selectPrevious);
        }
    }
}