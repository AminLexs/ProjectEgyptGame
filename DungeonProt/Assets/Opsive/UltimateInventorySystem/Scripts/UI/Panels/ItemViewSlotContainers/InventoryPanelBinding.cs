namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers
{
    using Opsive.Shared.Game;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using UnityEngine;

    public abstract class InventoryPanelBinding : DisplayPanelBinding
    {
        [SerializeField] protected bool m_BindToPanelOwnerInventory;
        [SerializeField] protected Inventory m_Inventory;

        public Inventory Inventory {
            get => m_Inventory;
            internal set => m_Inventory = value;
        }

        public override void Initialize(DisplayPanel display)
        {
            base.Initialize(display);

            OnInitializeBeforeInventoryBind();
            
            BindInventory();
        }

        protected virtual void OnInitializeBeforeInventoryBind()
        { }

        public void BindInventory()
        {
            if (m_Inventory != null) {
                BindInventory(m_Inventory);
                return;
            }

            if (m_BindToPanelOwnerInventory) {
                BindInventory(m_DisplayPanel.Manager.PanelOwner.GetCachedComponent<Core.InventoryCollections.Inventory>());
            }
        }
        
        public void BindInventory(Inventory inventory)
        {
            m_Inventory = inventory;
            OnInventoryBound();
        }

        protected abstract void OnInventoryBound();
    }
}