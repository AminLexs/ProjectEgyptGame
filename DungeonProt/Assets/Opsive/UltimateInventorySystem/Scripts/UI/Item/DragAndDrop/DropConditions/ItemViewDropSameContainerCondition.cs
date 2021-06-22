namespace Opsive.UltimateInventorySystem.UI.Item.DragAndDrop.DropConditions
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ItemViewDropSameContainerCondition : ItemViewDropCondition
    {
        [SerializeField] protected bool m_Same;
        
        public override bool CanDrop(ItemViewDropHandler itemViewDropHandler)
        {
            var same = itemViewDropHandler.SourceContainer == itemViewDropHandler.DestinationContainer;
            return same == m_Same;
        }

        public override string ToString()
        {
            return $"Same Container [{m_Same}]";
        }
    }
}