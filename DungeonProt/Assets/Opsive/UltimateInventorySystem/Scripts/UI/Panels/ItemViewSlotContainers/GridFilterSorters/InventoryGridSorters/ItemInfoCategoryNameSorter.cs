namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers.GridFilterSorters.InventoryGridSorters
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Grid;
    using System.Collections.Generic;

    public class ItemInfoCategoryNameSorter : ItemInfoSorterBase
    {
        protected Comparer<ItemInfo> m_ItemCategoryNameComparer = Comparer<ItemInfo>.Create((i1, i2) =>
        {
            var compare = i2.Item.Category.name.CompareTo(i1.Item.Category.name);

            if (compare != 0) { return compare; }

            compare = i2.Item.name.CompareTo(i1.Item.name);

            return compare;
        });
     
        public override Comparer<ItemInfo> Comparer => m_ItemCategoryNameComparer;
    }
}