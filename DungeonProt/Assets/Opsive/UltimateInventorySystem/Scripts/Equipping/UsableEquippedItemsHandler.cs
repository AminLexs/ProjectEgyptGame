namespace Opsive.UltimateInventorySystem.Equipping
{
    using Opsive.Shared.Events;
    using Opsive.Shared.Game;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.ItemActions;
    using Opsive.UltimateInventorySystem.ItemObjectActions;
    using Opsive.UltimateInventorySystem.ItemObjectBehaviours;
    using UnityEngine;

    public class UsableEquippedItemsHandler : MonoBehaviour
    {
        [SerializeField] protected ItemUser m_ItemUser;

        protected IEquipper m_Equipper;
        public IEquipper Equipper { 
            get => m_Equipper;
            set => m_Equipper = value;
        }

        private void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            if (m_Equipper == null) { m_Equipper = GetComponent<IEquipper>(); }

            if (m_ItemUser == null) { 
                m_ItemUser = GetComponent<ItemUser>();
                if (m_ItemUser == null) {
                    return;
                }
            }

            EventHandler.RegisterEvent<int, int>(m_ItemUser.gameObject, EventNames.c_GameObject_OnInput_UseItemObject_Int_Int,
                UseItem);
        }

        /// <summary>
        /// Use an item directly without using input.
        /// </summary>
        /// <param name="itemObjectIndex">The index of the usable item object to use.</param>
        /// <param name="itemActionIndex">The item action index to use.</param>
        private void UseItem(int itemObjectIndex, int itemActionIndex)
        {
            if (itemObjectIndex < 0 || itemObjectIndex >= m_Equipper.Slots.Length) {
                return;
            }
            
            var slot = m_Equipper.Slots[itemObjectIndex];

            var itemObject = slot.ItemObject;
            
            if(itemObject == null){return;}

            var itemObjectBehaviourHandler = itemObject.gameObject.GetCachedComponent<IItemObjectBehaviourHandler>();
            
            UseItem(itemObjectBehaviourHandler, itemActionIndex);
        }

        /// <summary>
        /// Use an item directly without using input.
        /// </summary>
        /// <param name="itemObjectBehaviourHandler">The usable item object to use.</param>
        /// <param name="itemActionIndex">The item action index to use.</param>
        public virtual void UseItem(IItemObjectBehaviourHandler itemObjectBehaviourHandler, int itemActionIndex)
        {
            if (itemObjectBehaviourHandler == null) { return; }
            itemObjectBehaviourHandler.UseItem(m_ItemUser,itemActionIndex);
        }
    }
}