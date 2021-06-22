/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Views
{
    using Opsive.Shared.Game;
    using Opsive.UltimateInventorySystem.Utility;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Serialization;

    /// <summary>
    /// The base class for a box drawer.
    /// </summary>
    public abstract class ViewDrawerBase : MonoBehaviour
    {
        [FormerlySerializedAs("m_UseViewParent")]
        [FormerlySerializedAs("m_UseBoxParent")]
        [Tooltip("If use box parent is true the Item Viewes will each be spawned under the next available box parent under this game object. If false they will spawn under this game object.")]
        [SerializeField] protected bool m_UseViewSlot = true;
        [FormerlySerializedAs("m_DisableViewParentImageComponent")]
        [FormerlySerializedAs("m_DisableBoxParentImageComponent")]
        [Tooltip("Disables the parent box image component so that it may be used as to preview the grid in the editor, but not affect the boxes in game.")]
        [SerializeField] protected bool m_DisableViewSlotImageComponent = true;
        [Tooltip("Content Transform where the Item Viewes or there parents reside.")]
        [SerializeField] protected Transform m_Content;
        [FormerlySerializedAs("m_RemoveItemViewsOnInitialize")]
        [FormerlySerializedAs("m_RemoveItemBoxesOnInitialize")]
        [Tooltip("Initialize this component on start?.")]
        [SerializeField] protected bool m_RemoveViewsOnInitialize = true;
        [FormerlySerializedAs("m_DrawEmptyItemViewsOnInitialize")]
        [FormerlySerializedAs("m_DrawEmptyBoxesOnInitialize")]
        [Tooltip("Draw Empty boxes when initialized?.")]
        [SerializeField] protected bool m_DrawEmptyViewsOnInitialize = true;

        protected IViewSlot[] m_BoxParents;

        public IViewSlot[] BoxParents => m_BoxParents;

        protected bool m_IsInitialized = false;

        public Transform Content
        {
            get => m_Content;
            set => m_Content = value;
        }

        /// <summary>
        /// Initialize on start.
        /// </summary>
        private void Start()
        {
            Initialize(false);
        }

        /// <summary>
        /// Initialize the component.
        /// </summary>
        public virtual void Initialize(bool force)
        {
            if (m_IsInitialized && !force) { return; }
            
            if (m_Content == null) { m_Content = transform; }
            
            var boxParents = new List<IViewSlot>();
            for (int i = 0; i < m_Content.childCount; i++) {
                var childObject = m_Content.GetChild(i).gameObject;
                var childParentBox = childObject.GetComponent<IViewSlot>();

                if (childParentBox == null) { continue; }

                if (m_DisableViewSlotImageComponent) {
                    childParentBox.DisableImage();
                }

                boxParents.Add(childParentBox);
                if (m_RemoveViewsOnInitialize) {
                    RemoveChildrenFromTransform(childParentBox.transform);
                }
            }

            m_BoxParents = boxParents.ToArray();

            if (m_DrawEmptyViewsOnInitialize) {
                for (int i = 0; i < m_BoxParents.Length; i++) {
                    DrawEmptyBox(i, i, true);
                }
            }

            m_IsInitialized = true;
        }

        /// <summary>
        /// Remove boxes.
        /// </summary>
        protected virtual void RemoveBoxes()
        {
            if (m_UseViewSlot) {
                RemoveBoxesFromBoxParents();
            } else {
                RemoveChildrenFromTransform(m_Content);
            }
        }

        /// <summary>
        /// Remove boxes from parents.
        /// </summary>
        protected void RemoveBoxesFromBoxParents()
        {
            for (int i = 0; i < m_BoxParents.Length; i++) {
                if (m_BoxParents[i].View == null) { continue; }

                var obj = m_BoxParents[i].View.gameObject;
                if (Application.isPlaying) {
                    if (ObjectPool.IsPooledObject(obj)) { ObjectPool.Destroy(obj); } else { Destroy(obj); }
                } else {
                    DestroyImmediate(obj);
                }
                
            }
        }

        /// <summary>
        /// Remove children from a transform.
        /// </summary>
        /// <param name="trans">The parent transform.</param>
        protected void RemoveChildrenFromTransform(Transform trans)
        {
            for (int i = trans.childCount - 1; i >= 0; i--) {
                var childObject = trans.GetChild(i).gameObject;
                if (Application.isPlaying) {
                    if (ObjectPool.IsPooledObject(childObject)) { ObjectPool.Destroy(childObject); } else { Destroy(childObject); }
                } else {
                    DestroyImmediate(childObject);
                }
            }
        }

        public void RemoveBox(int index)
        {
            GameObject objectToRemove = null;
            if (m_UseViewSlot) {
                objectToRemove = m_BoxParents[index].View?.gameObject;
                
            } else {
                objectToRemove = m_Content.GetChild(index)?.gameObject;
            }
            
            if(objectToRemove == null){return;}

            if (Application.isPlaying) {
                if (ObjectPool.IsPooledObject(objectToRemove)) { ObjectPool.Destroy(objectToRemove); } else { Destroy(objectToRemove); }
            } else {
                DestroyImmediate(objectToRemove);
            }
            
        }

        protected abstract View DrawEmptyBox(int boxIndex, int elementIndex, bool removePreviousBox);
    }

    /// <summary>
    /// The generic base class for Item Viewes.
    /// </summary>
    /// <typeparam name="T">The box object type.</typeparam>
    public abstract class ViewDrawer<T> : ViewDrawerBase
    {
        public event Action<View<T>, T> BeforeDrawing;
        public event Action<View<T>, T> AfterDrawing;

        protected List<View<T>> m_BoxObjects;

        public IReadOnlyList<View<T>> BoxObjects => m_BoxObjects;

        /// <summary>
        /// Initialize the component.
        /// </summary>
        public override void Initialize(bool force)
        {
            if (m_IsInitialized && !force) { return; }
            
            if (m_BoxObjects == null) {
                m_BoxObjects = new List<View<T>>();
            }
            base.Initialize(force);
        }
        
        /// <summary>
        /// Get a prefab for the Object type.
        /// </summary>
        /// <param name="item">The object.</param>
        /// <returns>The prefab.</returns>
        public abstract GameObject GetViewPrefabFor(T item);

        /// <summary>
        /// Draw the boxes using a set of the elements.
        /// </summary>
        /// <param name="startIndex">The start index of the list.</param>
        /// <param name="endIndex">The end index of the list.</param>
        /// <param name="elements">The list of elements.</param>
        public virtual void DrawBoxes(int startIndex, int endIndex, IReadOnlyList<T> elements)
        {
            
            if (startIndex < 0) {
                Debug.LogWarning($"The start index is {startIndex}, it should not be negative.");
                startIndex = 0;
            }

            RemoveBoxes();

            if (startIndex > endIndex) { return; }

            var elementsEnd = Mathf.Min(endIndex, elements.Count);

            for (int i = startIndex; i < endIndex; i++) {
                
                if (i < elementsEnd) {
                    DrawView(i - startIndex, i, elements[i], false);
                } else {
                    DrawView(i - startIndex, i, default, false);
                }
            }
        }

        /// <summary>
        /// Draw the box for an element.
        /// </summary>
        /// <param name="boxIndex">The box index.</param>
        /// <param name="elementIndex">The element index.</param>
        /// <param name="element">The element.</param>
        /// <returns>The box.</returns>
        public virtual View<T> DrawView(int boxIndex, int elementIndex, T element, bool removePreviousBox)
        {
            if (removePreviousBox) { RemoveBox(boxIndex); }
            
            var boxUI = InstantiateBoxUI(boxIndex, elementIndex, element);

            if (boxUI == null) {
                Debug.LogWarning("The Box Drawer BoxUI Prefab does not have a BoxUI component or it is not of the correct Type");
                return null;
            }

            //Clear to remove any previous state of the pooled object.
            boxUI.Clear();

            BeforeDrawingBox(elementIndex, boxUI, element);

            DrawingBox(elementIndex, boxUI, element);

            AfterDrawingBox(elementIndex, boxUI, element);
            
            m_BoxObjects.EnsureSize(boxIndex+1);

            m_BoxObjects[boxIndex] = boxUI;

            return boxUI;
        }

        protected override View DrawEmptyBox(int boxIndex, int elementIndex, bool removePreviousBox)
        {
            return DrawView(boxIndex, elementIndex, default, removePreviousBox);
        }

        /// <summary>
        /// Instantiate a box.
        /// </summary>
        /// <param name="boxIndex">The box index.</param>
        /// <param name="elementIndex">The element index.</param>
        /// <param name="element">The element.</param>
        /// <returns>The instantiated box.</returns>
        protected virtual View<T> InstantiateBoxUI(int boxIndex, int elementIndex, T element)
        {
            var itemViewPrefab = GetViewPrefabFor(element);
            if (itemViewPrefab == null) {
                Debug.LogWarning("View Prefab is null.");
                return null;
            }

            return m_UseViewSlot
                ? InstantiateBoxUIInBoxParent(itemViewPrefab, boxIndex, elementIndex, element)
                : InstantiateBoxUIInThis(itemViewPrefab, boxIndex, elementIndex, element);
        }

        /// <summary>
        /// Instantiate a box under a parent.
        /// </summary>
        /// <param name="prefab">The box prefab.</param>
        /// <param name="boxIndex">The box index.</param>
        /// <param name="elementIndex">The element index.</param>
        /// <param name="element">The element.</param>
        /// <returns>The box ui.</returns>
        protected virtual View<T> InstantiateBoxUIInBoxParent(GameObject prefab, int boxIndex, int elementIndex, T element)
        {
            if (boxIndex >= m_BoxParents.Length) {
                Debug.LogWarning("There is no view parent for the index provided. Please make sure there are enough box parents when using them.");
                return InstantiateBoxUIInThis(prefab, boxIndex, elementIndex, element);
            }

            var boxUI = ObjectPool.Instantiate(prefab, m_BoxParents[boxIndex].transform).GetComponent<View<T>>();
            boxUI.RectTransform.anchoredPosition = Vector2.zero;
            boxUI.RectTransform.anchorMin = new Vector2(0f, 0f);
            boxUI.RectTransform.anchorMax = new Vector2(1f, 1f);
            boxUI.RectTransform.pivot = new Vector2(0.5f, 0.5f);
            boxUI.RectTransform.sizeDelta = Vector2.zero;
            boxUI.RectTransform.offsetMin = Vector2.zero;
            boxUI.RectTransform.offsetMax = Vector2.zero;
            boxUI.RectTransform.localPosition = Vector3.zero;
            boxUI.RectTransform.localScale = Vector3.one;
            m_BoxParents[boxIndex].SetView(boxUI);
            return boxUI;

        }

        /// <summary>
        /// Instantiate the box Ui within this transform.
        /// </summary>
        /// <param name="prefab">The box prefab.</param>
        /// <param name="boxIndex">The box index.</param>
        /// <param name="elementIndex">The element index.</param>
        /// <param name="element">The element.</param>
        /// <returns>The box ui.</returns>
        protected virtual View<T> InstantiateBoxUIInThis(GameObject prefab, int boxIndex, int elementIndex, T element)
        {
            return ObjectPool.Instantiate(prefab, m_Content).GetComponent<View<T>>();
        }

        /// <summary>
        /// Send an event before drawing box.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="view">The box.</param>
        /// <param name="element">The element.</param>
        protected virtual void BeforeDrawingBox(int index, View<T> view, T element)
        {
            BeforeDrawing?.Invoke(view, element);
        }

        /// <summary>
        /// Draw the box.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="view">The box.</param>
        /// <param name="element">The element.</param>
        protected virtual void DrawingBox(int index, View<T> view, T element)
        {
            view.SetValue(element);
        }

        /// <summary>
        /// Send an event after drawing the box.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="view">The box.</param>
        /// <param name="element">The element.</param>
        protected virtual void AfterDrawingBox(int index, View<T> view, T element)
        {
            AfterDrawing?.Invoke(view, element);
        }

        public bool IsIndexAvailable(int index)
        {
            if (index >= m_BoxObjects.Count) { return false; }

            if (m_BoxObjects[index] == null) { return false; }

            if (m_BoxObjects[index].isActiveAndEnabled == false) { return false; }

            if (m_UseViewSlot && m_BoxParents[index].isActiveAndEnabled == false) { return false;}

            return true;
        }

        /// <summary>
        /// Select the box at index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void Select(int index)
        {
            for (int i = 0; i < m_BoxObjects.Count; i++) {
                if (m_BoxObjects[i] == null) { continue; }

                if (index == i) {
                    m_BoxObjects[index].Select(true);
                    continue;
                }

                if (m_BoxObjects[i].IsSelected) {
                    m_BoxObjects[i].Select(false);
                }
            }
        }

        /// <summary>
        /// Click a box at index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void Click(int index)
        {
            if (index < 0 || index >= m_BoxObjects.Count) { return; }
            m_BoxObjects[index].Click();
        }
    }
}