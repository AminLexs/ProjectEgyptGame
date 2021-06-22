/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.ItemActions
{
    using Opsive.UltimateInventorySystem.Input;
    using System.Collections.Generic;
    using UnityEngine;

    public interface IItemUserAction
    { }
    
    /// <summary>
    /// The component that is used as the Item User.
    /// </summary>
    public class ItemUser : MonoBehaviour
    {
        [Tooltip("The component used to get input from the player to control UI and use items")]
        [SerializeField] protected InventoryInput m_InventoryInput;

        public InventoryInput InventoryInput
        {
            get => m_InventoryInput;
            set => m_InventoryInput = value;
        }

        protected Dictionary<IItemUserAction, object> m_ItemActionsData;

        /// <summary>
        /// Initialize the component.
        /// </summary>
        protected virtual void Awake()
        {
            if (m_InventoryInput == null) { m_InventoryInput = GetComponent<InventoryInput>(); }
            
            m_ItemActionsData = new Dictionary<IItemUserAction, object>();
        }

        /// <summary>
        /// Try getting the data for an item action.
        /// </summary>
        /// <param name="itemAction">The item action.</param>
        /// <param name="data">The stored data for that item action.</param>
        /// <returns>True if the item action data exists.</returns>
        public bool TryGetItemActionData(IItemUserAction itemAction, out object data)
        {
            return m_ItemActionsData.TryGetValue(itemAction, out data);
        }

        /// <summary>
        /// Set the item action data.
        /// </summary>
        /// <param name="itemAction">The item action.</param>
        /// <param name="data">The data for the item action.</param>
        public void SetItemActionData(IItemUserAction itemAction, object data)
        {
            m_ItemActionsData[itemAction] = data;
        }
    }
}