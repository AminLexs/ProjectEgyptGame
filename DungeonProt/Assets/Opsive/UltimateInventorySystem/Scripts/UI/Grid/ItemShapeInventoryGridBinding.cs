using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using UnityEngine;

namespace Opsive.UltimateInventorySystem.UI.Grid
{
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Item;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
    using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
    using Opsive.UltimateInventorySystem.Utility;
    using System.Collections.Generic;
    using UnityEngine.Serialization;

    public class ItemShapeInventoryGridBinding : ItemViewSlotsContainerInventoryGridBinding
    {
        [FormerlySerializedAs("m_GridItemShapeDataID")] [SerializeField] protected int m_ItemShapeGridDataID;
        [SerializeField] protected ItemViewSlotCursorManager m_CursorManager;
    
        protected ItemShapeGridData m_InventoryItemShapesGridData;
        protected ItemShapeInventoryGridIndexer m_ItemShapeInventoryGridIndexer;
        protected InventoryGridIndexer m_PreviousIndexer;

        public override void Initialize(bool force)
        {
            base.Initialize(force);
            if (m_CursorManager == null) { m_CursorManager = GetComponentInParent<ItemViewSlotCursorManager>(); }
            m_ItemShapeInventoryGridIndexer = new ItemShapeInventoryGridIndexer();
        }

        protected override void OnBind()
        {
            //The Grid Data takes care of the index, not the Inventory Grid.
            m_InventoryGrid.UseGridIndex = true;
            m_PreviousIndexer = m_InventoryGrid.InventoryGridIndexer;
            m_InventoryGrid.InventoryGridIndexer = m_ItemShapeInventoryGridIndexer;
            
            m_InventoryGrid.Grid.OnAfterDraw += OnAfterDraw;
        
            m_InventoryGrid.OnBindInventory += BindInventory;
            m_InventoryGrid.OnUnBindInventory += UnBindInventory;
            if (m_InventoryGrid.Inventory != null) {
                BindInventory(m_InventoryGrid.Inventory);
            }

            var dragHandler = GetComponent<ItemViewSlotDragHandler>();
            if (dragHandler != null) {
                dragHandler.OnDragStarted += HandleItemViewSlotBeginDrag;
            }
        }

        private void BindInventory(Inventory inventory)
        {
            if (m_InventoryItemShapesGridData == null && inventory != null) {
                var gridsShapeController = inventory.GetComponent<ItemShapeInventoryGridController>();

                if (gridsShapeController != null) {
                    m_InventoryItemShapesGridData = gridsShapeController.GetGridDataWithID(m_ItemShapeGridDataID, m_InventoryGrid.TabID);
                } else {
                    Debug.LogWarning("The inventory is missing a InventoryItemShapesGridsController component.");
                }
            }

            if (m_InventoryItemShapesGridData == null) {
                Debug.LogError("THe Item Shape Grid data is null.");
                return;
            }

            Debug.Log("Bind inventory");
            m_ItemShapeInventoryGridIndexer.SetItemShapeGridData(m_InventoryItemShapesGridData);
            //m_InventoryGrid.Grid.BindGridFilterSorter(m_InventoryItemShapesGridData.FilterSorter);

            // Make sure the grid size is the same TODO resize the UI grid to match!?
            if (m_InventoryGrid.Grid.GridSize != m_InventoryItemShapesGridData.GridSize) {
                Debug.LogError("The grid size of the Inventory Grid (on the Grid gameobject) and the Inventory Grid Item Shape Data (on the Inventory gameobject) does not match!", gameObject);
            }
        }

        private void UnBindInventory(Inventory inventory)
        {
            //m_InventoryGrid.Grid.BindGridFilterSorter(null);
            m_InventoryItemShapesGridData = null;
        }

        protected override void OnUnBind()
        {
            m_InventoryGrid.InventoryGridIndexer = m_PreviousIndexer;
            m_InventoryGrid.OnBindInventory -= BindInventory;
            m_InventoryGrid.OnUnBindInventory -= UnBindInventory;
        }

