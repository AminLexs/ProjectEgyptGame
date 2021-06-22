namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers
{
    using Opsive.UltimateInventorySystem.ItemActions;
    using UnityEngine;
    using UnityEngine.Serialization;

    public class ItemViewSlotsContainerItemActionBinding : ItemViewSlotsContainerItemActionBindingBase
    {
        [FormerlySerializedAs("m_CategoryItemActions")]
        [Tooltip("The categories item actions. Specifies the actions that can be performed on each item. Can be null.")]
        [SerializeField] public ItemActionSet m_ItemActionSet;

        public override void Initialize(bool force)
        {
            if(m_IsInitialized && force == false){ return; }
            base.Initialize(force);
            
            m_ItemActionSet.ItemActionCollection.Initialize(false);
            m_ItemActions = new ItemAction[m_ItemActionSet.ItemActionCollection.Count];
            for (int i = 0; i < m_ItemActions.Length; i++) {
                m_ItemActions[i] = m_ItemActionSet.ItemActionCollection[i];
                m_ItemActions[i].Initialize(false);
            }
        }

        public override bool CanItemUsAction(int itemSlotIndex)
        {
            var canUse = base.CanItemUsAction(itemSlotIndex);
            if (canUse == false) { return false; }

            var itemInfo = m_ItemViewSlotsContainer.ItemViewSlots[itemSlotIndex].ItemInfo;
            if (m_ItemActionSet.MatchItem(itemInfo.Item) == false) {
                return false;
            }

            return true;
        }
        
        public override string ToString()
        {
            var actionsName = m_ItemActionSet == null ? "NULL" : m_ItemActionSet.name;
            return GetType().Name + ": "+ actionsName;
        }
    }
}