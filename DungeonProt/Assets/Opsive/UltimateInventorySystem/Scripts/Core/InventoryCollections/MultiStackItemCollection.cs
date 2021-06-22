/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Core.InventoryCollections
{
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using System;
    using UnityEngine;
    using EventHandler = Opsive.Shared.Events.EventHandler;

    /// <summary>
    /// An ItemCollection that can have multiple stacks of an item.
    /// </summary>
    [Serializable]
    public class MultiStackItemCollection : ItemCollection
    {
        [Tooltip("The stack limit for a stack of an immutable item.")]
        [SerializeField] protected int m_DefaultStackSizeLimit = 99;
        [Tooltip("The attribute name used for limiting the stack size for an immutable item.")]
        [SerializeField] protected string m_StackSizeLimitAttributeName = "StackSizeLimit";

        /// <summary>
        /// Internal method which removes an ItemAmount from the collection.
        /// </summary>
        /// <param name="itemInfo">The item info to remove.</param>
        /// <returns>Returns the number of items removed, 0 if no item was removed.</returns>
        protected override ItemInfo RemoveInternal(ItemInfo itemInfo)
        {
            var removed = 0;
            var amountToRemove = itemInfo.Amount;
            var previousIndexWithItem = -1;
            var maxStackSize = GetMaxStackSize(itemInfo.Item);
            ItemStack itemStackRemoved = null;

            if (m_ItemStacks.Contains(itemInfo.ItemStack)) {
                itemStackRemoved = itemInfo.ItemStack;
                previousIndexWithItem = RemoveItemFromStack(m_ItemStacks.IndexOf(itemStackRemoved),
                    itemInfo, previousIndexWithItem, maxStackSize, ref amountToRemove, ref removed);
            }

            for (int i = 0; i < m_ItemStacks.Count; i++) {
                if (m_ItemStacks[i].Item.ID != itemInfo.Item.ID) { continue; }
                itemStackRemoved = m_ItemStacks[i];

                previousIndexWithItem = RemoveItemFromStack(i, itemInfo, previousIndexWithItem,
                    maxStackSize, ref amountToRemove, ref removed);
            }

            if (removed == 0) {
                return (removed, itemInfo.Item, this);
            }

            if (m_Inventory != null) {
                EventHandler.ExecuteEvent<ItemInfo>(m_Inventory, EventNames.c_Inventory_OnRemove_ItemInfo,
                    (itemInfo.Item, removed, this, itemStackRemoved));
            }

            UpdateCollection();

            return (removed, itemInfo.Item, this, itemStackRemoved);
        }

        /// <summary>
        /// Removes an item amount from a specific stack.
        /// </summary>
        /// <param name="i">The index of the item stack.</param>
        /// <param name="itemInfo">The item info to remove.</param>
        /// <param name="previousIndexWithItem">The previous Index with the item.</param>
        /// <param name="maxStackSize">The max stack size.</param>
        /// <param name="amountToRemove">The amount to remove.</param>
        /// <param name="removed">The amount removed.</param>
        /// <returns>The index with the item.</returns>
        private int RemoveItemFromStack(int i, ItemInfo itemInfo, int previousIndexWithItem,
            int maxStackSize, ref int amountToRemove, ref int removed)
        {
            var itemStackRemoved = m_ItemStacks[i];

            if (previousIndexWithItem != -1) {
                var mergedAmount = itemStackRemoved.Amount + m_ItemStacks[previousIndexWithItem].Amount;
                if (mergedAmount > maxStackSize) {
                    m_ItemStacks[previousIndexWithItem].SetAmount(maxStackSize);
                    itemStackRemoved.SetAmount(mergedAmount - maxStackSize);
                } else {
                    m_ItemStacks[previousIndexWithItem].SetAmount(mergedAmount);
                    itemStackRemoved.SetAmount(0);
                }
            }

            if (amountToRemove == 0) { return previousIndexWithItem; }

            var newAmount = itemStackRemoved.Amount - amountToRemove;
            if (newAmount <= 0) {
                amountToRemove = -newAmount;
                removed += itemStackRemoved.Amount;
                m_ItemStacks.RemoveAt(i);
                itemInfo.Item.RemoveItemCollection(this);
                itemStackRemoved.Reset();
                GenericObjectPool.Return<ItemStack>(itemStackRemoved);
            } else {
                amountToRemove = 0;
                removed += itemInfo.Amount;
                itemStackRemoved.SetAmount(newAmount);
                previousIndexWithItem = i;
            }

            return previousIndexWithItem;
        }

        /// <summary>
        /// Add an ItemAmount in an organized way.
        /// </summary>
        /// <param name="itemInfo">The item info being added to the item collection.</param>
        /// <param name="targetStack">The item stack where the item should be added to (Can be null).</param>
        protected override ItemInfo AddInternal(ItemInfo itemInfo, ItemStack targetStack, bool notifyAdd = true)
        {
            var amountToAdd = itemInfo.Amount;

            var maxStackSize = GetMaxStackSize(itemInfo.Item);
            ItemStack addedItemStack = null;

            if (m_ItemStacks.Contains(targetStack)) {
                var sizeDifference = targetStack.Amount + amountToAdd - maxStackSize;

                if (sizeDifference > 0) {
                    amountToAdd = sizeDifference;
                    itemInfo = (itemInfo.Item, itemInfo.Amount - sizeDifference, itemInfo.ItemCollection);
                } else { amountToAdd = 0; }

                targetStack.SetAmount(itemInfo.Amount + targetStack.Amount);
                addedItemStack = targetStack;
            }

            for (int i = 0; i < m_ItemStacks.Count; i++) {
                if (m_ItemStacks[i].Item.ID != itemInfo.Item.ID) { continue; }

                var sizeDifference = m_ItemStacks[i].Amount + amountToAdd - maxStackSize;

                if (sizeDifference > 0) {
                    amountToAdd = sizeDifference;
                    itemInfo = (itemInfo.Item, itemInfo.Amount - sizeDifference, itemInfo.ItemCollection);
                } else { amountToAdd = 0; }

                m_ItemStacks[i].SetAmount(itemInfo.Amount + m_ItemStacks[i].Amount);
                addedItemStack = m_ItemStacks[i];
            }

            var stacksToAdd = amountToAdd / maxStackSize;
            var remainderStack = amountToAdd % maxStackSize;

            for (int i = 0; i < stacksToAdd; i++) {
                var newItemStack = GenericObjectPool.Get<ItemStack>();
                newItemStack.Initialize((itemInfo.Item, maxStackSize), this);
                m_ItemStacks.Add(newItemStack);
            }

            if (remainderStack != 0) {
                var newItemStack = GenericObjectPool.Get<ItemStack>();
                newItemStack.Initialize((itemInfo.Item, maxStackSize), this);
                m_ItemStacks.Add(newItemStack);
            }

            itemInfo.Item.AddItemCollection(this);

            if (notifyAdd) {
                NotifyAdd(itemInfo,addedItemStack);
            }

            return (itemInfo.Item, itemInfo.Amount, this, addedItemStack);
        }

        /// <summary>
        /// Get the max stack size for the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The maximum stack size.</returns>
        protected virtual int GetMaxStackSize(Item item)
        {
            if (item.TryGetAttributeValue<int>(m_StackSizeLimitAttributeName, out var maxStackSize) == false) {
                maxStackSize = m_DefaultStackSizeLimit;
            }

            return maxStackSize;
        }
    }
}