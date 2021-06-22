namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers
{
    //deprecated ItemViewSlotsContainerPanelBinding is used instead.
    
   /* /// <summary>
    /// The inventory Menu component.
    /// </summary>
    public class InventoryGridPanelBinding : DisplayBinding
    {
        [Tooltip("The inventory grid.")]
        [SerializeField] public ItemViewSlotsContainerInventoryGrid m_InventoryGrid;
        [Tooltip("The inventory to monitor.")]
        [SerializeField] protected Inventory m_Inventory;
        [Tooltip("Draw on Initialize")]
        [SerializeField] private bool m_DrawOnInitialize;

        public Inventory Inventory {
            get => m_Inventory;
            internal set => m_Inventory = value;
        }

        /// <summary>
        /// Open the menu.
        /// </summary>
        public override void Initialize(DisplayPanel display)
        {
            base.Initialize(display);

            if (m_Inventory == null) {
                m_Inventory = GameObject.FindWithTag("Player")?.GetComponent<Inventory>();
                if (m_Inventory == null) {
                    Debug.LogWarning("Inventory is missing in inventory Menu.",gameObject);
                    return;
                }
            }

            if (m_InventoryGrid == null) {
                Debug.LogError("Inventory Grid CANNOT be null",gameObject);
                return;
            }

            m_InventoryGrid.Initialize(false);

            m_InventoryGrid.SetParentPanel(m_Display);

            var inventoryGridItemActionBinding = m_InventoryGrid.GetComponent<ItemViewSlotsContainerItemActionBindingBase>();
            if (inventoryGridItemActionBinding != null) {
                inventoryGridItemActionBinding.CloseItemAction(false);
            }
            
            var itemViewSlotsContainer = m_InventoryGrid.GetComponent<ItemViewSlotsContainerInventoryGrid>();
            if (itemViewSlotsContainer != null) {
                itemViewSlotsContainer.SetInventory(m_Inventory);
            }

            m_InventoryGrid.SetInventory(m_Inventory);

            if (m_DrawOnInitialize) {
                m_InventoryGrid.Draw();
            }

            m_InventoryGrid.SelectSlot(0);
        }
    }*/
}