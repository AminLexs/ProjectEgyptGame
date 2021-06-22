namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers
{
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Utility;
    using System.Collections.Generic;
    using UnityEngine;

    
    
    public class InventoryGridIndexer
    {
        protected List<ItemInfo> m_CachedItemInfos;
        protected Queue<ItemInfo> m_TempUnsetItemInfos;
        protected Dictionary<ItemStack, int> m_IndexedItems;

        public Dictionary<ItemStack, int> IndexedItems => m_IndexedItems;

        public InventoryGridIndexer()
        {
            m_CachedItemInfos = new List<ItemInfo>();
            m_TempUnsetItemInfos = new Queue<ItemInfo>();
            m_IndexedItems = new Dictionary<ItemStack, int>();
        }

        public virtual void Copy(InventoryGridIndexer other)
        {
            if (other == null) {
                Clear();
                return;
            }
            SetIndexItems(other.IndexedItems);
        }
        
        public virtual void Clear()
        {
            //Debug.Log("Clear");
            m_IndexedItems.Clear();
        }

        public virtual void SetIndexItems(Dictionary<ItemStack, int> indexedItems)
        {
            //Debug.Log("Set indexer "+indexedItems);
            
            if(indexedItems == null){ return; }
            m_IndexedItems.Clear();

            foreach (var indexedItem in indexedItems) {
                //Debug.Log("Indexing "+indexedItem.Key+" to index "+indexedItem.Value);
                m_IndexedItems[indexedItem.Key] = indexedItem.Value;
            }
        }

        public virtual void SetStackIndex(ItemStack itemStack, int itemStackIndex)
        {
            m_IndexedItems[itemStack] = itemStackIndex;
        }

        public virtual int GetItemStackIndex(ItemStack itemStack)
        {
            if (m_IndexedItems.TryGetValue(itemStack, out var result)) { return result; }

            return -1;
        }
        
        public virtual ItemStack GetItemStackAtIndex(int itemStackIndex)
        {
            foreach (var keyValuePair in m_IndexedItems) {
                if (keyValuePair.Value == itemStackIndex) { return keyValuePair.Key; }
            }

            return null;
        }
        
        public virtual bool CanMoveItem(int sourceIndex, int destinationIndex)
        {
            return true;
        }
        
        public virtual void MoveItemStackIndex(ItemStack sourceStack, ItemStack destinationStack)
        {
            var sourceIndex = GetItemStackIndex(sourceStack);
            var destinationIndex = GetItemStackIndex(destinationStack);

            if (sourceIndex != -1) { m_IndexedItems[destinationStack] = sourceIndex; }
            if (destinationIndex != -1) { m_IndexedItems[sourceStack] = destinationIndex; }
        }
        
        public virtual void MoveItemStackIndex(int sourceIndex, int destinationIndex)
        {
            var sourceStack = GetItemStackAtIndex(sourceIndex);
            var destinationStack = GetItemStackAtIndex(destinationIndex);
            
            if (destinationStack != null) {
                m_IndexedItems[destinationStack] = sourceIndex;
            }

            if (sourceStack != null) {
                m_IndexedItems[sourceStack] = destinationIndex;
            }
        }

        public virtual ListSlice<ItemInfo> SortItemIndexes(Comparer<ItemInfo> comparer)
        {
            var indexedItems = m_IndexedItems;
            
            m_CachedItemInfos.Clear();
            
            var itemInfos= indexedItems.Keys;
            foreach (var itemInfo in itemInfos) {
                m_CachedItemInfos.Add((ItemInfo)itemInfo);
            }
            
            m_CachedItemInfos.Sort(comparer);

            for (int i = 0; i < m_CachedItemInfos.Count; i++) {
                var itemStack = m_CachedItemInfos[i].ItemStack;
                if(itemStack == null){continue;}

                indexedItems[itemStack] = i;
            }

            return m_CachedItemInfos;
        }

        public virtual ListSlice<ItemInfo> GetOrderedItems(ListSlice<ItemInfo> itemInfos)
        {
            var indexedItems = m_IndexedItems;

            m_CachedItemInfos.Clear();

            // Remove the item stacks from the dictionary if they are not part of the input item infos.
            var pooledItemStacksToRemove = GenericObjectPool.Get<List<ItemStack>>();
            pooledItemStacksToRemove.Clear();
            
            foreach (var indexedItem in indexedItems) {
                var atLeastOneMatch = false;
                for (int i = 0; i < itemInfos.Count; i++) {
                    if (itemInfos[i].ItemStack != indexedItem.Key) { continue; }

                    atLeastOneMatch = true;
                    break;
                }

                if (atLeastOneMatch == false) {
                    pooledItemStacksToRemove.Add(indexedItem.Key);
                }
            }

            for (int i = 0; i < pooledItemStacksToRemove.Count; i++) { indexedItems.Remove(pooledItemStacksToRemove[i]); }
            pooledItemStacksToRemove.Clear();
            GenericObjectPool.Return(pooledItemStacksToRemove);
            
            // Find if the item has an existing index.
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

            // Assign a new index to any item which does not yet have one.
            var count = 0;
            while (m_TempUnsetItemInfos.Count > 0)
            {
                var itemInfo = m_TempUnsetItemInfos.Dequeue();
                if(itemInfo.ItemStack == null){ continue; }

                var indexIsSet = false;
                for (int i = count; i < m_CachedItemInfos.Count; i++) {

                    count++;
                    if (m_CachedItemInfos[i].ItemStack != null) { continue; }
                    if(itemInfo.ItemStack == null){break;}
                    
                    
                    m_CachedItemInfos[i] = itemInfo;
                    m_IndexedItems[itemInfo.ItemStack] = i;
                    indexIsSet = true;
                    break;
                }
                
                if(indexIsSet){ continue; }

                m_CachedItemInfos.Add(itemInfo);
                m_IndexedItems[itemInfo.ItemStack] = count;
                count++;
            }

            return m_CachedItemInfos;
        }

        
    }
}