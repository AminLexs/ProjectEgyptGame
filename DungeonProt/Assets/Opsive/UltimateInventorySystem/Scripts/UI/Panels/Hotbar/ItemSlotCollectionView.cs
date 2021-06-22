namespace Opsive.UltimateInventorySystem.UI.Panels.Hotbar
{
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.Equipping;
    using Opsive.UltimateInventorySystem.ItemActions;
    using Opsive.UltimateInventorySystem.Storage;
    using Opsive.UltimateInventorySystem.UI.Item;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewSlotRestrictions;
    using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
    using Opsive.UltimateInventorySystem.Utility;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using EventHandler = Opsive.Shared.Events.EventHandler;

    /// <summary>
    /// The component used to view an item slot collection, for example an equipment window.
    /// </summary>
    public class ItemSlotCollectionView : ItemViewSlotsContainer, IItemViewSlotsContainerController, IDatabaseSwitcher
    {
        [Tooltip("The inventory.")]
        [SerializeField] protected Inventory m_Inventory;
        [Tooltip("Update UI when inventory updates")]
        [SerializeField] protected bool m_RefreshOnInventoryUpdate = true;
        [Tooltip("Update UI when inventory updates")]
        [SerializeField] protected ItemCollectionID m_ItemCollectionID = new ItemCollectionID(null, ItemCollectionPurpose.Equipped);
        [Tooltip("Automatically set the item view slot category restriction to match the ItemSlot category")]
        [SerializeField] protected bool m_SetItemViewSlotRestrictions = true;
        [Tooltip("The slots that belong to the collection.")]
        [SerializeField] protected ItemSlotSet m_ItemSlotSet;
        [Tooltip("The item view slots that map to item slots by index.")]
        [SerializeField] internal ItemViewSlot[] m_ItemSlotItemViewSlots;

        protected bool m_IsRegisteredToInventoryUpdate = false;
        protected bool m_IsInventorySet;
        protected ItemSlotCollection m_ItemSlotCollection;

        public Inventory Inventory => m_Inventory;
        public ItemSlotCollection ItemSlotCollection => m_ItemSlotCollection;
        public ItemUser ItemUser => Inventory.ItemUser;
        ItemViewSlotsContainerBase IItemViewSlotsContainerController.ItemViewSlotsContainer => this;
        public ItemSlotSet ItemSlotSet
        {
            get { return m_ItemSlotSet; }
            internal set { m_ItemSlotSet = value; }
        }

        /// <summary>
        /// Set up the item hot bar slots
        /// </summary>
        public override void Initialize(bool force)
        {
            if(m_IsInitialized && !force){ return; }
            
            base.Initialize(force);
            
            if (m_ItemSlotSet == null || m_ItemSlotSet.ItemSlots == null) {
                Debug.LogError("The item slot set cannot be null",gameObject);
                return;
            }

            if (m_ItemSlotItemViewSlots == null) {
                m_ItemSlotItemViewSlots = new ItemViewSlot[m_ItemSlotSet.ItemSlots.Count];
            }else if (m_ItemSlotItemViewSlots.Length != m_ItemSlotSet.ItemSlots.Count) {
                Array.Resize(ref m_ItemSlotItemViewSlots, m_ItemSlotSet.ItemSlots.Count);
            }

            if (m_SetItemViewSlotRestrictions) { SetItemViewSlotRestrictions(); }

            if (m_Inventory != null) {
                SetInventory(m_Inventory);
            }
        }

        private void OnEnable()
        {
            Initialize(false);
            RegisterToInventoryUpdate();
        }

        private void OnDisable()
        {
            UnregisterFromInventoryUpdate();
        }

        public void SetItemViewSlotRestrictions()
        {
            if (m_ItemSlotSet == null) { return; }

            for (int i = 0; i < m_ItemSlotItemViewSlots.Length; i++) {
                var itemViewSlot = m_ItemSlotItemViewSlots[i];
                if (itemViewSlot == null) {
                    return;
                    //Debug.LogError($"The item view slot at index {i} is null, the item view slots cannot be null",gameObject);
                }
                
                var itemSlot = m_ItemSlotSet.GetSlot(i);
                if(itemSlot.HasValue == false){ return; }

                var itemViewSlotRestriction = itemViewSlot.GetComponent<ItemViewSlotCategoryRestriction>();
                if (itemViewSlotRestriction == null) {
                    itemViewSlotRestriction = itemViewSlot.gameObject.AddComponent<ItemViewSlotCategoryRestriction>();
                }

                itemViewSlotRestriction.ItemCategory = itemSlot.Value.Category;
            }
        }

        public ItemViewSlot GetItemViewSlot(ItemSlot itemSlot)
        {
            return GetItemViewSlot(itemSlot.Name);
        }
        
        public ItemViewSlot GetItemViewSlot(string slotName)
        {
            for (int i = 0; i < m_ItemSlotSet.ItemSlots.Count; i++) {
                if (slotName == m_ItemSlotSet.ItemSlots[i].Name) {
                    return m_ItemSlotItemViewSlots[i];
                }
            }

            return null;
        }
        
        public ItemSlot? GetItemSlot(ItemViewSlot itemViewSlot)
        {
            for (int i = 0; i < m_ItemSlotItemViewSlots.Length; i++) {
                if (itemViewSlot == m_ItemSlotItemViewSlots[i]) {
                    return m_ItemSlotSet.GetSlot(i);
                }
            }

            return null;
        }

        public void SetInventory(Inventory inventory)
        {
            if(m_Inventory == inventory && m_IsInventorySet){ return; }
            m_IsInventorySet = true;

            if (m_Inventory != null) {
                UnregisterFromInventoryUpdate();
            }

            m_Inventory = inventory;
            if (m_Inventory == null) {
                m_ItemSlotCollection = null;
                return;
            }

            m_ItemSlotCollection = m_Inventory.GetItemCollection(m_ItemCollectionID) as ItemSlotCollection;
            if (m_ItemSlotCollection == null) {
                Debug.LogError($"The inventory '{m_Inventory}' does not contain an item slot collection for the ID provided in the ItemSlotCollectionView", gameObject);
                m_Inventory = null;
                return;
            }

            if (m_ItemSlotSet != m_ItemSlotCollection.ItemSlotSet) {
                Debug.LogError($"The item slot collection '{m_ItemSlotCollection}' has an Item Slot Set '{m_ItemSlotCollection.ItemSlotSet}' " +
                               $"which does not match with the Item Slot Set '{m_ItemSlotSet}' provided in the ItemSlotCollectionView", gameObject);
                m_Inventory = null;
                m_ItemSlotCollection = null;
                return;
            }

            RegisterToInventoryUpdate();
            
            OnInventoryChange();
        }

        /// <summary>
        /// Update the hot bar when the inventory changes.
        /// </summary>
        protected void OnInventoryChange()
        {
            if(m_RefreshOnInventoryUpdate == false){ return; }
            
            Draw();
        }
        
        /// <summary>
        /// Register to the inventory update event.
        /// </summary>
        protected virtual void RegisterToInventoryUpdate()
        {
            if (m_IsRegisteredToInventoryUpdate || m_Inventory == null || gameObject.activeInHierarchy == false) { return; }
            Shared.Events.EventHandler.RegisterEvent(m_Inventory, EventNames.c_Inventory_OnUpdate, OnInventoryChange);

            m_IsRegisteredToInventoryUpdate = true;
        }

        /// <summary>
        /// Unregister from the inventory update event.
        /// </summary>
        protected virtual void UnregisterFromInventoryUpdate()
        {
            if (m_Inventory == null || !m_IsRegisteredToInventoryUpdate) { return; }
            Shared.Events.EventHandler.UnregisterEvent(m_Inventory, EventNames.c_Inventory_OnUpdate, OnInventoryChange);

            m_IsRegisteredToInventoryUpdate = false;
        }

    #region Item View Slot Container Overrides

        public override ItemInfo RemoveItem(ItemInfo itemInfo, int index)
        {
            return m_ItemSlotCollection.RemoveItem(itemInfo);
        }
        
        public override bool CanAddItem(ItemInfo itemInfo, int index)
        {
            var canAddBase = base.CanAddItem(itemInfo, index);
            if (canAddBase == false) { return false; }

            if (Inventory.AddCondition(itemInfo,m_ItemSlotCollection) == null) { return false; }

            return true;
        }

        public override ItemInfo AddItem(ItemInfo itemInfo, int index)
        {
            if (CanAddItem(itemInfo,index) == false) {
                return (0, null, null);
            }

            //var targetItemStack = m_ItemSlotCollection.GetItemStackAtSlot(index);
            
            var addedItem = m_ItemSlotCollection.AddItem(itemInfo, index);
            AssignItemToSlot(addedItem, index);
            return itemInfo;
        }
        
        public override void Draw()
        {
            if (m_Inventory == null || m_ItemSlotCollection == null) {
                Debug.LogWarning("The inventory or the item slot collection for the item slot collection view is null",gameObject);
                for (int i = 0; i < m_ItemViewSlots.Length; i++) {
                    AssignItemToSlot(ItemInfo.None, i);
                }
                return;
            }

            for (int i = 0; i < m_ItemViewSlots.Length; i++) {
                var itemInSlot = m_ItemSlotCollection.GetItemStackAtSlot(i);
                AssignItemToSlot(new ItemInfo(itemInSlot), i);
            }
            
            //base.Draw();
        }

    #endregion

        /// <summary>
        /// Check if the object contained by this component are part of the database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>True if all the objects in the component are part of that database.</returns>
        bool IDatabaseSwitcher.IsComponentValidForDatabase(InventorySystemDatabase database)
        {
            if (database == null) { return false; }

            return true;
            // return (m_ItemSlotSet as IDatabaseSwitcher)?.IsComponentValidForDatabase(database) ?? true;
        }

        /// <summary>
        /// Replace any object that is not in the database by an equivalent object in the specified database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>The objects that have been changed.</returns>
        ModifiedObjectWithDatabaseObjects IDatabaseSwitcher.ReplaceInventoryObjectsBySelectedDatabaseEquivalents(InventorySystemDatabase database)
        {
            if (database == null) { return null; }

            return null;
            /*(m_ItemSlotSet as IDatabaseSwitcher)?.ReplaceInventoryObjectsBySelectedDatabaseEquivalents(database);

            return new UnityEngine.Object[] { m_ItemSlotSet };*/
        }
    }
}