/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.ItemActions
{
    using Opsive.Shared.Game;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.DropsAndPickups;
    using UnityEngine;

    /// <summary>
    /// Simple item action used to drop items.
    /// </summary>
    [System.Serializable]
    public class DropItemAction : ItemAction
    {
        [Tooltip("The pickup item prefab, it must have a ItemPickup component.")]
        [SerializeField] protected GameObject m_PickUpItemPrefab;
        [Tooltip("Drop One item instead of the item amount specified by the item info.")]
        [SerializeField] protected bool m_DropOne;
        [Tooltip("Remove the item that is dropped.")]
        [SerializeField] protected bool m_RemoveOnDrop;
        [Tooltip("The radius where the item should be dropped around the item user.")]
        [SerializeField] protected float m_DropRadius = 2f;

        public GameObject PickUpItemPrefab {
            get => m_PickUpItemPrefab;
            set => m_PickUpItemPrefab = value;
        }

        protected ItemPickup m_ItemPickup;
        public ItemPickup ItemPickup => m_ItemPickup;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DropItemAction()
        {
            m_Name = "Drop";
        }

        /// <summary>
        /// Check if the action can be invoked.
        /// </summary>
        /// <param name="itemInfo">The item.</param>
        /// <param name="itemUser">The item user (can be null).</param>
        /// <returns>True if the action can be invoked.</returns>
        protected override bool CanInvokeInternal(ItemInfo itemInfo, ItemUser itemUser)
        {
            return true;
        }

        /// <summary>
        /// Invoke the action.
        /// </summary>
        /// <param name="itemInfo">The item.</param>
        /// <param name="itemUser">The item user (can be null).</param>
        protected override void InvokeActionInternal(ItemInfo itemInfo, ItemUser itemUser)
        {
            if (m_PickUpItemPrefab == null) {
                Debug.LogWarning("Item Pickup Prefab is null on the Drop Item Action.");
                return;
            }

            var gameObject = itemUser?.gameObject ?? itemInfo.Inventory?.gameObject;

            if (gameObject == null) {
                Debug.LogWarning("The game object where the Item Pickup should spwaned to is null.");
                return;
            }

            if (m_DropOne) { itemInfo = (1, itemInfo); }

            if (m_RemoveOnDrop) {
                itemInfo.ItemCollection?.RemoveItem(itemInfo);
            }

            m_ItemPickup = ObjectPool.Instantiate(m_PickUpItemPrefab,
                gameObject.transform.position + new Vector3(Random.value * m_DropRadius - m_DropRadius / 2f, Random.value * m_DropRadius, Random.value * m_DropRadius - m_DropRadius / 2f),
                Quaternion.identity).GetComponent<ItemPickup>();

            if (m_ItemPickup == null) {
                Debug.LogWarning("Item Pickup is null on the Drop Item Action.");
                return;
            }

            var itemObject = m_ItemPickup.ItemObject;
            itemObject.SetItem(itemInfo.Item);
            itemObject.SetAmount(itemInfo.Amount);
        }
    }
}