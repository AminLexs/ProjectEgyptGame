/// ---------------------------------------------
/// Ultimate Inventory System.
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers.GridFilterSorters
{
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Grid;
    using System.Collections.Generic;
    using UnityEngine;

    public class ItemInfoMultiFilterSorter : ItemInfoFilterSorterBase
    {
        [SerializeField] internal List<ItemInfoFilterSorterBase> m_GridFilters;

        public List<ItemInfoFilterSorterBase> GridFilters => m_GridFilters;
        
        public override ListSlice<ItemInfo> Filter(ListSlice<ItemInfo> input, ref ItemInfo[] outputPooledArray)
        {
            var list = input;
            for (int i = 0; i < m_GridFilters.Count; i++) {
                list = m_GridFilters[i].Filter(list, ref outputPooledArray);
            }

            return list;
        }

        public override bool CanContain(ItemInfo input)
        {
            for (int i = 0; i < m_GridFilters.Count; i++) {
                if (m_GridFilters[i].CanContain(input)) { continue; }

                return false;
            }

            return true;
        }
    }
}