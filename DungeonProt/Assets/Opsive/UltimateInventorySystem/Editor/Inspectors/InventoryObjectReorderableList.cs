namespace Opsive.UltimateInventorySystem.Editor.Inspectors
{
    using Opsive.Shared.Editor.UIElements;
    using Opsive.UltimateInventorySystem.Storage;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class InventoryObjectReorderableList<T> : VisualElement where T : ScriptableObject
    {
        Func<T[]> m_GetObject;
        private Action<T[]> m_SetObjects;

        protected List<T> m_List;
        protected ReorderableList m_ReorderableList;
        
        public InventoryObjectReorderableList(string title, Func<T[]> getObject, Action<T[]> setObjects, Func<InventoryObjectListElement<T>> createListElement)
        {
            m_GetObject = getObject;
            m_SetObjects = setObjects;

            var array = m_GetObject.Invoke();
            m_List = array == null ? new List<T>() : new List<T>(array);

            m_ReorderableList = new ReorderableList(
                m_List,
                (parent, index) =>
                {
                    var listElement = createListElement.Invoke();
                    listElement.Index = index;
                    listElement.OnValueChanged += OnValueChanged;
                    parent.Add(listElement);
                }, (parent, index) =>
                {
                    var listElement = parent.ElementAt(0) as InventoryObjectListElement<T>;
                    listElement.Index = index;
                    listElement.Refresh(m_ReorderableList.ItemsSource[index] as T);
                }, (parent) =>
                {
                    parent.Add(new Label(title));
                }, (index) => { return 25f; },
                (index) =>
                {
                    //nothing
                }, () =>
                {
                    m_List.Add(default);

                    m_SetObjects.Invoke(m_List.ToArray());
                    m_ReorderableList.Refresh(m_List);
                }, (index) =>
                {
                    if (index < 0 || index >= m_List.Count) { return; }

                    m_List.RemoveAt(index);

                    m_SetObjects.Invoke(m_List.ToArray());
                    m_ReorderableList.Refresh(m_List);
                }, (i1, i2) =>
                {
                    var element1 = m_ReorderableList.ListItems[i1].ItemContents.ElementAt(0) as InventoryObjectListElement<T>;
                    element1.Index = i1;
                    var element2 = m_ReorderableList.ListItems[i2].ItemContents.ElementAt(0) as InventoryObjectListElement<T>;
                    element2.Index = i2;
                    m_SetObjects.Invoke(m_List.ToArray());
                });
            Add(m_ReorderableList);
        }

        /// <summary>
        /// Serialize and update the visuals.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void OnValueChanged(int index, T value)
        {
            m_List[index] = value;
            m_SetObjects.Invoke(m_List.ToArray());
            m_ReorderableList.Refresh(m_List);
        }
    }
    
    public abstract class InventoryObjectListElement<T>: VisualElement
    {
        private InventorySystemDatabase m_Database;
        public event Action<int, T> OnValueChanged;
        public int Index { get; set; }
        public InventorySystemDatabase Database { get; set; }

        /// <summary>
        /// The list element.
        /// </summary>
        public InventoryObjectListElement(InventorySystemDatabase database)
        {
            m_Database = database;
        }

        /// <summary>
        /// Update the visuals.
        /// </summary>
        /// <param name="value">The new value.</param>
        public abstract void Refresh(T value);

        public void HandleValueChanged(T value)
        {
            OnValueChanged?.Invoke(Index,value);
        }
    }
}