namespace Opsive.UltimateInventorySystem.UI.Item
{
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;

    public interface IItemViewSlotsContainerController
    {
        ItemViewSlotsContainerBase ItemViewSlotsContainer { get; }
        Inventory Inventory { get; }
        void SetInventory(Inventory inventory);
    }
}