        private void HandleItemViewSlotBeginDrag(ItemViewSlotPointerEventData eventdata)
        {
            if(m_InventoryItemShapesGridData == null){return;}

            if (m_InventoryItemShapesGridData.TryFindAnchorForItem(eventdata.ItemView.ItemInfo, out var anchorPos) == false) {
                return;
            }

            var itemSlotPos = m_InventoryItemShapesGridData.OneDTo2D(eventdata.Index);

            var modules = m_CursorManager.FloatingItemView.Modules;

            for (int i = 0; i < modules.Count; i++) {
                if (modules[i] is ItemShapeItemView itemShapeItemView) {
                    itemShapeItemView.DiscreteOffsetImage(anchorPos - itemSlotPos);
                }
            }
        
            //var offset = (anchorPos - itemSlotPos) m_InventoryItemShapesGridData.
        
            //m_ItemBoxCursorManager.PositionOffset = eventdata.ItemBoxObject.ItemInfo
        }

        /// <summary>
        /// Hide the images on after draw, the boxes need to reference the right object but only the anchor should show the item.
        /// </summary>
        private void OnAfterDraw()
        {
            if(m_InventoryItemShapesGridData == null){return;}
        
            for (int i = 0; i < m_InventoryItemShapesGridData.GridSizeCount; i++) {
            
                var itemView = m_InventoryGrid.GetItemViewAt(i);

                for (int j = 0; j < itemView.Modules.Count; j++) {
                    if (itemView.Modules[j] is ItemShapeItemView itemShapeModule) {
                        itemShapeModule.SetGridInfo(m_InventoryItemShapesGridData, i);
                    }
                }
            }
        }
    }

    public class ItemShapeInventoryGridIndexer : InventoryGridIndexer
    {
        private ItemShapeGridData m_GridData;
        private List<ItemInfo> m_ItemInfoList;
        private bool[,] m_OccupiedSpace;
        private Stack<ItemInfo> m_ItemsThatDoNotFit;

        public ItemShapeInventoryGridIndexer()
        {
            m_ItemInfoList = new List<ItemInfo>();
        }
        
        public void SetItemShapeGridData(ItemShapeGridData itemShapeGridData)
        {
            m_GridData = itemShapeGridData;
        }
        
        public override ListSlice<ItemInfo> GetOrderedItems(ListSlice<ItemInfo> itemInfos)
        {
            if (m_GridData == null) { return itemInfos;}
            m_ItemInfoList.Clear();

            // Add the items multiple times if they take multiple slots, the image will be hidden by the Binding component on after draw
            for (int row = 0; row < m_GridData.GridRows; row++) {
                for (int col = 0; col < m_GridData.GridColumns; col++) {

                    var element = m_GridData.GetElementAt(col, row);

                    if (element.IsEmpty) {
                        m_ItemInfoList.Add(ItemInfo.None);
                        continue;
                    }

                    var matchingItemInfo =
                        FindItemInfoMatchingStack(itemInfos, element.ItemStack);
                    if (matchingItemInfo == ItemInfo.None) {
                        Debug.LogWarning(
                            $"An grid is trying to sort an item which is not part of the shapes grid data: '{element.ItemStack}' ");
                        m_ItemInfoList.Add(ItemInfo.None);
                        continue;
                    }

                    m_ItemInfoList.Add(matchingItemInfo);
                }
            }

            return m_ItemInfoList;
        }
        
        private ItemInfo FindItemInfoMatchingStack(ListSlice<ItemInfo> itemInfos, ItemStack itemStack)
        {
            for (int i = 0; i < itemInfos.Count; i++) {
                if (itemInfos[i].ItemStack == itemStack) { return itemInfos[i]; }
            }

            return ItemInfo.None;
        }
        
        public override void SetStackIndex(ItemStack itemStack, int itemStackIndex)
        {
            m_GridData.TryAddItemToPosition((ItemInfo) itemStack, m_GridData.OneDTo2D(itemStackIndex));
        }

        public override bool CanMoveItem(int sourceIndex, int destinationIndex)
        {
            return m_GridData.CanMoveIndex(
                m_GridData.OneDTo2D(sourceIndex),
                m_GridData.OneDTo2D(destinationIndex));
        }

        public override void MoveItemStackIndex(int sourceItemIndex, int destinationItemIndex)
        {
            var result = m_GridData.TryMoveIndex(m_GridData.OneDTo2D(sourceItemIndex),
                m_GridData.OneDTo2D(destinationItemIndex));
        }
    }
}