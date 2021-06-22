/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Item.ItemViewModules
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.CompoundElements;
    using UnityEngine;

    /// <summary>
    /// An Item View component to display an amount.
    /// </summary>
    public class AmountItemView : ItemViewModule
    {
        [Tooltip("The amount text.")]
        [SerializeField] protected Text m_AmountText;
        [Tooltip("Do not show amount if it is 1 or lower.")]
        [SerializeField] protected bool m_HideAmountIfSingle;

        /// <summary>
        /// Set the value.
        /// </summary>
        /// <param name="info">The item info.</param>
        public override void SetValue(ItemInfo info)
        {
            if (m_HideAmountIfSingle && info.Amount <= 1) {
                m_AmountText.text = "";
            } else {
                m_AmountText.text = $"x {info.Amount}";
            }
        }

        /// <summary>
        /// Clear the value.
        /// </summary>
        public override void Clear()
        {
            m_AmountText.text = "";
        }
    }
}