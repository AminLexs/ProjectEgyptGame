/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Panels
{
    using Opsive.UltimateInventorySystem.Interactions;
    using UnityEngine;

    /// <summary>
    /// Base class of interactables that open menus.
    /// </summary>
    public class MenuInteractableBehavior : InteractableBehavior
    {
        [Tooltip("The panel manager.")]
        [SerializeField] protected InventoryPanelOpener m_MenuOpener;

        /// <summary>
        /// On Interaction.
        /// </summary>
        /// <param name="interactor">The interactor.</param>
        protected override void OnInteractInternal(IInteractor interactor)
        {
            if (!(interactor is IInteractorWithInventory interactorWithInventory)) { return; }

            if (m_MenuOpener == null) { m_MenuOpener = GetComponent<InventoryPanelOpener>(); }
            
            m_MenuOpener.Open(interactorWithInventory.Inventory);
        }
    }
}