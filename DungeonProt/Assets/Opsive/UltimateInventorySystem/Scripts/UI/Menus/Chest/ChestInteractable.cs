/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Menus.Chest
{
    using Opsive.UltimateInventorySystem.Interactions;
    using UnityEngine;

    /// <summary>
    /// A chest component which allows to open a chest menu on interaction.
    /// </summary>
    public class ChestInteractable : InteractableBehavior
    {
        [Tooltip("The chest menu.")]
        [SerializeField] protected Chest m_Chest;

        protected void Awake()
        {
            m_Chest.OnClose += () =>
            {
                m_Interactable.SetIsInteractable(true);
            };
            m_Chest.OnOpen += (clientInventory) =>
            {
                m_Interactable.SetIsInteractable(false);
            };
        }

        /// <summary>
        /// Make the chest no longer interactable.
        /// </summary>
        public override void Deactivate()
        {
            m_Interactable.SetIsInteractable(false);
        }

        /// <summary>
        /// Use the interactor to reference its inventory in the chest menu.
        /// </summary>
        /// <param name="interactor">The interactor.</param>
        protected override void OnInteractInternal(IInteractor interactor)
        {
            if (!(interactor is IInteractorWithInventory interactorWithInventory)) { return; }

            m_Chest.Open(interactorWithInventory.Inventory);
        }
    }
}