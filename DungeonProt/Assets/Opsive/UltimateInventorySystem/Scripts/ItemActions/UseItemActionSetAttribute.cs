namespace Opsive.UltimateInventorySystem.ItemActions
{
    using Opsive.Shared.Game;
    using Opsive.UltimateInventorySystem.Core.AttributeSystem;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.DropsAndPickups;
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// Simple item action used to drop items.
    /// </summary>
    [System.Serializable]
    public class UseItemActionSetAttribute : ItemAction
    {
        [Tooltip("The name of the attribute which holds the Item Action.")]
        [SerializeField] protected string m_AttributeName = "ItemActionSet";
        [Tooltip("Drop One item instead of the item amount specified by the item info.")]
        [SerializeField] protected int m_ActionIndex;
        [Tooltip("Drop One item instead of the item amount specified by the item info.")]
        [SerializeField] protected bool m_UseOne = true;
        [Tooltip("Remove the item that is dropped.")]
        [SerializeField] protected bool m_RemoveOnUse;

        public int ActionIndex
        {
            get => m_ActionIndex;
            set => m_ActionIndex = value;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UseItemActionSetAttribute()
        {
            m_Name = "Use";
        }

        /// <summary>
        /// Check if the action can be invoked.
        /// </summary>
        /// <param name="itemInfo">The item.</param>
        /// <param name="itemUser">The item user (can be null).</param>
        /// <returns>True if the action can be invoked.</returns>
        protected override bool CanInvokeInternal(ItemInfo itemInfo, ItemUser itemUser)
        {
            if (itemInfo.Item == null) { return false; }

            var attribute = itemInfo.Item.GetAttribute<Attribute<ItemActionSet>>(m_AttributeName);
            if (attribute == null) { return false;}
            
            var actionSet = attribute.GetValue();
            if (actionSet == null) { return false; }

            if (m_ActionIndex < 0 || actionSet.ItemActionCollection.Count >= m_ActionIndex) { return false;}
            
            return true;
        }

        /// <summary>
        /// Invoke the action.
        /// </summary>
        /// <param name="itemInfo">The item.</param>
        /// <param name="itemUser">The item user (can be null).</param>
        protected override void InvokeActionInternal(ItemInfo itemInfo, ItemUser itemUser)
        {
            if (m_UseOne) { itemInfo = (1, itemInfo); }
            
            var attribute = itemInfo.Item.GetAttribute<Attribute<ItemActionSet>>(m_AttributeName);
            var actionSet = attribute.GetValue();
            
            actionSet.ItemActionCollection[m_ActionIndex].InvokeAction(itemInfo,itemUser);
            
            if (m_RemoveOnUse) {
                itemInfo.ItemCollection?.RemoveItem(itemInfo);
            }
        }
    }
}