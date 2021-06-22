namespace Opsive.UltimateInventorySystem.UI.Item.DragAndDrop.DropActions
{
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using System;
    using UnityEngine;

    [Serializable]
    public class ItemViewDropInventoryGiveAction : ItemViewDropAction
    {
        [Tooltip("Add to the source container inventory")]
        [SerializeField] protected bool m_AddToDestinationInventory;
        [Tooltip("Remove from the source container inventory")]
        [SerializeField] protected bool m_RemoveFromSourceInventory;
        [Tooltip("The Item Collection were the item should be added.")]
        [SerializeField] protected ItemCollectionID m_DestinationItemCollectionID;

        
        public override void Drop(ItemViewDropHandler itemViewDropHandler)
        {
            var sourceInventory = (itemViewDropHandler.SourceContainer as IItemViewSlotsContainerController)?.Inventory ;

            if (sourceInventory == null) {
                sourceInventory = itemViewDropHandler.StreamData.SourceItemInfo.Inventory as Inventory;
            }
            
            var destinationInventory = (itemViewDropHandler.SourceContainer as IItemViewSlotsContainerController)?.Inventory ;

            if (m_RemoveFromSourceInventory && sourceInventory != null) {
                itemViewDropHandler.StreamData.SourceItemInfo = sourceInventory.RemoveItem(itemViewDropHandler.StreamData.SourceItemInfo);
            }
            
            if (m_AddToDestinationInventory && destinationInventory != null) {
                var destinationCollection = destinationInventory.GetItemCollection(m_DestinationItemCollectionID);
                if (destinationCollection == null) {
                    Debug.LogError(" The Destination Item Collection was not found.");
                    return;
                }
                var addedItem = destinationCollection.AddItem(itemViewDropHandler.StreamData.SourceItemInfo );
                var itemIndex = itemViewDropHandler.DestinationContainer.GetItemIndex(addedItem);

                if (itemIndex == -1) {
                    return;
                }
                itemViewDropHandler.DestinationContainer.MoveItem(itemIndex,itemViewDropHandler.DestinationIndex);
            }
        }
        
        public override string ToString()
        {
            var add = m_AddToDestinationInventory ? "+": "";
            var remove = m_RemoveFromSourceInventory ? "-": "";
            
            return base.ToString() + string.Format("[{0},{1}]", add, remove);
        }
    }
    
    [Serializable]
    public class ItemViewDropInventoryExchangeAction : ItemViewDropAction
    {
        [Tooltip("Remove from the source container inventory")]
        [SerializeField] protected bool m_RemoveFromSourceInventory;
        [Tooltip("Remove from the destination container inventory")]
        [SerializeField] protected bool m_RemoveFromDestinationInventory;
        [Tooltip("Add to the destination container inventory")]
        [SerializeField] protected bool m_AddToDestinationInventory;
        [Tooltip("Add to the source container inventory")]
        [SerializeField] protected bool m_AddToSourceInventory;
        [Tooltip("The Item Collection were the item should be added.")]
        [SerializeField] protected ItemCollectionID m_DestinationItemCollectionID;
        [Tooltip("The Item Collection were the item should be added.")]
        [SerializeField] protected ItemCollectionID m_SourceItemCollectionID;

        
        public override void Drop(ItemViewDropHandler itemViewDropHandler)
        {
            var sourceInventory = (itemViewDropHandler.SourceContainer as IItemViewSlotsContainerController)?.Inventory ;

            if (sourceInventory == null) {
                sourceInventory = itemViewDropHandler.StreamData.SourceItemInfo.Inventory as Inventory;
            }
            
            var destinationInventory = (itemViewDropHandler.SourceContainer as IItemViewSlotsContainerController)?.Inventory ;

            if (m_RemoveFromSourceInventory && sourceInventory != null) {
                itemViewDropHandler.StreamData.SourceItemInfo = sourceInventory.RemoveItem(itemViewDropHandler.StreamData.SourceItemInfo);
            }
            
            if (m_RemoveFromDestinationInventory && destinationInventory != null) {
                itemViewDropHandler.StreamData.DestinationItemInfo = destinationInventory.RemoveItem(itemViewDropHandler.StreamData.DestinationItemInfo);
            }
            
            if (m_AddToDestinationInventory && destinationInventory != null) {
                var destinationCollection = destinationInventory.GetItemCollection(m_DestinationItemCollectionID);
                if (destinationCollection == null) {
                    Debug.LogError(" The Destination Item Collection was not found.");
                    return;
                }
                var addedItem = destinationCollection.AddItem(itemViewDropHandler.StreamData.SourceItemInfo );
                var itemIndex = itemViewDropHandler.DestinationContainer.GetItemIndex(addedItem);

                if (itemIndex == -1) {
                    return;
                }
                itemViewDropHandler.DestinationContainer.MoveItem(itemIndex,itemViewDropHandler.DestinationIndex);
            }
            
            if (m_AddToSourceInventory && sourceInventory != null) {
                var sourceCollection = sourceInventory.GetItemCollection(m_SourceItemCollectionID);
                if (sourceCollection == null) {
                    Debug.LogError(" The Source Item Collection was not found.");
                    return;
                }
                var addedItem = sourceCollection.AddItem(itemViewDropHandler.StreamData.DestinationItemInfo );
                
                var itemIndex = itemViewDropHandler.SourceContainer.GetItemIndex(addedItem);

                if (itemIndex == -1) {
                    return;
                }
                itemViewDropHandler.SourceContainer.MoveItem(itemIndex,itemViewDropHandler.DestinationIndex);
            }
        }
        
        public override string ToString()
        {
            var remove = m_RemoveFromSourceInventory ? "s-": ".";
            var removeDestination = m_RemoveFromDestinationInventory ? "d-": ".";
            var add = m_AddToDestinationInventory ? "d+": ".";
            var addSource = m_AddToSourceInventory ? "s+": ".";
            
            return base.ToString() + string.Format("[{0},{1},{2},{3}]",remove,removeDestination,add,addSource);

        }
    }
}