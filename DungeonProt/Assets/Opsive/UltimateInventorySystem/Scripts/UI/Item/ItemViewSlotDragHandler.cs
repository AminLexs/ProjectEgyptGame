namespace Opsive.UltimateInventorySystem.UI.Item
{
    using System;
    using UnityEngine;
    using UnityEngine.Serialization;

    public class ItemViewSlotDragHandler : MonoBehaviour
    {
        public event Action<ItemViewSlotPointerEventData> OnDragStarted;
        public event Action<ItemViewSlotPointerEventData> OnDragEnded;
        
        [FormerlySerializedAs("m_ItemViewSlotCursor")]
        [FormerlySerializedAs("m_ItemViewSlotMouseCursor")]
        [FormerlySerializedAs("m_ItemViewCursorManager")]
        [FormerlySerializedAs("m_ItemBoxCursorManager")]
        [Tooltip("The Item View cursor manager")]
        [SerializeField] internal ItemViewSlotCursorManager m_ItemViewSlotCursorManager;
        [Tooltip("If true the slots with no items will not be able to be dragged.")]
        [SerializeField] internal bool m_DisableDragOnEmptySlot = true;

        protected ItemViewSlotsContainerBase m_ViewSlotsContainer;
        protected bool m_IsInitialized = false;

        public ItemViewSlotsContainerBase ViewSlotsContainer
        {
            get => m_ViewSlotsContainer;
            set => m_ViewSlotsContainer = value;
        }

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Initialize the grid.
        /// </summary>
        public virtual void Initialize()
        {
            if (m_IsInitialized) { return; }

            if (m_ItemViewSlotCursorManager == null) {
                m_ItemViewSlotCursorManager = GetComponentInParent<ItemViewSlotCursorManager>();
                if (m_ItemViewSlotCursorManager == null) {
                    Debug.LogWarning("The item view cursor manager is missing, please add one on your canvas.");
                }
            }

            m_ViewSlotsContainer = GetComponent<ItemViewSlotsContainerBase>();

            m_ViewSlotsContainer.OnItemViewSlotBeginDragE += HandleItemViewSlotBeginDrag;
            m_ViewSlotsContainer.OnItemViewSlotEndDragE += HandleItemViewSlotEndDrag;
            m_ViewSlotsContainer.OnItemViewSlotDragE += HandleItemViewSlotDrag;

            m_IsInitialized = true;
        }

        protected virtual void HandleItemViewSlotBeginDrag(ItemViewSlotPointerEventData eventData)
        {
            if(m_DisableDragOnEmptySlot && eventData.ItemViewSlot?.ItemInfo.Item == null){ return; }
            m_ItemViewSlotCursorManager.SetSourceItemViewSlot(eventData);
            m_ItemViewSlotCursorManager.SetPosition(eventData.PointerEventData.position);
            OnDragStarted?.Invoke(eventData);
        }

        protected virtual void HandleItemViewSlotDrag(ItemViewSlotPointerEventData eventData)
        {
            if(m_DisableDragOnEmptySlot && eventData.ItemViewSlot?.ItemInfo.Item == null){ return; }
            m_ItemViewSlotCursorManager.AddDeltaPosition(eventData.PointerEventData.delta);
        }

        protected virtual void HandleItemViewSlotEndDrag(ItemViewSlotPointerEventData eventData)
        {
            if(m_DisableDragOnEmptySlot && eventData.ItemViewSlot?.ItemInfo.Item == null){ return; }
            m_ItemViewSlotCursorManager.DragEnded();
            OnDragEnded?.Invoke(eventData);
        }
    }
}