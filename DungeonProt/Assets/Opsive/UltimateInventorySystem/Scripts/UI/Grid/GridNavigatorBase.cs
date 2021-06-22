/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Grid
{
    using UnityEngine;
    using UnityEngine.UI;

    public class GridNavigatorBase : MonoBehaviour
    {
        public event IndexChangeEvent OnIndexChange;
        
        [Tooltip("The grid.")]
        [SerializeField] internal GridBase m_Grid;
        [Tooltip("The tab control (this component is optional).")]
        [SerializeField] internal TabControl m_TabControl;
        [Tooltip("The previous button.")]
        [SerializeField] internal Button m_PreviousButton;
        [Tooltip("The next button.")]
        [SerializeField] internal Button m_NextButton;
        
        protected int m_Index;

        protected int GridElementCount => m_Grid.ElementCount;
        protected virtual int Step => 1;
        protected int StartIndex => m_Index * Step;
        protected int MaxSliceIndex => GridElementCount <= m_Grid.GridSizeCount? 0 : 1 + (GridElementCount - m_Grid.GridSizeCount - 1) / Step;

        protected bool UseTabControl => m_TabControl != null;

        protected bool m_IsInitialized = false;

        private void Awake()
        {
            Initialize(false);
        }

        public virtual void Initialize(bool force)
        {
            if(m_IsInitialized && force == false){ return; }
            
            RegisterGridSystemMoves();

            m_Grid.OnElementsChanged += () =>
            {
                if (m_Index > MaxSliceIndex) {
                    var previousIndex = m_Index;
                    m_Index = MaxSliceIndex; 
                    m_Grid.SetIndex(StartIndex);
                    OnIndexChange?.Invoke(previousIndex, m_Index);
                }
                
                RefreshArrows();
            };

            if (m_PreviousButton != null) {
                m_PreviousButton.onClick.AddListener(() => PreviousSlice());
            }
            
            if (m_NextButton != null) {
                m_NextButton.onClick.AddListener(() => NextSlice());
            }
            
            m_IsInitialized = true;
            m_Grid.SetIndex(StartIndex);
            //m_Grid.Draw();
            //RefreshArrows();
        }

        protected virtual void RegisterGridSystemMoves()
        {
            m_Grid.GridEventSystem.OnUnavailableNavigationLeft += () => PreviousSlice();
            m_Grid.GridEventSystem.OnUnavailableNavigationRight += () => NextSlice();
        }

        public virtual bool NextSlice()
        {
            if (IsNextSliceAvailableInternal()) { return NextSliceInternal(); }

            if (UseTabControl) {
                var result = m_TabControl.NextTab();

                if (result) {
                    m_Index = 0; 
                    RefreshArrows();
                }
                
                return result;
            }

            return false;
        }

        public virtual bool PreviousSlice()
        {
            if (IsPreviousSliceAvailableInternal()) { return PreviousSliceInternal(); }

            if (UseTabControl) {
                var result = m_TabControl.PreviousTab();

                if (result) {
                    m_Index = MaxSliceIndex; 
                    RefreshArrows();
                }
                
                return result;
            }

            return false;
        }

        protected virtual bool NextSliceInternal()
        {
            SetIndexInternal(m_Index + 1);
            return true;
        }

        protected virtual bool PreviousSliceInternal()
        {
            SetIndexInternal(m_Index - 1);
            return true;
        }

        protected void SetIndexInternal(int newIndex)
        {
            var previousIndex = m_Index;
            m_Index = newIndex;
            
            m_Grid.SetIndex(StartIndex);
            OnIndexChange?.Invoke(previousIndex, m_Index);
            m_Grid.Draw();
            RefreshArrows();
        }

        /// <summary>
        /// Is the next page available.
        /// </summary>
        /// <returns>True if it is.</returns>
        public virtual bool IsNextSliceAvailable()
        {
            return IsNextSliceAvailableInternal() || (UseTabControl && m_TabControl.IsNextTabAvailable());
        }

        /// <summary>
        /// Is the previous page available.
        /// </summary>
        /// <returns>True if it is.</returns>
        public virtual bool IsPreviousSliceAvailable()
        {
            return IsPreviousSliceAvailableInternal() || (UseTabControl && m_TabControl.IsPreviousTabAvailable());
        }

        /// <summary>
        /// Is the next page available.
        /// </summary>
        /// <returns>True if it is.</returns>
        protected virtual bool IsNextSliceAvailableInternal()
        {
            return m_Index < MaxSliceIndex;
        }

        /// <summary>
        /// Is the previous page available.
        /// </summary>
        /// <returns>True if it is.</returns>
        protected virtual bool IsPreviousSliceAvailableInternal()
        {
            return m_Index > 0;
        }
        
        /// <summary>
        /// Refresh the arrows, hide if unavailable.
        /// </summary>
        public virtual void RefreshArrows()
        {
            if (m_PreviousButton != null) { m_PreviousButton.gameObject.SetActive(IsPreviousSliceAvailable()); }

            if (m_NextButton != null) { m_NextButton.gameObject.SetActive(IsNextSliceAvailable()); }
        }
    }
}