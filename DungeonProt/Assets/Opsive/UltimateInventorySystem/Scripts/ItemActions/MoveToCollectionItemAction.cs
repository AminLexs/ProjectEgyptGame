/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.ItemActions
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using UnityEngine;

    /// <summary>
    /// Item action used to Move an item from one collection to another. It can be used to equip/unequip items too.
    /// </summary>
    [System.Serializable]
    public class MoveToCollectionItemAction : ItemAction
    {
        [Tooltip("The first original item collection.")]
        [SerializeField] protected ItemCollectionID m_FirstCollectionID = new ItemCollectionID(null, ItemCollectionPurpose.Main);
        [Tooltip("The second item collection.")]
        [SerializeField] protected ItemCollectionID m_SecondCollectionID = new ItemCollectionID(null, ItemCollectionPurpose.Equipped);
        [Tooltip("The name for the action to move the item from the first to the second item collection.")]
        [SerializeField] protected string m_MoveFromFirstToSecondActionName = "Equip";
        [Tooltip("The name for the action to move the item from the second to the first item collection.")]
        [SerializeField] protected string m_MoveFromSecondToFirstActionName = "Unequip";

        protected bool m_MoveFromFirstToSecond;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MoveToCollectionItemAction()
        {
            m_Name = "Equip";
        }

        /// <summary>
        /// Can the item action be invoked.
        /// </summary>
        /// <param name="itemInfo">The item.</param>
        /// <param name="itemUser">The item user (can be null).</param>
        /// <returns>True if it can be invoked.</returns>
        protected override bool CanInvokeInternal(ItemInfo itemInfo, ItemUser itemUser)
        {
            var item = itemInfo.Item;
            var inventory = itemInfo.Inventory;

            var secondCollection = inventory.GetItemCollection(m_SecondCollectionID);

            if (secondCollection == null) { return false; }

            if (secondCollection.HasItem((1, item))) {
                m_MoveFromFirstToSecond = false;
                m_Name = m_MoveFromSecondToFirstActionName;
            } else {
                m_MoveFromFirstToSecond = true;
                m_Name = m_MoveFromFirstToSecondActionName;
            }

            return true;
        }

        /// <summary>
        /// Move an item from one collection to another.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        /// <param name="itemUser">The item user (can be null).</param>
        protected override void InvokeActionInternal(ItemInfo itemInfo, ItemUser itemUser)
        {
            var item = itemInfo.Item;
            var inventory = itemInfo.Inventory;
            var firstCollection = inventory.GetItemCollection(m_FirstCollectionID);
            var secondCollection = inventory.GetItemCollection(m_SecondCollectionID);

            var originalCollection = m_MoveFromFirstToSecond ? firstCollection : secondCollection;
            var destinationCollection = m_MoveFromFirstToSecond ? secondCollection : firstCollection;

            if (destinationCollection is ItemSlotCollection itemSlotCollection) {
                var slotIndex = itemSlotCollection.GetTargetSlotIndex(item);
                if (slotIndex != -1) {
                    var previousItem = itemSlotCollection.GetItemAtSlot(slotIndex);
                    var amount = itemSlotCollection.GetItemAmount(previousItem);
                    itemSlotCollection.GiveItem((ItemInfo)(previousItem, amount), firstCollection, null);
                }
            }

            originalCollection.GiveItem((ItemInfo)(item, 1), destinationCollection, null);
        }
    }
}