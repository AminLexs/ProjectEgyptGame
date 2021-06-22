/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Panels
{
    using Core;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using EventHandler = Opsive.Shared.Events.EventHandler;

    /// <summary>
    /// The UI panel manager is used to keep track of panels being opened and closed.
    /// </summary>
    public class DisplayPanelManager : MonoBehaviour
    {
        [Tooltip("Use an ID to differentiate the Panel Managers in a multiplayer setting.")]
        [SerializeField] protected int m_ID = 1;
        [Tooltip("The panel owner will be the target of panel events such as open and close.")]
        [SerializeField] protected GameObject m_PanelOwner;
        [Tooltip("A reference to the panel used for gameplay.")]
        [UnityEngine.Serialization.FormerlySerializedAs("m_GamePlayPanel")]
        [SerializeField] protected DisplayPanel m_GameplayPanel;
        [Tooltip("A reference to the panel used for the inventory menu.")]
        [SerializeField] protected DisplayPanel m_MainMenuPanel;
        [Tooltip("Whenever a menu is opened, should the time scale be set to zero?")]
        [SerializeField] protected bool m_SetTimeScaleToZeroWhenMenuIsOpened;
        [Tooltip("Whenever a menu is opened, should the time scale be set to zero?")]
        [SerializeField] protected bool m_CloseMenuWhenOpeningAnother;

        protected Dictionary<string, DisplayPanel> m_PanelsByName;
        protected DisplayPanel m_SelectedDisplayPanel;
        protected DisplayPanel m_SelectedDisplayMenu;

        public int ID => m_ID;
        
        public DisplayPanel GameplayPanel {
            get { return m_GameplayPanel; }
            internal set => m_GameplayPanel = value;
        }

        public DisplayPanel MainMenu {
            get => m_MainMenuPanel;
            set => m_MainMenuPanel = value;
        }
        
        public GameObject PanelOwner {
            get { return m_PanelOwner; }
            internal set => m_PanelOwner = value;
        }

        public DisplayPanel SelectedDisplayPanel => m_SelectedDisplayPanel;
        public DisplayPanel SelectedDisplayMenu => m_SelectedDisplayMenu;

        protected DisplayPanel[] m_AllUIPanels;

        protected bool m_HasPanelOwner = false;

        /// <summary>
        /// Initialize the listener and set up all the panels.
        /// </summary>
        private void Awake()
        {
            m_PanelsByName = new Dictionary<string, DisplayPanel>();
            
            if (m_PanelOwner == null) { m_PanelOwner = gameObject; }
            SetPanelOwner(m_PanelOwner);

            m_AllUIPanels = GetComponentsInChildren<DisplayPanel>(true);
            for (int i = 0; i < m_AllUIPanels.Length; i++) {
                m_AllUIPanels[i].Setup(this);
            }
        }

        /// <summary>
        /// Initialize the component.
        /// </summary>
        private void Start()
        {
            if (m_GameplayPanel == null) { return; }

            if (m_GameplayPanel.IsOpen) { m_SelectedDisplayPanel = m_GameplayPanel; }
        }

        /// <summary>
        /// Set a new panel owner which will be the target of the menu panel events.
        /// </summary>
        /// <param name="panelOwner">The new panel owner.</param>
        public void SetPanelOwner(GameObject panelOwner)
        {
            if (m_HasPanelOwner && panelOwner == m_PanelOwner) { return; }

            m_HasPanelOwner = true;
            
            if (m_PanelOwner != null) {
                EventHandler.UnregisterEvent<OpenClosePanelInfo>(m_PanelOwner, EventNames.c_GameObject_OnPanelOpenClose_OpenClosePanelInfo, PanelOpenedOrClosed);
                EventHandler.UnregisterEvent(m_PanelOwner, EventNames.c_GameObject_OnInput_ClosePanel, CloseSelectedPanel);
                EventHandler.UnregisterEvent<string>(m_PanelOwner, EventNames.c_GameObject_OnInput_OpenPanel_String, OpenPanel);
                EventHandler.UnregisterEvent<string>(m_PanelOwner, EventNames.c_GameObject_OnInput_TogglePanel_String, TogglePanel);
            }

            m_PanelOwner = panelOwner ?? gameObject;

            EventHandler.RegisterEvent<OpenClosePanelInfo>(m_PanelOwner, EventNames.c_GameObject_OnPanelOpenClose_OpenClosePanelInfo, PanelOpenedOrClosed);
            EventHandler.RegisterEvent(m_PanelOwner, EventNames.c_GameObject_OnInput_ClosePanel, CloseSelectedPanel);
            EventHandler.RegisterEvent<string>(m_PanelOwner, EventNames.c_GameObject_OnInput_OpenPanel_String, OpenPanel);
            EventHandler.RegisterEvent<string>(m_PanelOwner, EventNames.c_GameObject_OnInput_TogglePanel_String, TogglePanel);
        }
        
        /// <summary>
        /// Register a panel.
        /// </summary>
        /// <param name="displayPanel">The panel to register.</param>
        public void RegisterPanel(DisplayPanel displayPanel)
        {
            if(displayPanel == null){ return; }
            
            
            if(string.IsNullOrWhiteSpace(displayPanel.UniqueName)){ return; }

            if (!m_PanelsByName.ContainsKey(displayPanel.UniqueName)) {
                m_PanelsByName.Add(displayPanel.UniqueName, displayPanel);
                return;
            }

            if (m_PanelsByName[displayPanel.UniqueName] != displayPanel) {
                Debug.LogWarning(
                    $"A panel that uses the name {displayPanel.UniqueName} is already registered, please use a different name for {displayPanel}",displayPanel);
            }
        }

        /// <summary>
        /// Get the display panel with the name provided.
        /// </summary>
        /// <param name="uniqueName">The unique name.</param>
        /// <returns>The display panel.</returns>
        public DisplayPanel GetPanel(string uniqueName)
        {
            m_PanelsByName.TryGetValue(uniqueName, out var displayPanel);
            return displayPanel;
        }
        
        /// <summary>
        /// Get the display panel with the name provided.
        /// </summary>
        /// <param name="uniqueName">The unique name.</param>
        /// <returns>The display panel.</returns>
        public void TogglePanel(string uniqueName)
        {
            var panel = GetPanel(uniqueName);
            TogglePanel(panel);
        }
        
        /// <summary>
        /// Toggle the panel specified.
        /// </summary>
        /// <param name="menu">The menu panel.</param>
        public void TogglePanel(DisplayPanel panel)
        {
            if(panel == null){ return; }

            if (!panel.IsOpen) {
                OpenPanel(panel);
                return;
            }

            if (m_SelectedDisplayPanel == panel) {
                CloseSelectedPanel();
                return;
            }

            panel.Close();
        }

        /// <summary>
        /// Get the display panel with the name provided.
        /// </summary>
        /// <param name="uniqueName">The unique name.</param>
        /// <returns>The display panel.</returns>
        public void OpenPanel(string uniqueName)
        {
            var panel = GetPanel(uniqueName);
            OpenPanel(panel);
        }

        /// <summary>
        /// Open the panel specified.
        /// </summary>
        /// <param name="menu">The menu panel.</param>
        public void OpenPanel(DisplayPanel panel)
        {
            if(panel == null){ return; }
            
            var currentSelectable = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();
            if (panel.IsMenuPanel == false) {
                panel.Open(m_SelectedDisplayPanel, currentSelectable);
                return;
            }

            if (m_SelectedDisplayMenu != null) {

                if (m_CloseMenuWhenOpeningAnother) {
                    m_SelectedDisplayMenu.Close();
                } else {
                    Debug.Log("The menu you are trying to open cannot open because another menu is currently opened, please change the settings on the Display panel manager if that is not wanted",gameObject);
                    return;
                }
            }

            if (m_GameplayPanel != null && m_SelectedDisplayPanel == m_GameplayPanel) {
                m_GameplayPanel.Close(false);
            }

            panel.Open(m_SelectedDisplayPanel, null);
        }

        /// <summary>
        /// A new Panel was opened.
        /// </summary>
        /// <param name="panel">The new Panel.</param>
        /// <param name="open">Was the panel opened or closed.</param>
        private void PanelOpenedOrClosed(OpenClosePanelInfo openClosePanelInfo)
        {
            var panel = openClosePanelInfo.ThisPanel;
            var open = openClosePanelInfo.Open;

            //If the panel is a menu
            if (panel.IsMenuPanel) {
                
                if (open) {
                    m_SelectedDisplayMenu = panel;
                    if (m_SetTimeScaleToZeroWhenMenuIsOpened) { Time.timeScale = 0; }
                } else {
                    if (m_SelectedDisplayMenu == panel) { m_SelectedDisplayMenu = null; }
                    if (m_SetTimeScaleToZeroWhenMenuIsOpened) { Time.timeScale = 1; }
                }
                
            }
            
            if (open == false) { return; }
            if (panel.IsNonSelectablePanel) { return; }
            
            m_SelectedDisplayPanel = panel;
        }

        /// <summary>
        /// Open the main menu.
        /// </summary>
        public void OpenMainMenu()
        {
            OpenPanel(m_MainMenuPanel);
        }

        /// <summary>
        /// Close the main menu.
        /// </summary>
        public void CloseMainMenu()
        {
            m_MainMenuPanel.Close();
        }

        /// <summary>
        /// Toggle the main menu.
        /// </summary>
        public void ToggleMainMenu()
        {
            if (m_MainMenuPanel.IsOpen) {
                CloseMainMenu();
            } else {
                OpenMainMenu();
            }
        }

        /// <summary>
        /// Close the panel if the input is pressed.
        /// </summary>
        public void CloseSelectedPanel()
        {
            if (m_SelectedDisplayPanel == null || !m_SelectedDisplayPanel.IsOpen) { return; }

            if (m_SelectedDisplayPanel == m_GameplayPanel) { return; }

            m_SelectedDisplayPanel.Close();
        }

        /// <summary>
        /// Stop listening.
        /// </summary>
        private void OnDestroy()
        {
            EventHandler.UnregisterEvent<OpenClosePanelInfo>(m_PanelOwner, EventNames.c_GameObject_OnPanelOpenClose_OpenClosePanelInfo, PanelOpenedOrClosed);
        }
    }
}
