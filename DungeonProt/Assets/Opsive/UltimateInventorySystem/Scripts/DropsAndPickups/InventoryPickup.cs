/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.DropsAndPickups
{
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.Interactions;
    using UnityEngine;

    /// <summary>
    /// A pickup that uses the inventory component.
    /// </summary>
    [RequireComponent(typeof(Inventory))]
    [RequireComponent(typeof(Interactable))]
    public class InventoryPickup : PickupBase
    {
        protected Inventory m_Inventory;

        public Inventory Inventory => m_Inventory;

        /// <summary>
        /// Initialize the component.
        /// </summary>
        protected void Awake()
        {
            m_Inventory = GetComponent<Inventory>();
        }

        /// <summary>
        /// The the inventory content to the interactors inventory.
        /// </summary>
        protected override void OnInteractInternal(IInteractor interactor)
        {
            if (!(interactor is IInteractorWithInventory interactorWithInventory)) { return; }

            AddPickupToCollection(interactorWithInventory.Inventory.MainItemCollection);
        }

        /// <summary>
        /// Add the pickup to the collection specified.
        /// </summary>
        /// <param name="itemCollection">The item Collection.</param>
        protected virtual void AddPickupToCollection(ItemCollection itemCollection)
        {
            var pickupItems = m_Inventory.MainItemCollection.GetAllItemStacks();
            Shared.Events.EventHandler.ExecuteEvent(m_Inventory.gameObject, "OnItemPickupStartPickup");
            for (int i = 0; i < pickupItems.Count; i++) {
                var itemAmount = pickupItems[i];
                if (itemAmount.Item.IsMutable) {
                    itemCollection.AddItem(Item.Create(itemAmount.Item), itemAmount.Amount);
                } else {
                    itemCollection.AddItem(itemAmount.Item, itemAmount.Amount);
                }
            }
            Shared.Events.EventHandler.ExecuteEvent(m_Inventory.gameObject, "OnItemPickupStopPickup");
            PlaySound();
        }
    }
}