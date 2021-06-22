namespace Opsive.UltimateInventorySystem.UI.Item
{
    using Opsive.UltimateInventorySystem.UI.Item.DragAndDrop;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
    using Opsive.UltimateInventorySystem.UI.Panels;
    using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
    using UnityEngine;

    public class ItemViewSlotMoveCursor : ItemViewSlotsContainerBinding
    {
        [SerializeField] internal ItemViewDropHandler m_DropHandler;
        [Tooltip("Can be a hidden display, it is required to allow cancelling the move action without closing the other panels")]
        [SerializeField] internal DisplayPanel m_MoveDisplayPanel;
        [Tooltip("Unbind ItemAction while moving")]
        [SerializeField] internal ItemViewSlotsContainerBinding[] m_UnbindWhileMoving;
        
        protected bool m_IsMoving;
        protected ItemViewSlotEventData m_ItemViewSlotEventData;
        
        private ItemViewSlotCursorManager ItemViewSlotCursorManager =>
            m_ItemViewSlotsContainer.ItemViewSlotCursor;

        protected override void OnBind()
        {
            if (m_DropHandler == null) {
                m_DropHandler = GetComponent<ItemViewDropHandler>();
                if (m_DropHandler == null) {
                    Debug.LogError("The Drop Handler is missing",gameObject);
                    return;
                }
            }
            
            if (m_ItemViewSlotEventData == null) {
                m_ItemViewSlotEventData = new ItemViewSlotEventData();
            }
            m_ItemViewSlotsContainer.OnItemViewSlotSelected += ItemViewSlotSelected;
            m_ItemViewSlotsContainer.OnItemViewSlotClicked += ItemViewSlotClicked;
            if (m_MoveDisplayPanel != null) {
                m_MoveDisplayPanel.OnClose += CancelMove;
            }
        }

        protected override void OnUnBind()
        {
            m_ItemViewSlotsContainer.OnItemViewSlotSelected -= ItemViewSlotSelected;
            m_ItemViewSlotsContainer.OnItemViewSlotClicked -= ItemViewSlotClicked;
            if (m_MoveDisplayPanel != null) {
                m_MoveDisplayPanel.OnClose -= CancelMove;
            }
        }

        private void ItemViewSlotSelected(ItemViewSlotEventData eventdata)
        {
            if(m_IsMoving == false || ItemViewSlotCursorManager.IsMovingItemView == false){ return; }
            ItemViewSlotCursorManager.SetPosition(eventdata.ItemView.transform.position);
        }

        private void ItemViewSlotClicked(ItemViewSlotEventData eventdata)
        {
            if(m_IsMoving == false || ItemViewSlotCursorManager.IsMovingItemView == false){ return; }
            m_DropHandler.HandleItemViewSlotDrop(eventdata);
            
            if (m_MoveDisplayPanel != null) {
                m_MoveDisplayPanel.Close(true);
            }
            
            EndMove();
        }

        private void CancelMove()
        {
            ItemViewSlotCursorManager.RemoveItemView();
            EndMove();
        }

        protected void EndMove()
        {
            m_IsMoving = false;
            for (int i = 0; i < m_UnbindWhileMoving.Length; i++) {
                m_UnbindWhileMoving[i]?.Bind(m_ItemViewSlotsContainer);
            }
        }

        public void StartMove(int index)
        {
            var itemViewSlot = m_ItemViewSlotsContainer.GetItemViewSlot(index);
            
            if (m_MoveDisplayPanel != null) {
                m_MoveDisplayPanel.Open(m_MoveDisplayPanel.Manager.SelectedDisplayPanel,
                    null, false);
            }
            
            m_IsMoving = true;
            
            m_ItemViewSlotEventData.SetValues(m_ItemViewSlotsContainer, index);
            
            ItemViewSlotCursorManager.SetSourceItemViewSlot(m_ItemViewSlotEventData);
            ItemViewSlotCursorManager.SetPosition(itemViewSlot.transform.position);
            
            for (int i = 0; i < m_UnbindWhileMoving.Length; i++) {
                m_UnbindWhileMoving[i]?.UnBind();
            }
        }
    }
}