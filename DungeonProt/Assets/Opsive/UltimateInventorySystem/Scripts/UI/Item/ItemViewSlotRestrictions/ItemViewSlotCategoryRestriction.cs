namespace Opsive.UltimateInventorySystem.UI.Item.ItemViewSlotRestrictions
{
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using UnityEngine;

    public class ItemViewSlotCategoryRestriction : ItemViewSlotRestriction
    {
        [Tooltip("The item category the the item should have")]
        [SerializeField] protected DynamicItemCategory m_ItemCategory;
        [Tooltip("Compare the category inherently or by exact match")]
        [SerializeField] protected bool m_Inherently = true;

        public ItemCategory ItemCategory
        {
            get => m_ItemCategory;
            set => m_ItemCategory = value;
        }

        public bool Inherently
        {
            get => m_Inherently;
            set => m_Inherently = value;
        }

        public override bool CanContain(ItemInfo itemInfo)
        {
            if (itemInfo.Item == null) { return true; }
            if (ItemCategory == null) { return true; }

            if (m_Inherently && ItemCategory.InherentlyContains(itemInfo.Item.Category)) { return true; }

            return m_ItemCategory == itemInfo.Item.Category;
        }
        
        public override string ToString()
        {
            var categoryName = ItemCategory != null ? ItemCategory.name : "NULL";
            return GetType().Name+ " With Category: "+categoryName;
        }
    }
}