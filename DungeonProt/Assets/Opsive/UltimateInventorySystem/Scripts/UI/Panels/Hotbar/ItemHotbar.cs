/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Panels.Hotbar
{
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.ItemActions;
    using Opsive.UltimateInventorySystem.UI.Item;
    using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
    using Opsive.UltimateInventorySystem.Utility;
    using UnityEngine;
    using UnityEngine.Serialization;
    using EventHandler = Opsive.Shared.Events.EventHandler;

    /// <summary>
    /// The hot item bar component allows you to use an item action for an item that was added to the hot bar.
    /// </summary>
    public class ItemHotbar : MonoBehaviour, IItemViewSlotsContainerController
    {
        [Tooltip("Use the item assigned to this slot when clicked.")]
        [SerializeField] protected ItemViewSlotsContainer m_ItemViewSlotsContainer;
        [Tooltip("The inventory.")]
        [SerializeField] protected Inventory m_Inventory;
        [Tooltip("Use the item assigned to this slot when clicked.")]
        [SerializeField] protected ItemViewSlotsContainerItemActionBindingBase m_ItemActionsBinding;
        [Tooltip("Update UI when inventory updates")]
        [SerializeField] protected bool m_RefreshOnInventoryUpdate = true;

        protected bool m_IsInventorySet;
        
        public int SlotCount => m_ItemViewSlotsContainer.GetItemViewSlotCount();
        
        public Inventory Inventory => m_Inventory;
        public ItemUser ItemUser => m_ItemActionsBinding.ItemUser;
        public ItemViewSlotsContainer ItemViewSlotsContainer => m_ItemViewSlotsContainer;
        ItemViewSlotsContainerBase IItemViewSlotsContainerController.ItemViewSlotsContainer => m_ItemViewSlotsContainer;

        /// <summary>
        /// Set up the item hot bar slots
        /// </summary>
        private void Awake()
        {
            if (m_ItemViewSlotsContainer == null) {
                m_ItemViewSlotsContainer = GetComponent<ItemViewSlotsContainer>();
                if (m_ItemViewSlotsContainer == null) {
                    Debug.LogError($"The item hotbar '{name}' is missing an item view slots container",gameObject);
                    return;
                }
            }

            if (m_ItemActionsBinding == null) {
                m_ItemActionsBinding = GetComponent<ItemViewSlotsContainerItemActionBindingBase>();
                if (m_ItemActionsBinding == null) {
                    Debug.LogError($"The item hotbar '{name}' is missing an item view slots container item action binding",gameObject);
                    return;
                }
            }

            if (m_Inventory != null) {
                SetInventory(m_Inventory);
            }
        }

        public void SetInventory(Inventory inventory)
        {
            if(m_Inventory == inventory && m_IsInventorySet){ return; }
            m_IsInventorySet = true;

            if (m_Inventory != null) {
                EventHandler.UnregisterEvent<int>(m_Inventory.gameObject, EventNames.c_GameObject_OnInput_HotbarUseItem_Int, UseItem);
                EventHandler.UnregisterEvent(m_Inventory, EventNames.c_Inventory_OnUpdate, OnInventoryChange);
            }

            m_Inventory = inventory;
            if (m_Inventory == null) { return; }
            
            m_ItemActionsBinding.SetItemUser(m_Inventory.ItemUser);

            EventHandler.RegisterEvent<int>(m_Inventory.gameObject, EventNames.c_GameObject_OnInput_HotbarUseItem_Int, UseItem);
            EventHandler.RegisterEvent(m_Inventory, EventNames.c_Inventory_OnUpdate, OnInventoryChange);
            
            OnInventoryChange();
        }
        
        /// <summary>
        /// Stop listening. 
        /// </summary>
        protected virtual void OnEnable()
        {
            if (m_Inventory == null) { return; }
            
            EventHandler.RegisterEvent(m_Inventory, EventNames.c_Inventory_OnUpdate, OnInventoryChange);
        }

        /// <summary>
        /// Assign an item to a slot.
        /// </summary>
        /// <param name="itemInfo">The item.</param>
        /// <param name="slot">The item slot.</param>
        public virtual void AssignItemToSlot(ItemInfo itemInfo, int slot)
        {
            m_ItemViewSlotsContainer.AddItem(itemInfo, slot);
        }

        /// <summary>
        /// Update the hot bar when the inventory changes.
        /// </summary>
        protected void OnInventoryChange()
        {
            if(m_RefreshOnInventoryUpdate == false){ return; }

            var slots = ItemViewSlotsContainer.ItemViewSlots;
            for (int i = 0; i < slots.Count; i++) {
                var itemInfo = slots[i].ItemInfo;

                if (itemInfo.Item == null) { continue; }

                var amount = itemInfo.ItemCollection?.GetItemAmount(itemInfo.Item) ?? 0;

                ItemInfo newItemInfo;
                if (amount == 0) {
                    var result = itemInfo.Inventory?.GetItemInfo(itemInfo.Item);

                    if (result.HasValue) { newItemInfo = result.Value; } else {
                        newItemInfo = (0, itemInfo);
                    }
                } else { newItemInfo = (itemInfo.Item, amount, itemInfo.ItemCollection); }

                slots[i].SetItemInfo(newItemInfo);
            }
        }

        /// <summary>
        /// Use an item from the hot bar.
        /// </summary>
        /// <param name="itemSlotIndex">The item slot index of the item to use.</param>
        /// <param name="itemUser">The item user.</param>
        public virtual void UseItem(int itemSlotIndex)
        {
            m_ItemActionsBinding.TriggerItemAction(itemSlotIndex);
        }

        /// <summary>
        /// Stop listening. 
        /// </summary>
        protected virtual void OnDisable()
        {
            EventHandler.UnregisterEvent(m_Inventory, EventNames.c_Inventory_OnUpdate, OnInventoryChange);
        }
    }
}
