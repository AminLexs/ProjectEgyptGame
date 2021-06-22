/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.ItemActions
{
    using Opsive.Shared.Game;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Panels;
    using Opsive.UltimateInventorySystem.UI.Panels.ActionPanels;
    using Opsive.UltimateInventorySystem.UI.Panels.Hotbar;
    using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
    using UnityEngine;

    /// <summary>
    /// Demo item action used to assign items to the hotbar
    /// </summary>
    [System.Serializable]
    public class AssignHotbarItemAction : ItemActionWithAsyncFuncActionPanel<int>
    {
        [SerializeField] protected string m_HotbarPanelName = "Item Hotbar";
        protected ItemHotbar m_ItemHotbar;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AssignHotbarItemAction()
        {
            m_Name = "Assign";
        }

        /// <summary>
        /// Invoke the action.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        /// <param name="itemUser">The item user (can be null).</param>
        protected override void InvokeActionInternal(ItemInfo itemInfo, ItemUser itemUser)
        {
            if (m_AsyncFuncActions.Count < m_ItemHotbar.SlotCount) {

                for (int i = m_AsyncFuncActions.Count; i < m_ItemHotbar.SlotCount; i++) {
                    var localIndex = i;
                    m_AsyncFuncActions.Add(new AsyncFuncAction<int>((localIndex + 1).ToString(), () => localIndex));
                }
            } else if (m_AsyncFuncActions.Count > m_ItemHotbar.SlotCount) {
                m_AsyncFuncActions.Trim(m_ItemHotbar.SlotCount);
            }

            base.InvokeActionInternal(itemInfo, itemUser);
        }

        /// <summary>
        /// Invoke action after waiting for index slot.
        /// </summary>
        /// <param name="itemInfo">The item.</param>
        /// <param name="itemUser">The item user (can be null).</param>
        /// <param name="awaitedValue">The index slot.</param>
        protected override void InvokeWithAwaitedValue(ItemInfo itemInfo, ItemUser itemUser, int awaitedValue)
        {
            m_ItemHotbar.AssignItemToSlot(itemInfo, awaitedValue);
        }

        /// <summary>
        /// Can the action be invoked.
        /// </summary>
        /// <param name="itemInfo">The item Info.</param>
        /// <param name="itemUser">The item user (can be null).</param>
        /// <returns>True if it can be invoked.</returns>
        protected override bool CanInvokeInternal(ItemInfo itemInfo, ItemUser itemUser)
        {
            var panelManager = m_PanelParentPanel?.Manager;

            if (panelManager == null) {
                panelManager = GameObject.FindObjectOfType<DisplayPanelManager>();
                if (panelManager == null) {
                    Debug.LogError("The Assign Hotbar Item Action could not find the display panel manager.");
                    return false;
                }
            }
            
            var hotbarPanel = panelManager.GetPanel(m_HotbarPanelName);
            for (int i = 0; i < hotbarPanel.Bindings.Count; i++) {
                var binding = hotbarPanel.Bindings[i];
                if (binding is ItemViewSlotsContainerPanelBinding itemViewPanel) {
                    var itemViewSlotsContainer = itemViewPanel.ItemViewSlotsContainer;
                    m_ItemHotbar = itemViewSlotsContainer.gameObject.GetCachedComponent<ItemHotbar>();
                    break;
                }
            }

            var item = itemInfo.Item;
            return m_ItemHotbar != null
                   && itemInfo.ItemCollection.HasItem((1, item));
        }
    }
}