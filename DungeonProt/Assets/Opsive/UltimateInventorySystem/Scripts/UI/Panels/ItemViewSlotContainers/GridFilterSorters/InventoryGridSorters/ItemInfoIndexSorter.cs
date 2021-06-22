namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers.GridFilterSorters.InventoryGridSorters
{
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Grid;
    using Opsive.UltimateInventorySystem.Utility;
    using System.Collections.Generic;
    using UnityEngine;

    //TODO instead of this maybe replace the the indexer in the grid using a setter? Not sure if that's a better idea?
    
    
    public class ItemInfoIndexSorter : ItemInfoSorterBase
    {
        [SerializeField] protected bool m_KeepIndexEvenIfRemoved = false;
 
        protected List<ItemInfo> m_CachedItemInfos;
        protected Queue<ItemInfo> m_TempUnsetItemInfos;
        
        protected Comparer<ItemInfo> m_Comparer;
        public override Comparer<ItemInfo> Comparer => m_Comparer;
 
        protected override void Awake()
        {
            base.Awake();
            m_Comparer = Comparer<ItemInfo>.Create((i1, i2) =>
            {
               return 0;
            });
        }

        public ListSlice<ItemInfo> GetOrderedItems(ListSlice<ItemInfo> itemInfos, InventoryGridIndexer indexer)
        {
            var indexedItems = indexer.IndexedItems;

            m_CachedItemInfos.Clear();
            
            for (int i = 0; i < itemInfos.Count; i++) {
                var itemInfo = itemInfos[i];

                var stackIndex = -1;
                
                var itemIsNotIndexed =
                    itemInfo.ItemStack == null
                    || !indexedItems.TryGetValue(itemInfo.ItemStack, out stackIndex)
                    || stackIndex < 0;

                if (itemIsNotIndexed) {
                    m_TempUnsetItemInfos.Enqueue(itemInfo);
                    continue;
                }

                if (stackIndex >= m_CachedItemInfos.Count) {
                    m_CachedItemInfos.EnsureSize(stackIndex+1);
                }

                if (m_CachedItemInfos[stackIndex].ItemStack == null) {
                    m_CachedItemInfos[stackIndex] = itemInfo;
                } else {
                    m_TempUnsetItemInfos.Enqueue(itemInfo);
                }
            }

            var count = 0;
            while (m_TempUnsetItemInfos.Count > 0)
            {
                var itemInfo = m_TempUnsetItemInfos.Dequeue();

                var indexIsSet = false;
                for (int i = count; i < m_CachedItemInfos.Count; i++) {

                    count++;
                    if (m_CachedItemInfos[i].ItemStack != null) { continue; }

                    m_CachedItemInfos[i] = itemInfo;
                    indexedItems[itemInfo.ItemStack] = i;
                    indexIsSet = true;
                    break;
                }
                
                if(indexIsSet){ continue; }

                m_CachedItemInfos.Add(itemInfo);
                indexedItems[itemInfo.ItemStack] = count;
                count++;
            }

            return m_CachedItemInfos;
        }
    }
}