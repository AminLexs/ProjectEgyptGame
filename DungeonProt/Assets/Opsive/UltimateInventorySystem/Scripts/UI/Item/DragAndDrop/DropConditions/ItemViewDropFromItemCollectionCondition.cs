namespace Opsive.UltimateInventorySystem.UI.Item.DragAndDrop.DropConditions
{
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using System;
    using UnityEngine;

    [Serializable]
    public class ItemViewDropFromItemCollectionCondition : ItemViewDropCondition
    {
        [SerializeField] protected ItemCollectionID m_SourceItemCollectionID;
        
        public override bool CanDrop(ItemViewDropHandler itemViewDropHandler)
        {
            return m_SourceItemCollectionID.Compare(itemViewDropHandler.SourceItemInfo.ItemCollection);
        }
        
        public override string ToString()
        {
            return base.ToString() + string.Format("[{0}]",m_SourceItemCollectionID);
        }
    }

}