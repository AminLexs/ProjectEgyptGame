/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Core.InventoryCollections
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using EventHandler = Opsive.Shared.Events.EventHandler;

    /// <summary>
    /// An ItemCollection that have a fixed amount of item stacks. which allows you to set items to specific indexes.
    /// </summary>
    [Serializable]
    public class FixedSizeItemCollection : ItemCollection
    {
        [Tooltip("The total number of item stacks allowed in this item collection.")]
        [SerializeField] protected int m_NumberOfStacks = 20;

        public int NumberOfStacks => m_NumberOfStacks;

        /// <summary>
        /// Initializes the Item Collection.
        /// </summary>
        /// <param name="owner">The GameObject doing the initialization.</param>
        /// <param name="force">Shouold the collection be force initialized?</param>
        public override void Initialize(IInventory owner, bool force)
        {
            if (m_Initialized && !force && m_Inventory != null) { return; }
            m_Inventory = owner;

            if (m_ItemStacks == null) {
                m_ItemStacks = new List<ItemStack>();
                for (int i = 0; i < m_NumberOfStacks; i++) {
                    var itemStack = new ItemStack();
                    itemStack.Initialize(ItemAmount.None, this);
                    m_ItemStacks.Add(itemStack);
                }
            }
            m_Initialized = true;

            if (Application.isPlaying == false || !force) { return; }

            m_ItemStacks.Clear();
            LoadDefaultLoadout();
        }

        /// <summary>
        /// Returns the number of item stacks which do not have an item.
        /// </summary>
        /// <returns>The number of available stacks.</returns>
        protected int NumberOfAvailableStacks()
        {
            var count = 0;

            for (int i = 0; i < m_ItemStacks.Count; i++) {
                if (m_ItemStacks[i].Item != null) { count++; }
            }

            return m_NumberOfStacks - count;
        }

        /// <summary>
        /// Add conditions, returns the itemInfo that can be added (or returns null if it cannot).
        /// </summary>
        /// <param name="itemInfo">Can this item be added.</param>
        public override ItemInfo? AddItemCondition(ItemInfo itemInfo)
        {
            var result = base.AddItemCondition(itemInfo);
            if (result.HasValue == false) {
                return null;
            }

            if (NumberOfAvailableStacks() == 0 && HasItem(itemInfo.Item) == false) { return null; }

            return result.Value;
        }

        /// <summary>
        /// Add an Item Amount in an organized way.
        /// </summary>
        /// <param name="itemInfo">The Item info being added to the item collection.</param>
        /// <param name="itemStackIndex">The item stack where you would like the item to be added.</param>
        public virtual ItemInfo AddItem(ItemInfo itemInfo, int itemStackIndex)
        {
            return AddItem(itemInfo, m_ItemStacks[itemStackIndex]);
        }

        /// <summary>
        /// Add an Item Amount in an organized way.
        /// </summary>
        /// <param name="itemInfo">The Item info being added to the item collection.</param>
        /// <param name="stackTarget">The item stack where you would like the item to be added (can be null).</param>
        protected override ItemInfo AddInternal(ItemInfo itemInfo, ItemStack stackTarget, bool notifyAdd = true)
        {
            var found = false;
            ItemStack addedItemStack = null;

            if (stackTarget != null && stackTarget.ItemCollection == this) {
                var index = m_ItemStacks.IndexOf(stackTarget);

                if (stackTarget.Item != null) {
                    if (stackTarget.Item == itemInfo.Item) {
                        stackTarget.SetAmount(itemInfo.Amount + stackTarget.Amount);
                        addedItemStack = stackTarget;
                    } else {
                        var itemInfoToRemove = (ItemInfo)stackTarget;

                        RemoveItem(itemInfoToRemove);
                        if (m_Inventory != null) {
                            EventHandler.ExecuteEvent<ItemInfo>(m_Inventory,
                                EventNames.c_Inventory_OnForceRemove_ItemInfo, itemInfoToRemove);
                        }

                        m_ItemStacks[index].Initialize((itemInfo.Item, itemInfo.Amount), this);
                    }
                } else {
                    m_ItemStacks[index].Initialize((itemInfo.Item, itemInfo.Amount), this);
                }
                found = true;
            }

            if (!found) {
                for (int i = 0; i < m_ItemStacks.Count; i++) {
                    if (m_ItemStacks[i].Item != itemInfo.Item) { continue; }

                    m_ItemStacks[i].SetAmount(itemInfo.Amount + m_ItemStacks[i].Amount);
                    addedItemStack = m_ItemStacks[i];
                    found = true;
                    break;
                }
            }

            if (!found) {
                for (int i = 0; i < m_ItemStacks.Count; i++) {
                    if (m_ItemStacks[i].Item != null) { continue; }

                    m_ItemStacks[i].Initialize((itemInfo.Item, itemInfo.Amount), this);
                    break;
                }
            }

            itemInfo.Item.AddItemCollection(this);

            if (notifyAdd) {
                NotifyAdd(itemInfo, addedItemStack);
            }

            return (itemInfo.Item, itemInfo.Amount, this, addedItemStack);
        }

        /// <summary>
        /// Remove an item from the itemCollection.
        /// </summary>
        /// <param name="itemInfo">The item to remove.</param>
        /// <param name="itemStackIndex">The stack index to remove from.</param>
        /// <returns>Returns true if the item was removed correctly.</returns>
        public ItemInfo RemoveItem(ItemInfo itemInfo, int itemStackIndex)
        {
            var itemStackTarget = m_ItemStacks[itemStackIndex];
            if (itemInfo.Item != itemStackTarget.Item) { return new ItemInfo(); }

            return RemoveItem((itemInfo.ItemAmount, this, itemStackTarget));
        }

        /// <summary>
        /// Internal method which removes an ItemAmount from the collection.
        /// </summary>
        /// <param name="itemInfo">The item info to remove.</param>
        /// <returns>Returns the number of items removed, 0 if no item was removed.</returns>
        protected override ItemInfo RemoveInternal(ItemInfo itemInfo)
        {
            var removed = 0;
            ItemStack itemStackToRemove = null;

            if (itemInfo.ItemStack != null && itemInfo.ItemStack.ItemCollection == this) {

                itemStackToRemove = itemInfo.ItemStack;
                var newAmount = itemStackToRemove.Amount - itemInfo.Amount;
                if (newAmount <= 0) {
                    removed = itemStackToRemove.Amount;
                    itemStackToRemove.Initialize(ItemAmount.None, this);
                    itemInfo.Item.RemoveItemCollection(this);
                } else {
                    removed = itemInfo.Amount;
                    itemStackToRemove.SetAmount(newAmount);
                }
            }

            if (removed != itemInfo.Amount) {
                for (int i = 0; i < m_ItemStacks.Count; i++) {
                    if (m_ItemStacks[i].Item != itemInfo.Item) { continue; }

                    itemStackToRemove = m_ItemStacks[i];
                    var newAmount = itemStackToRemove.Amount - itemInfo.Amount;
                    if (newAmount <= 0) {
                        removed = itemStackToRemove.Amount;
                        itemStackToRemove.Initialize(ItemAmount.None, this);
                        itemInfo.Item.RemoveItemCollection(this);
                    } else {
                        removed = itemInfo.Amount;
                        itemStackToRemove.SetAmount(newAmount);
                    }
                    break;
                }
            }

            if (removed == 0) {
                return (removed, itemInfo.Item, this);
            }

            if (m_Inventory != null) {
                EventHandler.ExecuteEvent<ItemInfo>(m_Inventory, EventNames.c_Inventory_OnRemove_ItemInfo,
                    (itemInfo.Item, itemInfo.Amount, this, itemStackToRemove));
            }

            UpdateCollection();
            return (removed, itemInfo.Item, this, itemStackToRemove);
        }
    }
}