namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers
{
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.UI.Item;
    using UnityEngine;

    /// <summary>
    /// The inventory Menu component.
    /// </summary>
    public class ItemViewSlotsContainerPanelBinding : InventoryPanelBinding
    {
        [Tooltip("The inventory grid.")]
        [SerializeField] protected ItemViewSlotsContainerBase m_ItemViewSlotsContainer;
        [Tooltip("Draw on Initialize")]
        [SerializeField] internal bool m_DrawOnInitialize = false;
        [Tooltip("Draw on open")]
        [SerializeField] internal bool m_DrawOnOpen = true;
        [Tooltip("Select Button 0 on Open")]
        [SerializeField] internal bool m_SelectSlotOnOpen = true;

        private IItemViewSlotsContainerController m_ItemViewSlotsContainerController;
        public IItemViewSlotsContainerController ItemViewSlotsContainerController => m_ItemViewSlotsContainerController;
        
        public ItemViewSlotsContainerBase ItemViewSlotsContainer
        {
            get => m_ItemViewSlotsContainer;
            internal set => m_ItemViewSlotsContainer = value;
        }

        public override void Initialize(DisplayPanel display)
        {
            base.Initialize(display);
            
            if (m_DrawOnInitialize) {
                m_ItemViewSlotsContainer.Draw();
                m_ItemViewSlotsContainer.SelectSlot(0);
            }

        }

        protected override void OnInitializeBeforeInventoryBind()
        {
            if (m_ItemViewSlotsContainer == null) {
                Debug.LogError("The item view slots container must NOT be null", gameObject);
                return;
            }
            
            m_ItemViewSlotsContainer.SetDisplayPanel(m_DisplayPanel);
            
            m_ItemViewSlotsContainer.Initialize(false);

            m_ItemViewSlotsContainerController =
                m_ItemViewSlotsContainer.GetComponent<IItemViewSlotsContainerController>();

            var inventoryGridItemActionBinding = m_ItemViewSlotsContainer.GetComponent<ItemViewSlotsContainerItemActionBindingBase>();
            if (inventoryGridItemActionBinding != null) {
                inventoryGridItemActionBinding.CloseItemAction(false);
            }
        }

        protected override void OnInventoryBound()
        {
            if (m_ItemViewSlotsContainerController == null) { return; }

            if (m_Inventory == null) {
                    
                Debug.LogWarning($"Inventory is missing for the ItemViewSlotsContainerController {m_ItemViewSlotsContainerController}.",gameObject);
                return;
            }

            m_ItemViewSlotsContainerController.SetInventory(m_Inventory);
            if (m_SelectSlotOnOpen) {
                m_ItemViewSlotsContainer.SelectSlot(0);
            }
        }

        public override void OnOpen()
        {
            base.OnOpen();
            
            if (m_DrawOnOpen) {
                m_ItemViewSlotsContainer.Draw();
            }
            
            if (m_SelectSlotOnOpen) {
                m_ItemViewSlotsContainer.SelectSlot(0);
            }
        }
    }
}