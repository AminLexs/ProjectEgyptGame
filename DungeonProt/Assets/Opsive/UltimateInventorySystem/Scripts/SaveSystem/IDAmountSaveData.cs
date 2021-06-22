/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.SaveSystem
{
    /// <summary>
    /// A struct of an ID and an amount, used to save amounts of an object with an ID.
    /// </summary>
    public struct IDAmountSaveData
    {
        public uint ID;
        public int Amount;

        public IDAmountSaveData(uint id, int amount)
        {
            ID = id;
            Amount = amount;
        }
    }
}