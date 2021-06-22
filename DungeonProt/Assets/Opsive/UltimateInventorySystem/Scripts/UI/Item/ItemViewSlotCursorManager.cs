namespace Opsive.UltimateInventorySystem.UI.Item
{
    using Opsive.Shared.Game;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
    using Opsive.UltimateInventorySystem.UI.Views;
    using UnityEngine;

    public class ItemViewSlotCursorManager : MonoBehaviour
    {
        [Tooltip("The canvas where this grid exist.")]
        [SerializeField] protected Canvas m_Canvas;
        [Tooltip("The position offset always added on the position")]
        [SerializeField] protected Vector2 m_PositionOffset;
        
        protected bool m_IsInitialized = false;

        protected ItemViewSlotsContainerBase m_SourceContainer;
        protected ItemViewSlot m_SourceItemViewSlot;
        protected ItemView m_FloatingItemView;
        protected bool m_IsMoving = false;
        
        protected ItemViewSlotEventData m_PointerSlotEventData;
        
        public bool IsMovingItemView => m_IsMoving;
        public ItemView FloatingItemView => m_FloatingItemView;
        public ItemViewSlot SourceItemViewSlot => m_SourceItemViewSlot;
        
        public ItemViewSlotEventData PointerSlotEventData => m_PointerSlotEventData;

        public Vector2 PositionOffset
        {
            get => m_PositionOffset;
            set => m_PositionOffset = value;
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

            if (m_Canvas == null) { m_Canvas = GetComponentInParent<Canvas>(); }
            
            m_IsInitialized = true;
        }

        public void SetSourceItemViewSlot(ItemViewSlotEventData slotEventData)
        {
            m_PointerSlotEventData = slotEventData;
            SetSourceItemViewSlot(slotEventData.ItemViewSlot, slotEventData.ItemViewSlotsContainer);
        }

        protected void SetSourceItemViewSlot(ItemViewSlot itemViewSlot, ItemViewSlotsContainerBase viewSlotsContainer)
        {
            m_SourceItemViewSlot = itemViewSlot;
            m_SourceContainer = viewSlotsContainer;

            SetItemViewAsMovingSource(m_SourceItemViewSlot, true);

            if (viewSlotsContainer != null) {
                m_FloatingItemView = SetMovingItemView(viewSlotsContainer, m_SourceItemViewSlot.ItemInfo);
                /*if (m_PointerEventData != null) {
                    m_PointerEventData.MovingItemBox = SetMovingItemBox(viewSlotsContainer, m_SourceItemBox.CurrentValue);
                }*/
            }

        }

        public ItemView SetMovingItemView(ItemViewSlotsContainerBase viewSlotsContainer, ItemInfo element)
        {
            var itemViewPrefab = viewSlotsContainer.GetBoxPrefabFor(element);
            if (itemViewPrefab == null) {
                Debug.LogWarning("View Drawer View Prefab is null.");
                return null;
            }

            var boxObject = ObjectPool.Instantiate(itemViewPrefab, m_Canvas.transform).GetComponent<ItemView>();

            var size = boxObject.RectTransform.rect.size;
            
            boxObject.RectTransform.anchorMin = Vector2.zero;
            boxObject.RectTransform.anchorMax = Vector2.zero;
            
            boxObject.RectTransform.sizeDelta = size;
            
            boxObject.SetValue(element);
            SetItemViewAsMoving(boxObject);

            m_FloatingItemView = boxObject;
            m_IsMoving = true;
            
            return boxObject;
        }

        public void SetPosition(Vector2 newPosition)
        {
            m_FloatingItemView.RectTransform.anchoredPosition = ( newPosition + PositionOffset )/ m_Canvas.scaleFactor;
        }
        
        public void AddDeltaPosition(Vector2 deltaPosition, bool autoScaleFactor = true)
        {
            if (autoScaleFactor) {
                m_FloatingItemView.RectTransform.anchoredPosition += deltaPosition / m_Canvas.scaleFactor;
            } else {
                m_FloatingItemView.RectTransform.anchoredPosition += deltaPosition;
            }
        }

        public void BeforeDrop()
        {
            m_IsMoving = false;
        }

        public void RemoveItemView()
        {
            if (m_SourceItemViewSlot != null) {
                SetItemViewAsMovingSource(m_SourceItemViewSlot, false);
                m_SourceItemViewSlot = null;
            }
            
            m_PointerSlotEventData = null;

            if (m_FloatingItemView != null) {
                ObjectPool.Destroy(m_FloatingItemView.gameObject);
                m_FloatingItemView = null;
            }
            m_IsMoving = false;
            
        }
        
        public virtual void SetItemViewAsMoving(ItemView itemView)
        {
            if (itemView.CanvasGroup == null) {
                Debug.LogWarning("Draggable Item Viewes MUST have a CanvasGroup component to be dropped.");
            } else {
                itemView.CanvasGroup.interactable = false;
                itemView.CanvasGroup.blocksRaycasts = false;
            }

            for (int i = 0; i < itemView.Modules.Count; i++) {
                if (itemView.Modules[i] is IViewModuleMovable movable) {
                    movable.SetAsMoving();
                }
            }
        }
        
        public virtual void SetItemViewAsMovingSource(ItemViewSlot itemViewSlot, bool movingSource)
        {
            for (int i = 0; i < itemViewSlot.ItemView.Modules.Count; i++) {
                if (itemViewSlot.ItemView.Modules[i] is IViewModuleMovable movable) {
                    movable.SetAsMovingSource(movingSource);
                }
            }
        }

        public void DragEnded()
        {
            RemoveItemView();
            if (m_SourceContainer != null) {
                m_SourceContainer.Draw();
            }
        }
    }
}