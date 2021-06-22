namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers.GridFilterSorters.InventoryGridFilters
{
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Grid;
    using Opsive.UltimateInventorySystem.UI.Item;
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

#if TEXTMESH_PRO_PRESENT

#endif

    public class InventorySearchFilter : ItemInfoFilterBase
    {
        [SerializeField] internal InventoryGrid m_InventoryGrid;

        [Tooltip("The item collections that should not be drawn.")]
#if TEXTMESH_PRO_PRESENT
        [SerializeField] protected TMP_InputField m_InputField;
#else
        [SerializeField] protected InputField m_InputField;
#endif

        [SerializeField] internal ItemInfoFilterSorterBase m_BindSorterWhileSearching;

        protected IFilterSorter<ItemInfo> m_PreviousBoundSorter;
        protected bool m_Searching = false;
        //protected InventoryGridIndexer m_GridIndexer;
        private bool defaultUseIndexerValue;
        
        public InventoryGrid InventoryGrid => m_InventoryGrid;
        
        protected void Awake()
        {
            m_InputField.onValueChanged.AddListener(HandleOnTextChange);

            if (m_InventoryGrid == null) {
                m_InventoryGrid = GetComponent<InventoryGrid>();
                if (m_InventoryGrid == null) {
                    Debug.LogError("The inventory grid search filter is missing an inventory grid.",gameObject);
                    return;
                }
            }
            
            defaultUseIndexerValue = m_InventoryGrid.UseGridIndex;
        }

        private void HandleOnTextChange(string newText)
        {
            if (m_Searching && string.IsNullOrEmpty(newText)) {
                m_Searching = false;
                if ((IFilterSorter<ItemInfo>)this == m_PreviousBoundSorter) {
                    m_InventoryGrid.BindGridFilterSorter(null);
                } else {
                    m_InventoryGrid.BindGridFilterSorter(m_PreviousBoundSorter);
                }

                m_InventoryGrid.UseGridIndex = defaultUseIndexerValue;
                //m_InventoryGrid.InventoryGridIndexer.Copy(m_GridIndexer);
            } else if(m_Searching == false && string.IsNullOrEmpty(newText) == false){
                m_Searching = true;
                m_PreviousBoundSorter = m_InventoryGrid.BindGridFilterSorter(this);
                //m_GridIndexer.Copy(m_InventoryGrid.InventoryGridIndexer);
                m_InventoryGrid.UseGridIndex = false;
            }

            m_InventoryGrid.Draw();
        }

        public override ListSlice<ItemInfo> Filter(ListSlice<ItemInfo> input, ref ItemInfo[] outputPooledArray)
        {
            input = base.Filter(input, ref outputPooledArray);

            if (m_BindSorterWhileSearching != null) {
                input = m_BindSorterWhileSearching.Filter(input, ref outputPooledArray);
            }

            return input;
        }

        public override bool Filter(ItemInfo itemInfo)
        {
            var search = m_InputField.text;
            var itemName = itemInfo.Item.name;

            return itemName.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }
    }
}