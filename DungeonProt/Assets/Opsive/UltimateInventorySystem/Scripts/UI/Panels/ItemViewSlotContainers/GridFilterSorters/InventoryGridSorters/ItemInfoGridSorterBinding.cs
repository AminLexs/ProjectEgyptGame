namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers.GridFilterSorters.InventoryGridSorters
{
    using Opsive.UltimateInventorySystem.UI.Grid;
    using UnityEngine;
    using UnityEngine.Serialization;

    public class InventoryGridSorterBinding : MonoBehaviour
    {
        [SerializeField] protected ItemInfoGrid m_ItemInfoGrid;
        [FormerlySerializedAs("m_ItemInfoGridSorter")] [FormerlySerializedAs("m_InventoryGridSorter")] [SerializeField] protected ItemInfoSorterBase m_ItemInfoSorter;
        [SerializeField] protected bool m_BindOnStart;
        
        protected virtual void Start()
        {
            if (m_ItemInfoGrid == null) {
                m_ItemInfoGrid = GetComponent<ItemInfoGrid>();
            }

            if (m_ItemInfoSorter == null) {
                m_ItemInfoSorter = GetComponent<ItemInfoSorterBase>();
            }

            if (m_BindOnStart) { Bind(); }
        }

        public void Bind()
        {
            m_ItemInfoGrid.BindGridFilterSorter(m_ItemInfoSorter);
        }
        
        public void UnBind()
        {
            m_ItemInfoGrid.BindGridFilterSorter(null);
        }
    }
}