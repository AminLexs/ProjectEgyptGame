namespace Opsive.UltimateInventorySystem.UI.Item.ItemViewSlotRestrictions
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using UnityEngine;

    public abstract class ItemViewSlotRestriction : MonoBehaviour
    {
        public abstract bool CanContain(ItemInfo itemInfo);

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}