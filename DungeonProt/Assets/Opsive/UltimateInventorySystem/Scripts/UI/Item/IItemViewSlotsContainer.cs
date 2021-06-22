namespace Opsive.UltimateInventorySystem.UI.Item
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
    using Opsive.UltimateInventorySystem.UI.Views;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public delegate void ItemViewSlotEventHandler(ItemViewSlotEventData slotEventData);

    public class ItemViewSlotEventData
    {
        protected ItemViewSlotsContainerBase m_ItemViewSlotsContainer;
        protected ItemViewSlot m_ItemViewSlot;
        protected ItemView m_ItemView;
        protected int m_Index;

        public ItemViewSlotsContainerBase ItemViewSlotsContainer => m_ItemViewSlotsContainer;
        public ItemViewSlot ItemViewSlot => m_ItemViewSlot;//{ get; set; }
        public ItemView ItemView => m_ItemView;
        public int Index => m_Index;

        public virtual void Reset()
        {
            m_ItemViewSlotsContainer = null;
            m_ItemView = null;
            m_ItemViewSlot = null;
            m_Index = -1;
        }

        public void SetValues(ItemViewSlotsContainerBase container, int index)
        {
            Reset();
            m_ItemViewSlotsContainer = container;
            m_Index = index;
            m_ItemViewSlot = ItemViewSlotsContainer.GetItemViewSlot(index);
            m_ItemView = m_ItemViewSlot.ItemView;
        }
    }
    
    public delegate void ItemViewSlotPointerEventHandler(ItemViewSlotPointerEventData eventData);

    public class ItemViewSlotPointerEventData : ItemViewSlotEventData
    {
        public PointerEventData PointerEventData { get; set; }

        public override void Reset()
        {
            base.Reset();
            PointerEventData = null;
        }
    }
}