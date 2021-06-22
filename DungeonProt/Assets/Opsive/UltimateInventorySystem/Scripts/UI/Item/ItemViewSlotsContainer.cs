namespace Opsive.UltimateInventorySystem.UI.Item
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// The hot item bar component allows you to use an item action for an item that was added to the hot bar.
    /// </summary>
    public class ItemViewSlotsContainer : ItemViewSlotsContainerBase
    {
        [FormerlySerializedAs("m_ItemBoxDrawer")]
        [Tooltip("The Item View drawer.")]
        [SerializeField] protected ItemViewDrawer m_ItemViewDrawer;
        [FormerlySerializedAs("m_SwapItemBoxOnAssign")]
        [Tooltip("Swap Item Viewes when Item is assigned.")]
        [SerializeField] protected bool m_SwapItemViewOnAssign;
        
        [Tooltip("The parent of all the itemBoxSlots.")]
        [SerializeField] protected RectTransform m_Content;
        
        public RectTransform Content => m_Content;

        public override void Initialize(bool force)
        {
            if(m_IsInitialized && !force){ return; }
            
            if (m_Content == null) { m_Content = transform as RectTransform;}
            
            m_ItemViewSlots = m_Content.GetComponentsInChildren<ItemViewSlot>();

            if (Application.isPlaying) {
                m_ItemViewDrawer.Content = m_Content;
                m_ItemViewDrawer.Initialize(force);
            }

            base.Initialize(force);
        }

        /// <summary>
        /// Assign an item to a slot.
        /// </summary>
        /// <param name="itemInfo">The item.</param>
        /// <param name="slot">The item slot.</param>
        protected override void AssignItemToSlot(ItemInfo itemInfo, int slot)
        {
            if (m_SwapItemViewOnAssign) {
                m_ItemViewDrawer.DrawView(slot, slot, itemInfo, true);
                return;
            }
            
            base.AssignItemToSlot(itemInfo, slot);
        }

        /// <summary>
        /// Get the Box prefab for the item specified.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        /// <returns>The box prefab game object.</returns>
        public override GameObject GetBoxPrefabFor(ItemInfo itemInfo)
        {
            return m_ItemViewDrawer.CategoryItemViewSet.FindItemViewPrefabForItem(itemInfo.Item);
        }
    }
}