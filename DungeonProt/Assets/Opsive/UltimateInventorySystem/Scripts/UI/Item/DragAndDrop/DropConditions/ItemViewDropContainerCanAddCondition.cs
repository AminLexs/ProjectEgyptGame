namespace Opsive.UltimateInventorySystem.UI.Item.DragAndDrop.DropConditions
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ItemViewDropContainerCanAddCondition : ItemViewDropCondition
    {
        [Tooltip("Check if the source can accept an exchange.")]
        [SerializeField] protected bool m_Source = true;
        [Tooltip("Check if the destination can accept an exchange.")]
        [SerializeField] protected bool m_Destination = true;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ItemViewDropContainerCanAddCondition()
        {
        }
        
        /// <summary>
        /// Constructor with overloads.
        /// </summary>
        public ItemViewDropContainerCanAddCondition(bool source, bool destination)
        {
            m_Source = source;
            m_Destination = destination;
        }

        public override bool CanDrop(ItemViewDropHandler itemViewDropHandler)
        {
            var source = 
                !m_Source ||
                    itemViewDropHandler.SourceContainer.CanAddItem(
                        itemViewDropHandler.StreamData.DestinationItemInfo,
                        itemViewDropHandler.SourceIndex);
            
            var destination = 
                !m_Destination ||
                    itemViewDropHandler.DestinationContainer.CanAddItem(
                        itemViewDropHandler.StreamData.SourceItemInfo,
                        itemViewDropHandler.DestinationIndex);
            
            return source && destination;
        }

        public override string ToString()
        {
            return $"Container Can Add [{m_Source},{m_Destination}]";
        }
    }
}