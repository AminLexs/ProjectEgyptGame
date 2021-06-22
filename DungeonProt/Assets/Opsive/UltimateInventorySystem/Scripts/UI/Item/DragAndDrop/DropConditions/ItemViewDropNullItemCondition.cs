namespace Opsive.UltimateInventorySystem.UI.Item.DragAndDrop.DropConditions
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ItemViewDropNullItemCondition : ItemViewDropCondition
    {
        [Tooltip("The source item is null.")]
        [SerializeField] protected bool m_Source = true;
        [Tooltip("The destination item is null.")]
        [SerializeField] protected bool m_Destination = true;
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ItemViewDropNullItemCondition()
        {
        }
        
        /// <summary>
        /// Constructor with overloads.
        /// </summary>
        public ItemViewDropNullItemCondition(bool source, bool destination)
        {
            m_Source = source;
            m_Destination = destination;
        }
        
        public override bool CanDrop(ItemViewDropHandler itemViewDropHandler)
        {
            var source = 
                !m_Source ||
                itemViewDropHandler.SourceItemInfo.Item == null;

            var destination = 
                !m_Destination ||
                itemViewDropHandler.DestinationItemInfo.Item == null;
            
            return source && destination;
        }

        public override string ToString()
        {
            return $"NullItem [{m_Source},{m_Destination}]";
        }
    }
}