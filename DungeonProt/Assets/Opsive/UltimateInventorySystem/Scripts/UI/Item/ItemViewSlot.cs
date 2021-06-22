namespace Opsive.UltimateInventorySystem.UI.Item
{
    using Opsive.Shared.Game;
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.CompoundElements;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewSlotRestrictions;
    using Opsive.UltimateInventorySystem.UI.Views;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    /// <summary>
    /// A hot bar item slot UI.
    /// </summary>
    public class ItemViewSlot : ActionButton, IViewSlot
    {
        [Tooltip("The index within the hot item bar.")]
        [SerializeField] internal ItemView m_ItemView;
        [Tooltip("The slot index.")]
        [SerializeField] protected int m_Index;
        //[Tooltip("The index within the hot item bar.")]
        /*[SerializeField]*/ protected ItemViewSlotRestriction[] m_ItemViewSlotRestrictions;

        public int Index => m_Index;
        public ItemInfo ItemInfo => m_ItemView?.CurrentValue ?? ItemInfo.None;
        public ItemView ItemView
        {
            get => m_ItemView;
        }

        public View View => m_ItemView;

        protected override void Awake()
        {
            base.Awake();
            if (m_ItemView == null) {
                m_ItemView = GetComponentInChildren<ItemView>(true);
            }

            SetItemView(m_ItemView);
            m_ItemViewSlotRestrictions = GetComponents<ItemViewSlotRestriction>();
        }

        /// <summary>
        /// Assign the index to this element.
        /// </summary>
        /// <param name="index">The index.</param>
        public virtual void AssignIndex(int index)
        {
            m_Index = index;
            UpdateUI();
        }

        /// <summary>
        /// Set the item info.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        public virtual void SetItemInfo(ItemInfo itemInfo)
        {
            if (itemInfo.Item == null || RandomID.IsIDEmpty(itemInfo.Item.ID)) {
                m_ItemView.Clear();
                UpdateUI();
                return;
            }
            m_ItemView.SetValue(itemInfo);
            UpdateUI();
        }

        public virtual void SetItemView(ItemView itemView)
        {
            m_ItemView = itemView;
            targetGraphic = m_ItemView != null ? m_ItemView.TargetGraphic : null;
            
            if(m_ItemView == null){return;}
            
            m_ItemView.SetViewSlot(this);
            
            if (m_Selected) {
                m_ItemView.Select(true);
            }
        }
        
        /// <summary>
        /// Invoke OnSelect even if it was already selected.
        /// </summary>
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            
            if (m_ItemView != null) {
                m_ItemView.Select(true);
            }
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            if (m_ItemView != null) {
                m_ItemView.Select(false);
            }
        }


        /// <summary>
        /// Set the box child.
        /// </summary>
        /// <param name="child">The child box.</param>
        public void SetView(View child)
        {
            SetItemView(child as ItemView);
        }

        public void DisableImage()
        {
            if (image != null) {
                image.enabled = false;
                return;
            }

            image = gameObject.GetCachedComponent<Image>();
            if (image != null) {
                image.enabled = false;
            }

        }

        /// <summary>
        /// Update the ui.
        /// </summary>
        public virtual void UpdateUI()
        {
            if(m_ItemView == null){ return; }
            
            m_ItemView.SetViewSlot(this);
        }

        /// <summary>
        /// Select the Item View when highlighting with the mouse.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            
            if (eventData.IsPointerMoving() == false) { return; }

            m_ItemView.Select(true);
        }

        /// <summary>
        /// Deselect the Item View when stop highlighting with the mouse.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            m_ItemView.Select(false);
        }

        /// <summary>
        /// use the item when the hot bar item slot is clicked.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            m_ItemView.Click();
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            m_ItemView.Click();
        }

        public bool CanContain(ItemInfo itemInfo)
        {
            for (int i = 0; i < m_ItemViewSlotRestrictions.Length; i++) {
                if (m_ItemViewSlotRestrictions[i].CanContain(itemInfo) == false) { return false; }
            }

            return true;
        }
    }
}