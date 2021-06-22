namespace Opsive.UltimateInventorySystem.UI.Item
{
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.UI.Grid;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
    using Opsive.UltimateInventorySystem.UI.Panels;
    using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
    using Opsive.UltimateInventorySystem.Utility;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The hot item bar component allows you to use an item action for an item that was added to the hot bar.
    /// </summary>
    public class InventoryGrid : ItemViewSlotsContainerBase, IItemViewSlotsContainerController
    {
        public event Action<Inventory> OnBindInventory;
        public event Action<Inventory> OnUnBindInventory;
        
        [Tooltip("The inventory Grid.")]
        [SerializeField] protected ItemInfoGrid m_Grid;
        [Tooltip("The inventory to draw.")]
        [SerializeField] protected Inventory m_Inventory;
        [Tooltip("If true the inventory grid will keep items on the slot they were last set, if false there won't be empty spaces between items.")]
        [SerializeField] internal bool m_UseGridIndex = true;
        [Tooltip("Redraw the inventory grid each time the inventory updates.")]
        [SerializeField] protected bool m_DrawOnInventoryUpdate = true;
        [Tooltip("If true the inventory will draw when enabled (leave false when using panels).")]
        [SerializeField] protected bool m_DrawOnEnable = false;
        
        protected bool m_IsRegisteredToInventoryUpdate = false;
        protected InventoryGridIndexer m_InventoryGridIndexer;

        public ItemInfoGrid Grid => m_Grid;
        public InventoryGridIndexer InventoryGridIndexer
        {
            get { return m_InventoryGridIndexer; }
            set { m_InventoryGridIndexer = value; }
        }

        public Inventory Inventory => m_Inventory;

        public int GridID => Grid.GridID;
        public int TabID { get; set; }
        public IFilterSorter<ItemInfo> FilterSorter => m_Grid.FilterSorter;

        ItemViewSlotsContainerBase IItemViewSlotsContainerController.ItemViewSlotsContainer => this;

        public bool UseGridIndex
        {
            get => m_UseGridIndex;
            set => m_UseGridIndex = value;
        }

        /// <summary>
        /// Listen to the inventory update event.
        /// </summary>
        protected virtual void OnEnable()
        {
            Initialize(false);
            RegisterToInventoryUpdate();
            if (m_DrawOnEnable) {
                SetInventoryElementsAndDraw();
            }
        }

        /// <summary>
        /// Stop listening to the inventory update event.
        /// </summary>
        protected virtual void OnDisable()
        {
            UnregisterFromInventoryUpdate();
        }
        
        public override void Initialize(bool force)
        {
            if(m_IsInitialized && !force){ return; }
            
            if (m_Grid == null) {
                m_Grid = GetComponent<ItemInfoGrid>();
                if (m_Grid == null) {
                    Debug.LogError("Inventory grid field should not be null.",gameObject);
                    return;
                }
            }
            
            m_Grid.Initialize(force);
            
            var itemViewSlotAmount = m_Grid.ViewDrawer.BoxParents.Length;
            m_ItemViewSlots = new ItemViewSlot[itemViewSlotAmount];
            for (int i = 0; i < itemViewSlotAmount; i++) {
                var boxSlot = m_Grid.ViewDrawer.BoxParents[i] as ItemViewSlot;
                if (boxSlot == null) {
                    Debug.LogWarning("The item view slot container must use ItemViewSlots and not IViewSlots");
                }
                m_ItemViewSlots[i] = boxSlot;
            }
            
            if (m_InventoryGridIndexer == null) { m_InventoryGridIndexer = new InventoryGridIndexer(); }

            if (m_Inventory != null) { RegisterToInventoryUpdate(); }

            base.Initialize(force);
        }
        
    #region Item View Slot Container Overrides

        public override ItemInfo RemoveItem(ItemInfo itemInfo, int index)
        {
            return Inventory.RemoveItem(itemInfo);
        }
        
        public override bool CanAddItem(ItemInfo itemInfo, int index)
        {
            var canAddBase = base.CanAddItem(itemInfo, index);
            if (canAddBase == false) { return false; }

            if (Inventory.AddCondition(itemInfo,null) == null) { return false; }

            return true;
        }

        public override ItemInfo AddItem(ItemInfo itemInfo, int index)
        {
            if (CanAddItem(itemInfo,index) == false) {
                return (0, null, null);
            }

            var addedItem = Inventory.AddItem(itemInfo);
            m_InventoryGridIndexer.SetStackIndex(addedItem.ItemStack,index);
            return itemInfo;
        }

        public override bool CanMoveItem(int sourceIndex, int destinationIndex)
        {
            if (base.CanMoveItem(sourceIndex, destinationIndex) == false) { return false;}
            return m_InventoryGridIndexer.CanMoveItem(sourceIndex, destinationIndex);
        }

        public override void MoveItem(int sourceIndex, int destinationIndex)
        {
            if (CanMoveItem(sourceIndex, destinationIndex) == false) { return; }
            
            var sourceItemIndex = m_Grid.StartIndex + sourceIndex;
            var destinationItemIndex = m_Grid.StartIndex + destinationIndex;
            m_InventoryGridIndexer.MoveItemStackIndex(sourceItemIndex, destinationItemIndex);
        }

        public override void Draw()
        {
            if (m_Inventory == null) {
                m_Grid.SetElements((null, 0, 0));
                m_Grid.Draw();
                return;
            }

            var listSlice = new ListSlice<ItemInfo>(m_Inventory.AllItemInfos);
            var filterSorter = m_Grid.FilterSorter;

            if(filterSorter != null) {
                
                var pooledArray = GenericObjectPool.Get<ItemInfo[]>();
                listSlice = filterSorter.Filter(listSlice, ref pooledArray);
                if (m_UseGridIndex) {
                    listSlice = m_InventoryGridIndexer.GetOrderedItems(listSlice);
                }
                GenericObjectPool.Return(pooledArray);
            } else {
                if (m_UseGridIndex) {
                    listSlice = m_InventoryGridIndexer.GetOrderedItems(listSlice);
                }
            }

            m_Grid.SetElements(listSlice, true);
            
            m_Grid.Draw();
        }
        
        public override void SetDisplayPanel(DisplayPanel display)
        {
            base.SetDisplayPanel(display);
            if (m_Grid == null) {
                Debug.LogError("The Inventory Grid MUST have a Item Info Grid",gameObject);
                return;
            }
            m_Grid.SetParentPanel(display);
        }
        
        /// <summary>
        /// Get the Box prefab for the item specified.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        /// <returns>The box prefab game object.</returns>
        public override GameObject GetBoxPrefabFor(ItemInfo itemInfo)
        {
            return m_Grid.ViewDrawer.GetViewPrefabFor(itemInfo);
        }
    #endregion

        /// <summary>
        /// Set the inventory.
        /// </summary>
        /// <param name="inventory">The inventory.</param>
        public void SetInventory(Inventory inventory)
        {
            SetInventory(inventory, true);
        }
        
        /// <summary>
        /// Set the inventory.
        /// </summary>
        /// <param name="inventory">The inventory.</param>
        public void SetInventory(Inventory inventory, bool draw)
        {
            if (m_Inventory == inventory) { return; }

            UnregisterFromInventoryUpdate();

            m_Inventory = inventory;

            RegisterToInventoryUpdate();

            if (draw) {
                SetInventoryElementsAndDraw();
            }
        }

        /// <summary>
        /// Register to the inventory update event.
        /// </summary>
        protected virtual void RegisterToInventoryUpdate()
        {
            if (m_IsRegisteredToInventoryUpdate || m_Inventory == null || gameObject.activeInHierarchy == false) { return; }
            Shared.Events.EventHandler.RegisterEvent(m_Inventory, EventNames.c_Inventory_OnUpdate, InventoryUpdated);

            var inventoryGridIndexData = m_Inventory.GetComponent<InventoryGridIndexData>();
            if (inventoryGridIndexData != null) {
                m_InventoryGridIndexer.Copy(inventoryGridIndexData.GetGridIndexer(this));
                inventoryGridIndexData.SetGridIndexData(this);
            }
            
            m_IsRegisteredToInventoryUpdate = true;
            OnBindInventory?.Invoke(m_Inventory);
        }

        /// <summary>
        /// Unregister from the inventory update event.
        /// </summary>
        protected virtual void UnregisterFromInventoryUpdate()
        {
            if (m_Inventory == null || !m_IsRegisteredToInventoryUpdate) { return; }
            Shared.Events.EventHandler.UnregisterEvent(m_Inventory, EventNames.c_Inventory_OnUpdate, InventoryUpdated);

            var inventoryGridIndexData = m_Inventory.GetComponent<InventoryGridIndexData>();
            if (inventoryGridIndexData != null) {
                inventoryGridIndexData.SetGridIndexData(this);
            }

            m_IsRegisteredToInventoryUpdate = false;
            
            OnUnBindInventory?.Invoke(m_Inventory);
        }

        /// <summary>
        /// Draw the UI whenever the inventory changes.
        /// </summary>
        /// <param name="inventory">The inventory.</param>
        protected virtual void InventoryUpdated()
        {
            if (gameObject.activeInHierarchy == false || m_DrawOnInventoryUpdate == false) { return; }
            
            SetInventoryElementsAndDraw();
        }

        public void SetInventoryElementsAndDraw()
        {
            /*if (m_Inventory == null) {
                m_Grid.SetElements((null, 0, 0));
                Draw();
                return;
            }

            var listSlice = new ListSlice<ItemInfo>(m_Inventory.AllItemInfos);

            listSlice = m_InventoryGridIndexer.GetOrderedItems(listSlice);

            m_Grid.SetElements(listSlice);*/
            
            Draw();
        }
        
        public void SortItemIndexes(Comparer<ItemInfo> comparer)
        {
            m_InventoryGridIndexer.SortItemIndexes(comparer);
        }

        public IFilterSorter<ItemInfo> BindGridFilterSorter(IFilterSorter<ItemInfo> sorterFilter)
        {
            return Grid.BindGridFilterSorter(sorterFilter);
        }
        
    }
}