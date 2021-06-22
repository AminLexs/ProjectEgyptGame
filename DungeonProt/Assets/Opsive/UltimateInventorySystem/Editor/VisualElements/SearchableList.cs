/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.VisualElements
{
    using Opsive.Shared.Editor.UIElements;
    using Opsive.UltimateInventorySystem.Editor.Styles;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;

    /// <summary>
    /// Searchable List allowing for searching and sorting a ReorderableList.
    /// </summary>
    public class SearchableList<T> : VisualElement where T : class
    {
        protected IList<T> m_ItemSource;
        protected List<T> m_ItemSourceCopy;
        protected T m_PreviousSelectedObject;
        protected string m_PreviousSearch;
        protected SortOption m_PreviousSortOption;
        private Action<int> m_OnSelection;

        protected ToolbarSearchField m_SearchField;
        protected SortOptionsDropDown m_SortOptionsDropDown;
        protected ReorderableList m_ReorderableList;

        protected Func<IList<T>, string, IList<T>> m_SearchFilter;

        public List<T> ItemList => m_ItemSourceCopy;

        public bool selectOnRefresh = true;

        public int SelectedIndex {
            get => m_ReorderableList.SelectedIndex;
            set => m_ReorderableList.SelectedIndex = value;
        }

        public T SelectedObject {
            get => SelectedIndex < 0 || SelectedIndex >= m_ItemSourceCopy.Count ? null : m_ItemSourceCopy[SelectedIndex];
            set => SelectObject(value);
        }

        /// <summary>
        /// SearchableList constructor.
        /// </summary>
        /// <param name="itemsSource">The list of items.</param>
        /// <param name="makeItem">The callback when each element is being created.</param>
        /// <param name="bindItem">the callback when the element is binding to the data.</param>
        /// <param name="header">The list header (can be null).</param>
        /// <param name="onSelection">The callback when an element is selected (can be null).</param>
        /// <param name="onAdd">The callback when the add button is pressed (can be null).</param>
        /// <param name="onRemove">The callback when the remove button is pressed (can be null).</param>
        /// <param name="onReorder">The callback when elements are reordered.</param>
        /// <param name="sortOptions">Options allowing for sorting.</param>
        /// <param name="searchFilter">The filter function.</param>
        public SearchableList(IList<T> itemsSource, Action<VisualElement, int> makeItem,
            Action<VisualElement, int> bindItem, Action<VisualElement> header,
            Action<int> onSelection, Action onAdd, Action<int> onRemove,
            Action<int, int> onReorder,
            IList<SortOption> sortOptions,
            Func<IList<T>, string, IList<T>> searchFilter)
        {
            m_SearchFilter = searchFilter;
            m_ItemSource = itemsSource;
            m_ItemSourceCopy = new List<T>();
            if (m_ItemSource != null) {
                for (int i = 0; i < m_ItemSource.Count; i++) { m_ItemSourceCopy.Add(m_ItemSource[i]); }
            }

            m_OnSelection = onSelection;

            AddToClassList(CommonStyles.s_SearchList);

            var searchSortContainer = new VisualElement();
            searchSortContainer.AddToClassList(CommonStyles.s_SearchList_SearchSortContainer);
            m_SearchField = new ToolbarSearchField();
            m_SearchField.style.flexShrink = 1;
            searchSortContainer.Add(m_SearchField);
            m_SearchField.RegisterValueChangedCallback(evt =>
            {
                SearchAndSort(evt.newValue);
            });

            m_SortOptionsDropDown = new SortOptionsDropDown(sortOptions);
            m_SortOptionsDropDown.RegisterValueChangedCallback(evt =>
            {
                SortList();
                SearchSortRefresh(m_PreviousSearch);
            });
            searchSortContainer.Add(m_SortOptionsDropDown);

            m_ReorderableList = new ReorderableList(m_ItemSourceCopy, makeItem, bindItem, header,
                index =>
                {
                    m_OnSelection?.Invoke(index);
                    m_PreviousSelectedObject = (index < 0 || index >= m_ItemSourceCopy.Count) ? null : m_ItemSourceCopy[index];
                }, onAdd, onRemove, onReorder);
            m_ReorderableList.Refresh(m_ItemSourceCopy);
            Add(m_ReorderableList);

            m_ReorderableList.Add(searchSortContainer);
            searchSortContainer.PlaceBehind(m_ReorderableList.Q("body"));

            Refresh();
        }

        /// <summary>
        /// Clears the search and refreshes.
        /// </summary>
        public void ClearSearch()
        {
            m_SearchField.SetValueWithoutNotify((string)null);
            SearchAndSort(null);
        }

        /// <summary>
        /// Search and sort, the order is important since we only sort the filtered list.
        /// </summary>
        /// <param name="searchString">The search text.</param>
        public void SearchAndSort(string searchString)
        {
            m_ItemSourceCopy.Clear();
            Search(searchString);
            SortList();

            SearchSortRefresh(searchString);
        }

        /// <summary>
        /// Refresh but also select the object that was selected before the search sort.
        /// </summary>
        /// <param name="searchString">The search text.</param>
        private void SearchSortRefresh(string searchString)
        {
            var previousSelectedObject = m_PreviousSelectedObject;
            var previousSelectedIndex = m_ReorderableList.SelectedIndex;

            if (selectOnRefresh == false) { m_ReorderableList.SelectedIndex = -1; }

            m_ReorderableList.Refresh(m_ItemSourceCopy);

            if (selectOnRefresh) {
                //Select the object that was selected before the search change if possible.
                if (m_PreviousSearch != searchString || m_PreviousSortOption != m_SortOptionsDropDown.CurrentOption) {
                    if (m_ItemSourceCopy.Contains(previousSelectedObject)) {
                        SelectObject(previousSelectedObject);
                    } else if (m_ItemSourceCopy != null && previousSelectedIndex != -1) {

                        if (m_ItemSourceCopy.Count <= 0) {
                            m_ReorderableList.SelectedIndex = -1;
                            m_OnSelection?.Invoke(-1);
                        } else if (previousSelectedObject != m_ItemSourceCopy[0]) {
                            m_ReorderableList.SelectedIndex = -1;
                            m_OnSelection?.Invoke(-1);
                            m_PreviousSelectedObject = m_ItemSourceCopy[0];
                            m_ReorderableList.SelectedIndex = 0;
                        }
                    }
                } else if (previousSelectedIndex >= m_ItemSourceCopy.Count) {
                    m_ReorderableList.SelectedIndex = m_ItemSourceCopy.Count - 1;
                }
            }

            m_PreviousSearch = searchString;
            m_PreviousSortOption = m_SortOptionsDropDown.CurrentOption;
        }

        /// <summary>
        /// Searches the list.
        /// </summary>
        /// <param name="searchString">The search text.</param>
        protected void Search(string searchString)
        {
            SearchFilter(searchString, m_ItemSourceCopy, m_ItemSource, m_SearchFilter);
        }

        /// <summary>
        /// Sort the sublist.
        /// </summary>
        public void SortList()
        {
            m_SortOptionsDropDown.CurrentOption.Sort?.Invoke(m_ItemSourceCopy);
        }

        /// <summary>
        /// Refresh after assigning a new itemSource.
        /// </summary>
        /// <param name="itemSource">The item source.</param>
        public void Refresh(IList<T> itemSource)
        {
            m_ItemSource = itemSource;
            Refresh();
        }

        /// <summary>
        /// Refresh the list.
        /// </summary>
        public void Refresh()
        {
            var previousSelected = SelectedObject;
            SearchAndSort(m_SearchField.value);
            SelectObject(previousSelected);
        }

        /// <summary>
        /// Select an object in the list.
        /// </summary>
        /// <param name="obj">The object to select.</param>
        public virtual void SelectObject(T obj)
        {
            if (obj == null) { return; }

            var index = -1;

            for (int i = 0; i < m_ItemSourceCopy.Count; i++) {
                if (ReferenceEquals(obj, m_ItemSourceCopy[i]) == false) { continue; }

                index = i;
                break;
            }

            m_ReorderableList.HighlightSelectedItem = true;
            if (index == -1 || (index == SelectedIndex && obj == SelectedObject)) { return; }
            m_ReorderableList.SelectedIndex = index;
        }

        /// <summary>
        /// Function used to filter a list. Used by the search options
        /// </summary>
        /// <param name="searchValue">The search string.</param>
        /// <param name="subSource">The filtered list.</param>
        /// <param name="fullSource">The full lists of objects.</param>
        /// <param name="searchFilter">The filter function.</param>
        protected void SearchFilter(string searchValue, IList subSource, IList<T> fullSource, Func<IList<T>, string, IList<T>> searchFilter)
        {
            if (string.IsNullOrWhiteSpace(searchValue) == false && searchFilter != null) {
                var filteredList = searchFilter.Invoke(fullSource, searchValue);
                for (int i = 0; i < filteredList.Count; ++i) {
                    subSource.Add(filteredList[i]);
                }
            } else if (fullSource != null) {
                for (int i = 0; i < fullSource.Count; ++i) {
                    subSource.Add(fullSource[i]);
                }
            }
        }

        /// <summary>
        /// Focus the search field
        /// </summary>
        public void FocusSearchField()
        {
            m_SearchField.Focus();
            m_SearchField.Q("unity-text-input").Focus();
        }
    }
}
