/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.VisualElements
{
    using Opsive.Shared.Editor.Utility;
    using Opsive.UltimateInventorySystem.Editor.Styles;
    using Opsive.UltimateInventorySystem.Editor.VisualElements.ViewNames;
    using Opsive.UltimateInventorySystem.Storage;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using PopupWindow = Opsive.UltimateInventorySystem.Editor.Managers.PopupWindow;

    /// <summary>
    /// Inventory Object Field used for a customized object picker.
    /// </summary>
    /// <typeparam name="T">The Object type.</typeparam>
    public abstract class InventoryObjectField<T> : VisualElement where T : class
    {
        public event Action OnClose;

        protected Label m_Label;
        protected ViewName<T> m_ViewName;
        protected Button m_Button;

        protected PopupWindow m_PopupWindow;
        protected VisualElement m_PopupWindowContent;
        protected SearchableList<T> m_SearchableList;

        protected InventorySystemDatabase m_InventorySystemDatabase;
        protected IList<(string, Action<T>)> m_Actions;
        protected T m_SelectedValue;
        protected Func<T, bool> m_PrefilterCondition;

        public T Value => m_ViewName?.Object;
        public ViewName<T> ViewName => m_ViewName;
        public bool IncludeNullOption { get; set; } = true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="inventorySystemDatabase">The inventory system database.</param>
        /// <param name="mainManagerWindow">The main manager window used to position the popup.</param>
        /// <param name="actions">The actions that can be performed on the selected object.</param>
        /// <param name="preFilterCondition">The pre filter conditions can filter the fields objects.</param>
        public InventoryObjectField(string label,
            InventorySystemDatabase inventorySystemDatabase,
            IList<(string, Action<T>)> actions,
            Func<T, bool> preFilterCondition)
        {
            m_InventorySystemDatabase = inventorySystemDatabase;
            m_Actions = actions;
            m_PrefilterCondition = preFilterCondition;

            m_ViewName = MakeFieldViewName();

            if (string.IsNullOrEmpty(label) == false) {
                m_Label = new Label(label);
                m_Label.style.alignSelf = Align.Center;
                Add(m_Label);

                var box = new VisualElement();
                box.AddToClassList("flex-grow");
                box.AddToClassList(ManagerStyles.BoxBackground);
                box.Add(m_ViewName);
                Add(box);
            } else {
                Add(m_ViewName);
            }

            m_Button = new Button();
            m_Button.text = "Change";
            m_Button.clickable.clicked += ChangeButtonClick;
            Add(m_Button);

            AddToClassList(ManagerStyles.s_InventoryObjectField);

            BuildPopupContent();
            Refresh();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="inventorySystemDatabase">The inventory system database.</param>
        /// <param name="mainManagerWindow">The main manager window used to position the popup.</param>
        /// <param name="action">The actions that can be performed on the selected object.</param>
        /// <param name="preFilterCondition">The pre filter conditions can filter the fields objects.</param>
        public InventoryObjectField(string label,
            InventorySystemDatabase inventorySystemDatabase,
            Action<T> action,
            Func<T, bool> preFilterCondition) : this(label, inventorySystemDatabase, new[] { ("Select", action) }, preFilterCondition)
        {
        }

        /// <summary>
        /// Make the field view name.
        /// </summary>
        /// <returns>The new field view name.</returns>
        protected abstract ViewName<T> MakeFieldViewName();

        /// <summary>
        /// Open the pop up window.
        /// </summary>
        private void ChangeButtonClick()
        {
            OpenPopUpWindow(m_Button.worldBound.position,
                m_Button.worldBound.size);
        }

        public void OpenPopUpWindow(Vector2 buttonPosition, Vector2 buttonSize)
        {
            Refresh();

            var size = new Vector2(250, 300);
            var position = EditorWindow.focusedWindow.position.position + buttonPosition
                           + new Vector2(buttonSize.x - size.x, 0);
            m_PopupWindow = PopupWindow.OpenWindow(
                new Rect(position, size),
                size, m_PopupWindowContent);
            m_PopupWindow.rootVisualElement.styleSheets.Add(InspectorUtility.LoadAsset<StyleSheet>("e70f56fae2d84394b861a2013cb384d0"));
            m_PopupWindow.rootVisualElement.styleSheets.Add(CommonStyles.StyleSheet);
            m_PopupWindow.rootVisualElement.styleSheets.Add(ManagerStyles.StyleSheet);
            m_PopupWindow.rootVisualElement.styleSheets.Add(ControlTypeStyles.StyleSheet);
            m_SearchableList.FocusSearchField();
        }

        /// <summary>
        /// Build the pop up window content.
        /// </summary>
        protected virtual void BuildPopupContent()
        {
            var genericMenu = new GenericMenu();

            for (int i = 0; i < m_Actions.Count; i++) {
                var localI = i;
                genericMenu.AddItem(new GUIContent(m_Actions[localI].Item1), false, () =>
                {
                    m_Actions[localI].Item2?.Invoke(m_SelectedValue);
                    m_SearchableList.SelectedIndex = -1;
                    ClosePopup();

                });
            }

            m_PopupWindowContent = new VisualElement();
            m_PopupWindowContent.name = "PopupSearchableList";
            m_SearchableList = new SearchableList<T>(
                GetSource(),
                MakeItem,
                BindItem, null,
                (index) =>
                {
                    if (index == -1) { return; }

                    m_SelectedValue = m_SearchableList.SelectedObject;
                    genericMenu.ShowAsContext();
                    m_SearchableList.SelectedIndex = -1;
                },
                null, null, null,
                GetSortOptions(),
                FilterOptions);
            m_SearchableList.selectOnRefresh = false;

            m_PopupWindowContent.Add(m_SearchableList);
        }

        /// <summary>
        /// Set the preFilter.
        /// </summary>
        /// <param name="preFilterCondition">The prefilter condition.</param>
        public virtual void SetPreFilter(Func<T, bool> preFilterCondition)
        {
            if (preFilterCondition == null) { return; }
            m_PrefilterCondition = preFilterCondition;
        }

        /// <summary>
        /// Set if interactable.
        /// </summary>
        /// <param name="interactable">interactable.</param>
        public virtual void SetInteractable(bool interactable)
        {
            m_Button.SetEnabled(interactable);
            m_Button.visible = interactable;
        }

        /// <summary>
        /// Filter options for the search list.
        /// </summary>
        /// <param name="list">The list source.</param>
        /// <param name="searchValue">The search value.</param>
        /// <returns>The filtered list.</returns>
        protected abstract IList<T> FilterOptions(IList<T> list, string searchValue);

        /// <summary>
        /// The sort options.
        /// </summary>
        /// <returns>The sort options.</returns>
        protected abstract IList<SortOption> GetSortOptions();

        /// <summary>
        /// Bind the list item.
        /// </summary>
        /// <param name="parent">The parent visual element.</param>
        /// <param name="index">The index.</param>
        protected abstract void BindItem(VisualElement parent, int index);

        /// <summary>
        /// Make the list item.
        /// </summary>
        /// <param name="parent">The parent visual element.</param>
        /// <param name="index">The index.</param>
        protected abstract void MakeItem(VisualElement parent, int index);

        /// <summary>
        /// Return the source of the list.
        /// </summary>
        /// <returns>The list source.</returns>
        protected IList<T> GetSource()
        {
            var source = GetSourceInternal();
            if (source == null) {
                return null;
            }
            var filteredList = new List<T>();
            for (int i = 0; i < source.Count; i++) {
                if (m_PrefilterCondition?.Invoke(source[i]) ?? true) {
                    filteredList.Add(source[i]);
                }
            }
            //Add a null option to remove object.
            if (IncludeNullOption) { filteredList.Add(null); }
            return filteredList;
        }

        /// <summary>
        /// Return the source of the list.
        /// </summary>
        /// <returns>The list source.</returns>
        protected abstract IList<T> GetSourceInternal();

        /// <summary>
        /// Refresh the ObjectField.
        /// </summary>
        /// <param name="obj">The new object to view.</param>
        public void Refresh(T obj)
        {
            if (obj == null || obj.Equals(null)) { obj = null; }

            m_ViewName.Refresh(obj);
            Refresh();
        }

        /// <summary>
        /// Refresh the ObjectField.
        /// </summary>
        public void Refresh()
        {
            m_ViewName.Refresh(Value);
            m_SearchableList.Refresh(GetSource());
        }

        /// <summary>
        /// Close the pop up window.
        /// </summary>
        protected void ClosePopup()
        {
            m_PopupWindow.Close();
            OnClose?.Invoke();
        }
    }
}