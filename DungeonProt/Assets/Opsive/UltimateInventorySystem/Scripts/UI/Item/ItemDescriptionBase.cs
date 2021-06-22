/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Item
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
    using UnityEngine;

    /// <summary>
    /// The base class for the item description component.
    /// </summary>
    public abstract class ItemDescriptionBase : ItemView
    {

        /// <summary>
        /// Initialize.
        /// </summary>
        protected virtual void Awake()
        {
            Initialize(false);
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public virtual void Initialize()
        {
            Initialize(false);
        }
        
        /// <summary>
        /// Initialize.
        /// </summary>
        protected override void Initialize(bool force)
        {
            if (!force && m_Initialized) { return; }
            base.Initialize(false);
            Clear();
        }

        /// <summary>
        /// Set the item.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        public override void SetValue(ItemInfo itemInfo)
        {
            Initialize(false);
            base.SetValue(itemInfo);
            
            if (ItemInfo.Item == null) {
                Clear();
                return;
            }
            
            OnSetValue();
        }

        /// <summary>
        /// Draw the description.
        /// </summary>
        protected abstract void OnSetValue();

        /// <summary>
        /// Draw the description when the item is missing.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            OnClear();
        }

        /// <summary>
        /// Draw the description when the item is missing.
        /// </summary>
        protected abstract void OnClear();

    }
}