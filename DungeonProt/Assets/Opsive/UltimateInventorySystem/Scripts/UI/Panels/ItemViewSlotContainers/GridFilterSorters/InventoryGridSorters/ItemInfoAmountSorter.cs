namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers.GridFilterSorters.InventoryGridSorters
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Grid;
    using System.Collections.Generic;

    public class ItemInfoAmountSorter : ItemInfoSorterBase
    {
        protected Comparer<ItemInfo> m_ItemNameComparer = Comparer<ItemInfo>.Create((i1, i2) =>
        {
            return i2.Amount.CompareTo(i1.Amount);
        });

        public override Comparer<ItemInfo> Comparer => m_ItemNameComparer;
    }
}