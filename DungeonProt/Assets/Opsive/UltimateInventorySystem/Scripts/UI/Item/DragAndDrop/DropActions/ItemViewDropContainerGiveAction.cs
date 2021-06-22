namespace Opsive.UltimateInventorySystem.UI.Item.DragAndDrop.DropActions
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ItemViewDropContainerGiveAction : ItemViewDropAction
    {
        [Tooltip("Only used if the ItemBoxDropActionsWithConditions is null")]
        [SerializeField] protected bool m_AddToDestinationContainer  = true;
        [Tooltip("Only used if the ItemBoxDropActionsWithConditions is null")]
        [SerializeField] protected bool m_RemoveFromSourceContainer  = true;
        
        public override void Drop(ItemViewDropHandler itemViewDropHandler)
        {
            if (m_RemoveFromSourceContainer) {
                itemViewDropHandler.StreamData.SourceItemInfo = itemViewDropHandler.SourceContainer.RemoveItem(itemViewDropHandler.StreamData.SourceItemInfo, itemViewDropHandler.SourceIndex);
            }
            
            if (m_AddToDestinationContainer) {
                itemViewDropHandler.DestinationContainer.AddItem(itemViewDropHandler.StreamData.SourceItemInfo, itemViewDropHandler.DestinationIndex);
            }
        }
        
        public override string ToString()
        {
            var add = m_AddToDestinationContainer ? "+": "";
            var remove = m_RemoveFromSourceContainer ? "-": "";

            return base.ToString() + string.Format("[{0},{1}]", add, remove);
        }
    }
    
    [Serializable]
    public class ItemViewDropContainerExchangeAction : ItemViewDropAction
    {
        [Tooltip("Only used if the ItemBoxDropActionsWithConditions is null")]
        [SerializeField] protected bool m_RemoveFromSourceContainer = true;
        [Tooltip("Only used if the ItemBoxDropActionsWithConditions is null")]
        [SerializeField] protected bool m_RemoveFromDestinationContainer = true;
        [Tooltip("Only used if the ItemBoxDropActionsWithConditions is null")]
        [SerializeField] protected bool m_AddToDestinationContainer = true;
        [Tooltip("Only used if the ItemBoxDropActionsWithConditions is null")]
        [SerializeField] protected bool m_AddToSourceContainer = true;
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ItemViewDropContainerExchangeAction()
        {
        }
        
        /// <summary>
        /// Constructor with overloads.
        /// </summary>
        public ItemViewDropContainerExchangeAction(bool removeFromSource, bool removeFromDestination, bool addToDestination, bool addToSource)
        {
            m_RemoveFromSourceContainer = removeFromSource;
            m_RemoveFromDestinationContainer = removeFromDestination;
            m_AddToDestinationContainer = addToDestination;
            m_AddToSourceContainer = addToSource;
        }
        
        public override void Drop(ItemViewDropHandler itemViewDropHandler)
        {
            if (m_RemoveFromSourceContainer) {
                itemViewDropHandler.StreamData.SourceItemInfo = itemViewDropHandler.SourceContainer.RemoveItem(itemViewDropHandler.StreamData.SourceItemInfo, itemViewDropHandler.SourceIndex);
            }

            if (m_RemoveFromDestinationContainer) {
                itemViewDropHandler.StreamData.DestinationItemInfo = itemViewDropHandler.DestinationContainer.RemoveItem(itemViewDropHandler.StreamData.DestinationItemInfo, itemViewDropHandler.DestinationIndex);
            }

            if (m_AddToDestinationContainer) {
                itemViewDropHandler.DestinationContainer.AddItem(itemViewDropHandler.StreamData.SourceItemInfo, itemViewDropHandler.DestinationIndex);
            }

            if (m_AddToSourceContainer) {
                itemViewDropHandler.SourceContainer.AddItem(itemViewDropHandler.StreamData.DestinationItemInfo, itemViewDropHandler.SourceIndex);
            }
        }
        
        public override string ToString()
        {
            var remove = m_RemoveFromSourceContainer ? "s-": ".";
            var removeDestination = m_RemoveFromDestinationContainer ? "d-": ".";
            var add = m_AddToDestinationContainer ? "d+": ".";
            var addSource = m_AddToSourceContainer ? "s+": ".";
            
            return base.ToString() + string.Format("[{0},{1},{2},{3}]",remove,removeDestination,add,addSource);
        }
    }
}