using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using UnityEngine;
using EventHandler = Opsive.Shared.Events.EventHandler;

namespace Opsive.UltimateInventorySystem.UI.Grid
{
    using System.Collections.Generic;
    using UnityEngine.Serialization;

    public class ItemShapeInventoryGridController : MonoBehaviour, IItemRestriction
    {
        [Tooltip("Add the item even if it does not match the filters of any grids.")]
        [SerializeField] protected bool m_NoGridAddItem;
        [FormerlySerializedAs("m_GridItemShapeHandlers")] [SerializeField] internal List<ItemShapeGridData> m_ItemShapeGridData;
        [SerializeField] protected string m_ShapeAttributeName = "Shape";

        protected Inventory m_Inventory;
    
        public Inventory Inventory => m_Inventory;
        public string ShapeAttributeName => m_ShapeAttributeName;


        public void Initialize(IInventory inventory, bool force)
        {
            m_Inventory = inventory as Inventory;
        
            for (int i = 0; i < m_ItemShapeGridData.Count; i++) {
                var gridShapeHandler = m_ItemShapeGridData[i];
            
                gridShapeHandler.Initialize(this);
            }
        
            EventHandler.RegisterEvent<ItemInfo,ItemStack>(m_Inventory,EventNames.c_Inventory_OnAdd_ItemInfo_ItemStack, OnItemAdded);
            EventHandler.RegisterEvent<ItemInfo>(m_Inventory,EventNames.c_Inventory_OnRemove_ItemInfo, OnItemRemoved);
        }

        public ItemShapeGridData GetGridDataWithID(int id, int tabID)
        {
            for (int i = 0; i < m_ItemShapeGridData.Count; i++) {
                if (m_ItemShapeGridData[i].ID == id && m_ItemShapeGridData[i].TabID == tabID) {
                    return m_ItemShapeGridData[i];
                }
            }

            return null;
        }

        private void OnItemRemoved(ItemInfo itemInfoRemoved)
        {
            var gridData = GetGridDataForItem((ItemInfo)itemInfoRemoved);

            if (gridData == null) { return; }

            gridData.OnItemRemoved(itemInfoRemoved);
        }

        private void OnItemAdded(ItemInfo originItemInfo, ItemStack itemInfoAdded)
        {
            var gridData = GetGridDataForItem((ItemInfo)itemInfoAdded);

            if (gridData == null) { return; }

            gridData.OnItemAdded(originItemInfo, itemInfoAdded);
            //gridData.PrintGridArray();
        }

        public ItemInfo? AddCondition(ItemInfo itemInfo, ItemCollection receivingCollection)
        {
            var itemInfoPreview = new ItemInfo(itemInfo.ItemAmount,receivingCollection,itemInfo.ItemStack);

            var gridData = GetGridDataForItem(itemInfoPreview);

            if (gridData == null) {
                if (m_NoGridAddItem) { return itemInfo; }
                return null;
            }


            if (gridData.TryFindAvailablePosition(itemInfoPreview, out var position)) {
                return itemInfo;
            }

            return null;
        }

        public ItemInfo? RemoveCondition(ItemInfo itemInfo)
        {
            return itemInfo;
        }

        public ItemShapeGridData GetGridDataForItem(ItemInfo itemInfo)
        {
            var index = -1;
            for (int i = 0; i < m_ItemShapeGridData.Count; i++) {
                if (m_ItemShapeGridData[i].CanContain(itemInfo)) {
                    if (index != -1) {
                        Debug.LogWarning($"The inventory has multiple grids which could show the same item '{itemInfo}', this can cause many types of issues.");
                    }
                    index = i;
                }
            }

            if (index == -1) {
            
            }

            return m_ItemShapeGridData[index];
        }
    }
}