using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using UnityEngine;

namespace Examples
{
    public class ItemAdderRemover : MonoBehaviour
    {
        [SerializeField] protected int m_Amount = 1;
        [SerializeField] protected ItemDefinition m_Definition;
        [SerializeField] protected Inventory m_Inventory;

        [ContextMenu("Add Item")]
        public void AddItem()
        {
            var item = InventorySystemManager.CreateItem(m_Definition);
            m_Inventory.AddItem((ItemInfo) (m_Amount, item));
        }
        
        [ContextMenu("Remove Item")]
        public void RemoveItem()
        {
            var info = m_Inventory.GetItemInfo(m_Definition);

            if (info.HasValue) {
                m_Inventory.RemoveItem((m_Amount, info.Value));
            }
            
        }
    }
}
