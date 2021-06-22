/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.Managers
{
    using Opsive.UltimateInventorySystem.Editor.Styles;
    using UnityEngine.UIElements;

    /// <summary>
    /// The Manager is an abstract class which allows for various categories to the drawn to the MainManagerWindow pane.
    /// </summary>
    [System.Serializable]
    public abstract class Manager
    {
        protected VisualElement m_ManagerContentContainer;
        protected MainManagerWindow m_MainManagerWindow;

        public VisualElement ManagerContentContainer => m_ManagerContentContainer;
        public MainManagerWindow MainManagerWindow => m_MainManagerWindow;

        /// <summary>
        /// Initializes the manager after deserialization.
        /// </summary>
        public virtual void Initialize(MainManagerWindow mainManagerWindow)
        {
            m_MainManagerWindow = mainManagerWindow;
            m_ManagerContentContainer = new VisualElement();
            m_ManagerContentContainer.AddToClassList(CommonStyles.s_VerticalLayout);
        }

        /// <summary>
        /// Adds the visual elements to the ManagerContentContainer visual element. 
        /// </summary>
        public abstract void BuildVisualElements();

        /// <summary>
        /// Refreshes the content for the current database.
        /// </summary>
        public virtual void Refresh() { }

        /// <summary>
        /// The manager properties have changed. Serialize the managers.
        /// </summary>
        public void OnManagerChange() { m_MainManagerWindow.SerializeManagers(); }
    }
}