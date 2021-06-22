/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.VisualElements.ControlTypes
{
    using Opsive.Shared.Editor.UIElements.Controls;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Storage;
    using System;
    using UnityEngine.UIElements;

    /// <summary>
    /// Implements TypeControlBase for the ObjectAmounts ControlType.
    /// </summary>
    [ControlType(typeof(ItemCategoryAmounts))]
    public class ItemCategoryAmountsControl : InventoryObjectAmountsControl
    {
        /// <summary>
        /// Create the ObjectAmountsView.
        /// </summary>
        /// <param name="value">The default value.</param>
        /// <param name="onChangeEvent">The on change event function.</param>
        /// <returns>The ObjectAmountsView.</returns>
        public override VisualElement CreateObjectAmountsView(object value, Func<object, bool> onChangeEvent)
        {
            return new ItemCategoryAmountsView(value as ItemCategoryAmounts, m_Database, onChangeEvent);
        }
    }

    /// <summary>
    /// Object Amounts view from ObjectAmountsBasView
    /// </summary>
    public class ItemCategoryAmountsView : InventoryObjectAmountsView<ItemCategoryAmounts, ItemCategoryAmount, ItemCategory>
    {
        /// <summary>
        /// Requires base constructor.
        /// </summary>
        /// <param name="objectAmounts">The object amounts.</param>
        /// <param name="database">The inventory system database.</param>
        /// <param name="onChangeEvent">The onChangeEvent function.</param>
        public ItemCategoryAmountsView(ItemCategoryAmounts objectAmounts, InventorySystemDatabase database, Func<object, bool> onChangeEvent) :
            base("Item Category Amounts", objectAmounts, database, onChangeEvent)
        {
        }

        /// <summary>
        /// Create the ObjectAmountView.
        /// </summary>
        /// <returns>The ObjectAmountView.</returns>
        protected override InventoryObjectAmountView<ItemCategory> CreateObjectAmountView()
        {
            return new ItemCategoryAmountView(m_Database);
        }

        /// <summary>
        /// Create a default ObjectAmount.
        /// </summary>
        /// <returns>The default ObjectAmount.</returns>
        protected override ItemCategoryAmount CreateObjectAmount()
        {
            return new ItemCategoryAmount();
        }

        /// <summary>
        /// Create a default ObjectAmounts
        /// </summary>
        /// <returns>The default ObjectAmounts.</returns>
        protected override ItemCategoryAmounts CreateObjectAmounts()
        {
            return new ItemCategoryAmounts();
        }
    }

    /// <summary>
    /// ObjectAmounts View from ObjectAmountBaseView
    /// </summary>
    public class ItemCategoryAmountView : InventoryObjectAmountView<ItemCategory>
    {
        protected ItemCategoryField m_ItemCategoryField;

        protected override ItemCategory ObjectFieldValue {
            get => m_ItemCategoryField?.Value;
            set => m_ItemCategoryField.Refresh(value);
        }

        /// <summary>
        /// The item category amount view.
        /// </summary>
        /// <param name="database">The database.</param>
        public ItemCategoryAmountView(InventorySystemDatabase database) : base(database)
        {
            m_ItemCategoryField = new ItemCategoryField("",
                m_Database,
                new (string, Action<ItemCategory>)[]
            {
                ("Set ItemCategory", (x) =>InvokeOnValueChanged(CreateNewObjectAmount(x,m_IntegerField.value)))
            }, (x) => true);
            Add(m_ItemCategoryField);
            m_ItemCategoryField.Refresh();
        }

        /// <summary>
        /// Refresh the object Icon.
        /// </summary>
        public override void RefreshInternal()
        {
            m_ItemCategoryField.Refresh(m_ObjectAmount.Object);
        }

        /// <summary>
        /// Create an objectAmount.
        /// </summary>
        /// <param name="obj">The new Object.</param>
        /// <param name="amount">The new Amount.</param>
        /// <returns>The ObjectAmount.</returns>
        public override IObjectAmount<ItemCategory> CreateNewObjectAmount(object obj, int amount)
        {
            return new ItemCategoryAmount((ItemCategory)obj, amount);
        }

        /// <summary>
        /// Set if the Amount View is interactable
        /// </summary>
        /// <param name="interactable">true if interactable.</param>
        public override void SetInteractable(bool interactable)
        {
            m_Interactable = interactable;

            m_IntegerField.SetEnabled(m_Interactable);
            m_ItemCategoryField.SetInteractable(m_Interactable);
        }

        /// <summary>
        /// Show the Type in the name
        /// </summary>
        /// <param name="showType">Show or hide the type.</param>
        public override void SetShowType(bool showType)
        {
            m_ShowType = showType;
            m_ItemCategoryField.ViewName.SetShowType(showType);
        }
    }
}