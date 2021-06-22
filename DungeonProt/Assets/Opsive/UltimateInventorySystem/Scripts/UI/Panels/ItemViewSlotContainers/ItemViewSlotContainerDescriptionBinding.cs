namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Item;
    using UnityEngine;

    public class ItemViewSlotContainerDescriptionBinding : ItemViewSlotsContainerBinding
    {
        [Tooltip("The item description panel. Can be null.")]
        [SerializeField] internal ItemDescriptionPanelBinding m_ItemDescriptionPanel;
        [Tooltip("The item description panel. Can be null.")]
        [SerializeField] internal ItemDescriptionBase m_ItemDescription;
        [Tooltip("The item description panel. Can be null.")]
        [SerializeField] protected bool m_HideIfSelectedItemIsNull;

        public ItemDescriptionBase ItemDescription => m_ItemDescription;

        protected override void OnBind()
        {
            if (m_ItemDescriptionPanel != null) {
                m_ItemDescription = m_ItemDescriptionPanel.ItemDescription;
            }
            
            m_ItemViewSlotsContainer.OnItemViewSlotSelected += DrawDescription;
        }

        protected override void OnUnBind()
        {
            m_ItemViewSlotsContainer.OnItemViewSlotSelected -= DrawDescription;
        }

        private void DrawDescription(ItemViewSlotEventData sloteventdata)
        {
            var itemInfo = sloteventdata?.ItemView?.ItemInfo;
            if (itemInfo.HasValue == false || itemInfo.Value.Item == null) {
                ClearDescription(); 
                return;
            }

            DrawDescription(itemInfo.Value);
        }

        /// <summary>
        /// Clear the description on the item.
        /// </summary>
        /// <param name="index">The index.</param>
        private void ClearDescription()
        {
            if (m_ItemDescription == null) { return; }
            m_ItemDescription.Clear();

            if (m_HideIfSelectedItemIsNull) {
                if (m_ItemDescriptionPanel != null) {
                    m_ItemDescriptionPanel.DisplayPanel.Close();
                } else {
                    m_ItemDescription.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Draw the description for an item.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        /// <param name="index">The index.</param>
        protected virtual void DrawDescription(ItemInfo itemInfo)
        {
            if (m_ItemDescription == null) { return; }

            m_ItemDescription.SetValue(itemInfo);
            
            if (m_HideIfSelectedItemIsNull) {
                if (m_ItemDescriptionPanel != null) {
                    m_ItemDescriptionPanel.DisplayPanel.Open();
                } else {
                    m_ItemDescription.gameObject.SetActive(true);
                }
            }
        }
    }
}