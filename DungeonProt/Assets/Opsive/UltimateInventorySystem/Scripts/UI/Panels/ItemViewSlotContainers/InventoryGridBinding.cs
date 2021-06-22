/*namespace Opsive.UltimateInventorySystem.UI.Panels.Inventory
{
    using Opsive.UltimateInventorySystem.UI.Grid;

    public abstract class InventoryGridBinding : GridBinding
    {

        protected InventoryGrid m_InventoryGrid;
       
        public override void Bind(GridBase grid)
        {
            Initialize(false);
            if(m_Grid == grid){ return; }

            UnBind();

            if (grid is InventoryGrid inventoryGrid) {
                m_InventoryGrid = inventoryGrid;
                m_Grid = grid;
                OnBindInventoryGrid(inventoryGrid);
            }
        }

        protected abstract void OnBindInventoryGrid(InventoryGrid inventoryGrid);

        public override void UnBind()
        {
            Initialize(false);
            if(m_Grid == null){ return; }
            
            OnUnBindInventoryGrid();
            m_Grid = null;
            m_InventoryGrid = null;
        }

        protected abstract void OnUnBindInventoryGrid();
    }
}*/