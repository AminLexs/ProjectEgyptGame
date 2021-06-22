/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.VisualElements
{
    using Opsive.UltimateInventorySystem.Crafting;
    using Opsive.UltimateInventorySystem.Editor.Inspectors;
    using Opsive.UltimateInventorySystem.Editor.Utility;
    using Opsive.UltimateInventorySystem.Editor.VisualElements.ViewNames;
    using Opsive.UltimateInventorySystem.Storage;
    using System;
    using System.Collections.Generic;
    using UnityEngine.UIElements;

    /// <summary>
    /// The crafting category field.
    /// </summary>
    public class CraftingCategoryField : InventoryObjectField<CraftingCategory>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="inventorySystemDatabase">The inventory system database.</param>
        /// <param name="mainManagerWindow">The main manager window used to position the popup.</param>
        /// <param name="actions">The actions that can be performed on the selected object.</param>
        /// <param name="preFilterCondition">Use a condition to prefilter the source list.</param>
        public CraftingCategoryField(string label,
            InventorySystemDatabase inventorySystemDatabase,
            IList<(string, Action<CraftingCategory>)> actions,
            Func<CraftingCategory, bool> preFilterCondition)
            : base(label, inventorySystemDatabase, actions, preFilterCondition)
        {
        }

        /// <summary>
        /// Bind the list item.
        /// </summary>
        /// <param name="parent">The parent visual element.</param>
        /// <param name="index">The index.</param>
        protected override void BindItem(VisualElement parent, int index)
        {
            if (index < 0 || index >= m_SearchableList.ItemList.Count) {
                return;
            }
            var viewName = parent.ElementAt(0) as CraftingCategoryViewName;
            var category = m_SearchableList.ItemList[index] as CraftingCategory;
            viewName.Refresh(category);
        }

        /// <summary>
        /// Make the list item.
        /// </summary>
        /// <param name="parent">The parent visual element.</param>
        /// <param name="index">The index.</param>
        protected override void MakeItem(VisualElement parent, int index)
        {
            var viewName = new CraftingCategoryViewName();
            parent.Add(viewName);
        }

        /// <summary>
        /// Return the source of the list.
        /// </summary>
        /// <returns>The list source.</returns>
        protected override IList<CraftingCategory> GetSourceInternal()
        {
            return m_InventorySystemDatabase?.CraftingCategories;
        }

        /// <summary>
        /// Make the field view name.
        /// </summary>
        /// <returns>The new field view name.</returns>
        protected override ViewName<CraftingCategory> MakeFieldViewName()
        {
            return new CraftingCategoryViewName();
        }

        /// <summary>
        /// The sort options.
        /// </summary>
        /// <returns>The sort options.</returns>
        protected override IList<SortOption> GetSortOptions()
        {
            return CraftingCategoryEditorUtility.SortOptions();
        }

        /// <summary>
        /// Filter options for the search list.
        /// </summary>
        /// <param name="list">The list source.</param>
        /// <param name="searchValue">The search value.</param>
        /// <returns>The filtered list.</returns>
        protected override IList<CraftingCategory> FilterOptions(IList<CraftingCategory> list, string searchValue)
        {
            return CraftingCategoryEditorUtility.SearchFilter(list, searchValue);
        }

    }
    
    public class CraftingCategoryReorderableList : InventoryObjectReorderableList< CraftingCategory>
    {

        // <summary>
        public  CraftingCategoryReorderableList(string title, InventorySystemDatabase database, 
            Func< CraftingCategory[]> getObject, Action< CraftingCategory[]> setObjects) : 
            base(title, getObject, setObjects, () => new  CraftingCategoryListElement(database))
        { }

        /// The list element for the tabs.
        /// </summary>
        public class  CraftingCategoryListElement : InventoryObjectListElement< CraftingCategory>
        {
            protected  CraftingCategoryField m_ObjectField;

            /// <summary>
            /// The list element.
            /// </summary>
            public  CraftingCategoryListElement(InventorySystemDatabase database) : base(database)
            {
                m_ObjectField = new  CraftingCategoryField(
                    "",
                    database, new (string, Action<CraftingCategory>)[]
                    {
                        ("Set Category", (x) =>
                        {
                            HandleValueChanged(x);
                        })
                    },
                    (x) => true);
                Add(m_ObjectField);
            }

            /// <summary>
            /// Update the visuals.
            /// </summary>
            /// <param name="value">The new value.</param>
            public override void Refresh( CraftingCategory value)
            {
                m_ObjectField.Refresh(value);
            }
        }
    }
}