namespace Opsive.UltimateInventorySystem.Core
{
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.Core.Registers;

    public class InventoryIdentifierRegister : InventoryObjectIDOnlyRegister<InventoryIdentifier>
    {
        public InventoryIdentifierRegister(InventorySystemRegister register) : base(register)
        { }
    }
}