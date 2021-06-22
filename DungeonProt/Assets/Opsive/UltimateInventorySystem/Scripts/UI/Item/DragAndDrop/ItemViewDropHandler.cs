namespace Opsive.UltimateInventorySystem.UI.Item.DragAndDrop
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
    using UnityEngine;
    using UnityEngine.Serialization;

    public interface IItemViewSlotDropHoverSelectable
    {
        void SelectWith(ItemViewDropHandler dropHandler);
        void DeselectWith(ItemViewDropHandler dropHandler);
    }
    
    public class ItemViewSlotDropHandlerStreamData
    {
        public ItemViewSlotsContainerBase SourceContainer { get; set; }
        public ItemViewSlot SourceItemViewSlot { get; set; }
        public ItemInfo SourceItemInfo { get; set; }
        public int SourceIndex { get; set; }
        
        public ItemViewSlotsContainerBase DestinationContainer { get; set; }
        public ItemViewSlot DestinationItemViewSlot { get; set; }
        public ItemInfo DestinationItemInfo { get; set; }
        public int DestinationIndex { get; set; }

        public ItemViewSlotDropHandlerStreamData()
        {
        }

        public virtual void Reset(ItemViewSlot sourceItemViewSlot, ItemViewSlotEventData dragSlotEventData, ItemViewSlotEventData dropSlotEventData)
        {
            SourceContainer = dragSlotEventData.ItemViewSlotsContainer;
            SourceItemViewSlot = sourceItemViewSlot;
            SourceItemInfo = dragSlotEventData.ItemView.CurrentValue;
            SourceIndex = dragSlotEventData.Index;
            
            DestinationContainer = dropSlotEventData.ItemViewSlotsContainer;
            DestinationItemViewSlot = dropSlotEventData.ItemViewSlot;
            DestinationItemInfo = dropSlotEventData.ItemView.CurrentValue;
            DestinationIndex = dropSlotEventData.Index;
        }
    }

    public class ItemViewDropHandler : MonoBehaviour
    {
        [SerializeField] protected int m_DebugPassedConditionIndex;

        [Tooltip("The item view cursor manager")]
        [SerializeField] protected ItemViewSlotCursorManager m_ItemViewSlotCursorManager;
        [Tooltip("Only used if the ItemBoxDropActionsWithConditions is null")]
        [SerializeField] protected bool m_MoveIndexIfSameContainer;
        [Tooltip("Only used if the ItemBoxDropActionsWithConditions is null")]
        [SerializeField] protected bool m_AddToDestinationContainer;
        [Tooltip("Only used if the ItemBoxDropActionsWithConditions is null")]
        [SerializeField] protected bool m_RemoveFromSourceContainer;
        
        [SerializeField] internal ItemViewSlotDropActionSet m_ItemViewSlotDropActionSet;
        
        protected ItemViewSlotsContainerBase m_ViewSlotsContainer;
        protected bool m_IsInitialized = false;
        protected ItemViewSlotDropHandlerStreamData m_StreamData;
        protected ItemViewSlotEventData m_DropSlotEventData;

        public ItemViewSlotCursorManager SlotCursorManager => m_ItemViewSlotCursorManager;
        public ItemViewSlotDropHandlerStreamData StreamData => m_StreamData;

        public ItemViewSlotsContainerBase SourceContainer => m_ItemViewSlotCursorManager.PointerSlotEventData.ItemViewSlotsContainer;

        public ItemViewSlot SourceItemViewSlot => m_ItemViewSlotCursorManager.SourceItemViewSlot;

        public ItemInfo SourceItemInfo => m_ItemViewSlotCursorManager.SourceItemViewSlot.ItemInfo;

        public int SourceIndex => m_ItemViewSlotCursorManager.PointerSlotEventData.Index;

        public ItemViewSlotEventData DropSlotEventData => m_DropSlotEventData;

        public ItemView DestinationItemView => m_DropSlotEventData?.ItemView;

        public ItemInfo DestinationItemInfo => DestinationItemView?.CurrentValue ?? (0,null,null);

        public ItemViewSlotsContainerBase DestinationContainer => m_DropSlotEventData?.ItemViewSlotsContainer;

        public int DestinationIndex => m_DropSlotEventData?.Index ?? -1;

        public ItemViewSlotDropActionSet ItemViewSlotDropActionSet
        {
            get => m_ItemViewSlotDropActionSet;
            set => m_ItemViewSlotDropActionSet = value;
        }

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

            if (m_ItemViewSlotDropActionSet != null) {
                m_ItemViewSlotDropActionSet.Initialize(false);
            }

            m_ViewSlotsContainer = GetComponent<ItemViewSlotsContainerBase>();

            m_ViewSlotsContainer.OnItemViewSlotDropE += HandleItemViewSlotDrop;
            
            m_ViewSlotsContainer.OnItemViewSlotSelected += ItemViewSlotSelected;
            m_ViewSlotsContainer.OnItemViewSlotDeselected += ItemViewSlotDeselected;
            
            m_StreamData = new ItemViewSlotDropHandlerStreamData();

            m_IsInitialized = true;
        }

        public void HandleItemViewSlotDrop(ItemViewSlotEventData dropSlotEventData)
        {
            m_DropSlotEventData = dropSlotEventData;
            
            var sourceItemViewSlot = m_ItemViewSlotCursorManager.SourceItemViewSlot;
            var dragEventData = m_ItemViewSlotCursorManager.PointerSlotEventData;

            if (dragEventData == null) {
                Debug.LogWarning("dragEventData == null");
                return;
            }

            m_StreamData.Reset(sourceItemViewSlot, dragEventData, dropSlotEventData);

            m_ItemViewSlotCursorManager.BeforeDrop();
            HandleItemViewSlotDropInternal();
            m_ItemViewSlotCursorManager.RemoveItemView();
        }

        protected virtual void HandleItemViewSlotDropInternal()
        {

            if (m_ItemViewSlotDropActionSet != null) {
                m_ItemViewSlotDropActionSet.HandleItemViewSlotDrop(this);
                SourceContainer.Draw();
                DestinationContainer.Draw();
                return;
            }

            // The Container is the same
            if (SourceContainer == DestinationContainer) {
                if (m_MoveIndexIfSameContainer) {
                    SourceContainer.MoveItem(SourceIndex, DestinationIndex);
                    SourceContainer.Draw();
                }
                
                return;
            }
            
            // The container is not the same
            var itemToRemove = SourceItemInfo;
            var itemToAdd = SourceItemInfo;
            
            if (m_RemoveFromSourceContainer) {
                itemToAdd = SourceContainer.RemoveItem(itemToRemove, SourceIndex);
            }
            
            if (m_AddToDestinationContainer) {
                DestinationContainer.AddItem(itemToAdd, DestinationIndex);
            }
            
            SourceContainer.Draw();
            DestinationContainer.Draw();
        }
        
        private void ItemViewSlotSelected(ItemViewSlotEventData eventdata)
        {
            if (m_ItemViewSlotCursorManager.IsMovingItemView == false) { return; }
            
            
            m_DropSlotEventData = eventdata;
            m_StreamData.Reset(m_ItemViewSlotCursorManager.SourceItemViewSlot, m_ItemViewSlotCursorManager.PointerSlotEventData, eventdata);

            m_DebugPassedConditionIndex = m_ItemViewSlotDropActionSet.GetFirstPassingConditionIndex(this);

            var selectedItemView = eventdata.ItemView;
            
            // Select the item view.
            for (int i = 0; i < selectedItemView.Modules.Count; i++) {
                if (selectedItemView.Modules[i] is IItemViewSlotDropHoverSelectable module) {
                    module.SelectWith(this);
                }
            }
        }
        
        private void ItemViewSlotDeselected(ItemViewSlotEventData slotEventData)
        {
            var itemView = slotEventData.ItemView; 
            if (m_ItemViewSlotCursorManager.IsMovingItemView == false) { return; }

            for (int i = 0; i < itemView.Modules.Count; i++) {
                if (itemView.Modules[i] is IItemViewSlotDropHoverSelectable module) {
                    module.DeselectWith(this);
                }
            }
        }
    }
}