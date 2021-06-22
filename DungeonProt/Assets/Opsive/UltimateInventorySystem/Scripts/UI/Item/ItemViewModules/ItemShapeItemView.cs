/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Item.ItemViewModules
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Grid;
    using UnityEngine;
    using UnityEngine.UI;

    public class ItemShapeItemView : ItemViewModule
    {
        [Tooltip("The icon image.")]
        [SerializeField] protected Image m_Icon;
        [Tooltip("The missing item icon sprite.")]
        [SerializeField] protected Sprite m_MissingIcon;
        [Tooltip("Disable the image component if item is null.")]
        [SerializeField] protected bool m_DisableOnClear;
        [Tooltip("The size of a box")]
        [SerializeField] protected string m_ShapeAttributeName = "Shape";
        [Tooltip("The size of a box")]
        [SerializeField] protected Vector2 m_BoxSize = new Vector2(100,100);

        private ItemShapeGridData m_InventoryItemShapesGridData;
        private int m_Index;

        public ItemShapeGridData InventoryItemShapesGridData => m_InventoryItemShapesGridData;
        public int Index => m_Index;

        public bool IsAnchor => (m_InventoryItemShapesGridData?.GetElementAt(m_Index).IsAnchor ?? false);
        
        /// <summary>
        /// Set the item info.
        /// </summary>
        /// <param name="info">The item info.</param>
        public override void SetValue(ItemInfo info)
        {

            if (info.Item == null || info.Item.IsInitialized == false) {
                Clear();
                return;
            }

            SetIcon(info);
        }

        public void SetIcon(ItemInfo info)
        {
            if (info.Item == null) {
                ClearIcon();
                return;
            }

            m_Icon.enabled = true;
            var iconToSet = m_MissingIcon;
            if (info.Item.TryGetAttributeValue<Sprite>("Icon", out var icon)) {
                iconToSet = icon == null ? m_MissingIcon : icon;
            }

            m_Icon.sprite = iconToSet;

            ResizeToItemShape(info);
        }

        private void ClearIcon()
        {
            m_Icon.sprite = m_MissingIcon;
            if (m_DisableOnClear) { m_Icon.enabled = false; }
            m_Icon.rectTransform.sizeDelta = m_BoxSize;
        }

        protected virtual void ResizeToItemShape(ItemInfo info)
        {
            //Debug.Log("About to resize item "+info);
            
            if (info.Item.TryGetAttributeValue<ItemShape>(m_ShapeAttributeName, out var itemShape) == false) {
                m_Icon.rectTransform.anchoredPosition = Vector2.zero;
                m_Icon.rectTransform.sizeDelta = m_BoxSize;
                return;
            }
            
            //Debug.Log(info+" size: "+itemShape.Size);
            
            m_Icon.rectTransform.anchoredPosition = new Vector2(
                -itemShape.Anchor.x * m_BoxSize.x,
                itemShape.Anchor.y * m_BoxSize.y);

            m_Icon.rectTransform.sizeDelta = new Vector2(m_BoxSize.x*itemShape.Cols, m_BoxSize.y*itemShape.Rows);
        }

        /// <summary>
        /// Clear the component.
        /// </summary>
        public override void Clear()
        {
            //Debug.Log("Clear Iem shape");

            ClearIcon();
            
            m_Index = -1;
            m_InventoryItemShapesGridData = null;
        }
        
        public void SetGridInfo(ItemShapeGridData inventoryItemShapesGridData, int index)
        {
            m_Index = index;
            m_InventoryItemShapesGridData = inventoryItemShapesGridData;

            if (inventoryItemShapesGridData.GetElementAt(index).IsAnchor) {
                return;
            }

            ClearIcon();
        }

        public virtual void DiscreteOffsetImage(Vector2Int offset)
        {
            m_Icon.rectTransform.anchoredPosition += new Vector2(offset.x,-offset.y)*m_BoxSize;
        }
    }
}