namespace Opsive.UltimateInventorySystem.UI.Item.DragAndDrop.DropConditions
{
    using Opsive.UltimateInventorySystem.UI.Panels.Hotbar;
    using System;
    using UnityEngine;

    [Serializable]
    public class ItemViewDropContainerHasNameCondition : ItemViewDropCondition
    {
        [SerializeField] protected string m_SourceContainerName;
        [SerializeField] protected string m_DestinationContainerName;
        
        public override bool CanDrop(ItemViewDropHandler itemViewDropHandler)
        {
            if (string.IsNullOrWhiteSpace(m_SourceContainerName) == false) {
                if (itemViewDropHandler.SourceContainer.ContainerName != m_SourceContainerName) { return false; }
            }

            if (string.IsNullOrWhiteSpace(m_DestinationContainerName) == false) {
                if (itemViewDropHandler.DestinationContainer.ContainerName != m_DestinationContainerName) { return false; }
            }
            
            return true;
        }

        public override string ToString()
        {
            var sourceName = string.IsNullOrWhiteSpace(m_SourceContainerName) ? "ANY" :
                string.Format("'{0}'",m_SourceContainerName);
            var destinationName = string.IsNullOrWhiteSpace(m_DestinationContainerName) ? "ANY" :
                string.Format("'{0}'",m_DestinationContainerName);
            
            return base.ToString() + string.Format("[{0},{1}]",sourceName, destinationName);
        }
    }
}