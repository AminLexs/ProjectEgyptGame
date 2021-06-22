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
    /// Item View component displaying an integer attribute value.
    /// </summary>
    public class IntAttributeItemView : ItemViewModule
    {
        [Tooltip("The attribute name. Must be an integer.")]
        [SerializeField] protected string m_AttributeName;
        [Tooltip("The attribute value text.")]
        [SerializeField] protected Text m_AttributeValueText;

        public string AttributeName {
            get => m_AttributeName;
            set => m_AttributeName = value;
        }

        /// <summary>
        /// Set the value.
        /// </summary>
        /// <param name="info">The item info.</param>
        public override void SetValue(ItemInfo info)
        {
            if (info.Item.TryGetAttributeValue<int>(m_AttributeName, out var value)) {
                m_AttributeValueText.text = value.ToString();
            }
        }

        /// <summary>
        /// Clear the value.
        /// </summary>
        public override void Clear()
        {
            m_AttributeValueText.text = "0";
        }
    }
}