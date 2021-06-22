namespace Opsive.UltimateInventorySystem.UI.Grid
{
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.UI.Item;
    using Opsive.UltimateInventorySystem.Utility;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Serialization;

    public class ItemShapeGridData : MonoBehaviour
    {
        public struct GridElementData
        {
            public static GridElementData None => new GridElementData();
        
            public ItemStack ItemStack { get; private set; }
            public bool IsAnchor { get; private set; }
            public bool IsEmpty => ItemStack == null;
            public bool IsOccupied => ItemStack != null;
        
            public GridElementData(ItemStack itemStack, bool isAnchor)
            {
                //Debug.Log(itemStack);
                ItemStack = itemStack;
                IsAnchor = isAnchor;
            
                //Debug.Log(IsOccupied);
            }

            public override string ToString()
            {
                return $"Grid ElementData : [Is Anchor '{IsAnchor}' -> {ItemStack}]";
            }
        }

        [SerializeField] protected int m_ID;
        [SerializeField] protected int m_TabID;
        [SerializeField] internal ItemInfoFilterSorterBase m_ItemInfoFilter;
        [SerializeField] internal Vector2Int m_GridSize;

        public int GridSizeCount => m_GridSize.x*m_GridSize.y;
        public int GridColumns => m_GridSize.x;
        public int GridRows => m_GridSize.y;
        protected string ShapeAttributeName => m_Controller.ShapeAttributeName;

        protected ItemShapeInventoryGridController m_Controller;
    
        protected GridElementData[,] m_ItemStackAnchorGrid;
        protected GridElementData[,] m_TemporaryItemStackAnchorGrid;

        public Inventory Inventory => m_Controller.Inventory;
        public int ID => m_ID;
        public int TabID => m_TabID;
        public Vector2Int GridSize => m_GridSize;
        public IFilterSorter<ItemInfo> FilterSorter => m_ItemInfoFilter;

        public Vector2Int OneDTo2D(int index) => new Vector2Int(index % m_GridSize.x, index / m_GridSize.x);
        public int TwoDTo1D(Vector2Int pos) => pos.y * m_GridSize.x + pos.x;

        public void Initialize(ItemShapeInventoryGridController controller)
        {
            if (m_Controller == controller) {
                return;
            }

            m_Controller = controller;

            m_ItemStackAnchorGrid = new GridElementData[GridColumns,GridRows];
            m_TemporaryItemStackAnchorGrid = new GridElementData[GridColumns,GridRows];
        }

        public GridElementData GetElementAt(Vector2Int pos)
        {
            return m_ItemStackAnchorGrid[pos.x, pos.y];
        }
    
        public GridElementData GetElementAt(int x, int y)
        {
            return m_ItemStackAnchorGrid[x, y];
        }
    
        public GridElementData GetElementAt(int index)
        {
            return GetElementAt(OneDTo2D(index));
        }

        public bool TryAddItemToPosition(ItemInfo info, Vector2Int position)
        {
            //Debug.Log($"Try Add: {position} + {info}");
            var x = position.x;
            var y = position.y;
        
        
            if (info.Item.TryGetAttributeValue<ItemShape>(ShapeAttributeName, out var shape) == false
                || shape.Count <= 1) {
            
                // Item takes a 1x1 shape.
                if (m_ItemStackAnchorGrid[x, y].IsOccupied) {
                    return false;
                }

                m_ItemStackAnchorGrid[x, y] = new GridElementData(info.ItemStack, true);
                //Debug.Log("Value 1x1 is now set: "+m_ItemStackAnchorGrid[x, y].IsOccupied);
                return true;
            }

            // Out of range
            if (x - shape.Anchor.x < 0 || x - shape.Anchor.x + shape.Cols > GridColumns) { return false; }
            if (y - shape.Anchor.y < 0 || y - shape.Anchor.y + shape.Rows > GridRows) { return false; }

            // Check if the item fits
            for (int row = 0; row < shape.Rows; row++) {
                for (int col = 0; col < shape.Cols; col++) {
                    if (m_ItemStackAnchorGrid[x - shape.Anchor.x + col, y - shape.Anchor.y + row].IsOccupied && shape.IsIndexOccupied(col,row)) {
                        return false;
                    }
                }
            }
        
            // Item fits, place it
            for (int row = 0; row < shape.Rows; row++) {
                for (int col = 0; col < shape.Cols; col++) {
                    if (shape.IsIndexOccupied(col, row)) {
                        m_ItemStackAnchorGrid[x - shape.Anchor.x + col, y - shape.Anchor.y + row] = new GridElementData(info.ItemStack,shape.Anchor.x==col && shape.Anchor.y == row);
                    }
                }
            }

            //Debug.Log("Value mxn is now set: "+m_ItemStackAnchorGrid[x, y].IsOccupied);
            return true;
        }

        public bool TryFindAvailablePosition(ItemInfo info, out Vector2Int position)
        {
            position = Vector2Int.zero;
            if (info.Item == null) { return false;}

            for (int y = 0; y < GridRows; y++) {
                for (int x = 0; x < GridColumns; x++) {
                    if (IsPositionAvailable(info, x, y)) {
                        position = new Vector2Int(x,y);
                        return true;
                    }
                }
            }

            position = new Vector2Int(-1,-1);
            return false;
        }

        public bool IsPositionAvailable(ItemInfo info, int x, int y, Func<Vector2Int,bool> canIgnore = null)
        {
            if (info.Item.TryGetAttributeValue<ItemShape>(ShapeAttributeName, out var shape) == false
                || shape.Count <= 1) {
            
                // Item takes a 1x1 shape.
                if (m_ItemStackAnchorGrid[x, y].IsOccupied) {

                    if ((canIgnore?.Invoke(new Vector2Int(x, y)) ?? false) == false) {
                        return false;
                    }
                }
                return true;
            }

            // Out of range
            if (x - shape.Anchor.x < 0 || x - shape.Anchor.x + shape.Cols > GridColumns) { return false; }
            if (y - shape.Anchor.y < 0 || y - shape.Anchor.y + shape.Rows > GridRows) { return false; }

            // Check if the item fits
            for (int row = 0; row < shape.Rows; row++) {
                for (int col = 0; col < shape.Cols; col++) {
                    if (!m_ItemStackAnchorGrid[x - shape.Anchor.x + col, y - shape.Anchor.y + row].IsOccupied ||
                        !shape.IsIndexOccupied(col, row)) { continue; }

                    if ((canIgnore?.Invoke(new Vector2Int(x, y)) ?? false) == false) {
                        return false;
                    }
                }
            }

            return true;
        }
    
        public bool IsPositionAvailable(ItemInfo info, int x, int y, ListSlice<ItemStack> itemStacksToIgnore)
        {
            if (info.Item.TryGetAttributeValue<ItemShape>(ShapeAttributeName, out var shape) == false
                || shape.Count <= 1) {
            
                // Item takes a 1x1 shape.
                if (m_ItemStackAnchorGrid[x, y].IsOccupied && itemStacksToIgnore.Contains(m_ItemStackAnchorGrid[x, y].ItemStack) == false) {
                    return false;
                }
                return true;
            }

            // Out of range
            if (x - shape.Anchor.x < 0 || x - shape.Anchor.x + shape.Cols > GridColumns) { return false; }
            if (y - shape.Anchor.y < 0 || y - shape.Anchor.y + shape.Rows > GridRows) { return false; }

            // Check if the item fits
            for (int row = 0; row < shape.Rows; row++) {
                for (int col = 0; col < shape.Cols; col++) {

                    var localX = x - shape.Anchor.x + col;
                    var localY = y - shape.Anchor.y + row;
                
                    if (m_ItemStackAnchorGrid[localX, localY].IsOccupied &&
                        shape.IsIndexOccupied(col, row) &&
                        itemStacksToIgnore.Contains(m_ItemStackAnchorGrid[localX, localY].ItemStack) == false) {
                    
                        return false;
                    
                    }
                }
            }

            return true;
        }

        public void OnItemAdded(ItemInfo originItemInfo, ItemStack itemStackAdded)
        {
            var itemInfoAdded = (ItemInfo) itemStackAdded;
            Vector2Int position = new Vector2Int(-1,-1);
        
            // First check if the item stack already exists in the grid
            if (TryFindAnchorForItem(itemInfoAdded, out position)) {
                // The item is already placed in the grid.
                return;   
            }

            if (TryFindAvailablePosition(itemInfoAdded, out position) == false) {
                Debug.LogWarning($"An Item '{itemStackAdded}' was added to the inventory but there is no place for it in the grid!");
                return;
            }

            //Place the item in the grid
            if (TryAddItemToPosition(itemInfoAdded, position) == false) {
                Debug.LogError("This should never happen, item could fit but was not added: "+itemInfoAdded);
                return;
            };
        
            //Debug.Log("Added without any issues");
        
        }

        public bool TryFindAnchorForItem(ItemInfo itemInfo, out Vector2Int position)
        {
            var itemStack = itemInfo.ItemStack;
            if (itemStack == null) {
                position = new Vector2Int(-1,-1);
                return false;
            }
        
            for (int y = 0; y < GridRows; y++) {
                for (int x = 0; x < GridColumns; x++) {
                    if (m_ItemStackAnchorGrid[x, y].IsAnchor && m_ItemStackAnchorGrid[x, y].ItemStack == itemStack) {
                        position = new Vector2Int(x,y);
                        return true;
                    }
                }
            }
        
            position = new Vector2Int(-1,-1);
            return false;
        }

        public void OnItemRemoved(ItemInfo itemInfoRemoved)
        {
            if (TryFindAnchorForItem(itemInfoRemoved, out var position) == false) {
                Debug.LogWarning($"The item '{itemInfoRemoved}' was removed from the inventory but not from the grid");
                return;
            }
        
            RemoveItemFromPosition(position);
        }

        public bool RemoveItemFromPosition(Vector2Int position)
        {
            var x = position.x;
            var y = position.y;

            if (m_ItemStackAnchorGrid[x, y].ItemStack == null) { return false; }

            if (m_ItemStackAnchorGrid[x, y].IsAnchor == false) {

                if (TryFindAnchorForItem((ItemInfo)m_ItemStackAnchorGrid[x, y].ItemStack, out var anchor) == false) { return false; }

                x = anchor.x;
                y = anchor.y;
            }

            var itemStack = m_ItemStackAnchorGrid[x, y].ItemStack;
        
            if (itemStack.Item.TryGetAttributeValue<ItemShape>(ShapeAttributeName, out var shape) == false
                || shape.Count <= 1) {
            
                // Item takes a 1x1 shape.
                m_ItemStackAnchorGrid[x, y] = GridElementData.None;
                return true;
            }
        
            // Remove item from each grid element from the shape.
            for (int row = 0; row < shape.Rows; row++) {
                for (int col = 0; col < shape.Cols; col++) {
                    if (shape.IsIndexOccupied(col, row)) {
                        m_ItemStackAnchorGrid[x - shape.Anchor.x + col, y - shape.Anchor.y + row] = GridElementData.None;
                    }
                }
            }

            return true;
        }

        public bool CanContain(ItemInfo itemInfoPreview)
        {
            if (m_ItemInfoFilter == null) { return true; }

            return m_ItemInfoFilter.CanContain(itemInfoPreview);
        }

        public void PrintGridArray()
        {
            var printMessage = "";
            for (int y = 0; y < m_ItemStackAnchorGrid.GetLength(1); y++) {
                printMessage += "| ";
                for (int x = 0; x < m_ItemStackAnchorGrid.GetLength(0); x++) {
                    printMessage += (m_ItemStackAnchorGrid[x,y].IsOccupied ? "x" : "o") +" | ";
                }
                printMessage += "\n";
            }
        
            Debug.Log(printMessage);
        }
    
        public bool CanMoveIndex(Vector2Int sourcePos, Vector2Int destinationPos)
        {
            // save current layout.
            Copy(m_ItemStackAnchorGrid, m_TemporaryItemStackAnchorGrid);

            var result = TryMoveIndex(sourcePos, destinationPos);
        
            //Return to previous state.
            Copy(m_TemporaryItemStackAnchorGrid, m_ItemStackAnchorGrid);

            return result;
        }

        public bool TryMoveIndex(Vector2Int sourcePos, Vector2Int destinationPos)
        {
            // Copy values to temporary slot.
            Copy(m_ItemStackAnchorGrid, m_TemporaryItemStackAnchorGrid);
        
            var sourceElement = GetElementAt(sourcePos);
            var destinationElement = GetElementAt(destinationPos);

            if (sourceElement.IsEmpty) { return false;}

            var oneWayMove = destinationElement.ItemStack == sourceElement.ItemStack || destinationElement.IsEmpty;

            TryFindAnchorForItem((ItemInfo) sourceElement.ItemStack, out var sourceAnchor);
            TryFindAnchorForItem((ItemInfo) destinationElement.ItemStack, out var destinationAnchor);

            var sourceOffset = sourceAnchor - sourcePos;
            var destinationOffset = destinationAnchor - destinationPos;

            var sourcePosWithOffset = sourcePos + destinationOffset;
            var destinationPosWithOffset = destinationPos + sourceOffset;

            // Remove source item.
            if (RemoveItemFromPosition(sourceAnchor) == false) {
                Debug.LogError("Nothing should be preventing it to be removed");
                //Return to previous state.
                Copy(m_TemporaryItemStackAnchorGrid, m_ItemStackAnchorGrid);
                return false;
            }
        
            if (oneWayMove == false) {
                if (RemoveItemFromPosition(destinationAnchor) == false) {
                    Debug.LogError("Nothing should be preventing it to be removed");
                    //Return to previous state.
                    Copy(m_TemporaryItemStackAnchorGrid, m_ItemStackAnchorGrid);
                    return false;
                }
            
                if (TryAddItemToPosition((ItemInfo) destinationElement.ItemStack, sourcePosWithOffset) == false) {
                    //Return to previous state.
                    Copy(m_TemporaryItemStackAnchorGrid, m_ItemStackAnchorGrid);
                    return false;
                }
            }

            if (TryAddItemToPosition((ItemInfo) sourceElement.ItemStack, destinationPosWithOffset) == false) {
                //Return to previous state.
                Copy(m_TemporaryItemStackAnchorGrid, m_ItemStackAnchorGrid);
                return false;
            }

            return true;
        }

        public Vector2Int GetAnchorOffset(ItemInfo itemInfo, Vector2Int pos)
        {
            if (TryFindAnchorForItem(itemInfo, out var anchor) == false) {
                return new Vector2Int(-1,-1);
            }
        
            return anchor - pos;
        }

        public void Copy(GridElementData[,] source, GridElementData[,] destination)
        {
            for (int row = 0; row < GridRows; row++) {
                for (int col = 0; col < GridColumns; col++) { destination[col, row] = source[col, row]; }
            }
        }
    }
}