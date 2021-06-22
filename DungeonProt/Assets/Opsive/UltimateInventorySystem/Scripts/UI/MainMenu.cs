/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI
{
    using Opsive.UltimateInventorySystem.UI.CompoundElements;
    using Opsive.UltimateInventorySystem.UI.Panels;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// A struct of a panel and a button.
    /// </summary>
    [Serializable]
    public struct PanelButton
    {
        [Tooltip("The panel.")]
        public DisplayPanel Panel;
        [Tooltip("The action button.")]
        public ActionButton Button;
    }

    /// <summary>
    /// The main menu panel.
    /// </summary>
    public class MainMenu : DisplayPanelBinding
    {
        [Tooltip("The button mapped to open panels.")]
        [SerializeField] internal List<PanelButton> m_Panels;
        [Tooltip("The button used to close the menu.")]
        [SerializeField] protected ActionButton m_CloseButton;
        [Tooltip("The button used to quit the application.")]
        [SerializeField] protected ActionButton m_QuitButton;
        [Tooltip("The menu tabs transform.")]
        [SerializeField] protected RectTransform m_MenuTabs;

        protected int m_SelectedPanelIndex;
        
        public List<PanelButton> Panels {
            get => m_Panels;
            set => m_Panels = value;
        }

        public RectTransform Content => m_DisplayPanel.MainContent;
        public RectTransform MenuTabs => m_MenuTabs;

        /// <summary>
        /// Set up the panel.
        /// </summary>
        public override void Initialize(DisplayPanel display)
        {
            if (m_IsInitialized) { return; }
            base.Initialize(display);

            for (int i = 0; i < m_Panels.Count; i++) {
                if (m_Panels[i].Button == null) {
                    Debug.LogWarning("One of the buttons on the MainMenu is null",gameObject);
                    continue;
                }
                var localI = i;
                m_Panels[i].Button.OnSubmitE += () => OpenSubPanel(localI);
            }

            if (m_CloseButton != null) {
                m_CloseButton.OnSubmitE += () => m_DisplayPanel.Close(true);
            }

            if (m_QuitButton != null) { m_QuitButton.OnSubmitE += Application.Quit; }
        }

        public override void OnOpen()
        {
            base.OnOpen();
            OpenSubPanel(m_SelectedPanelIndex);
        }

        /// <summary>
        /// Open the menu.
        /// </summary>
        /// <param name="index">The menu index.</param>
        public void OpenSubPanel(int index)
        {
            m_SelectedPanelIndex = index;

            for (int i = 0; i < m_Panels.Count; i++) {
                if (i == index) { continue; }

                m_Panels[i].Panel.Close(false);
                m_Panels[i].Panel.gameObject.SetActive(false);
            }
            
            if(index <0 || index >= m_Panels.Count){ return; }

            //Important to do this after the loop because the close function selects the previous button
            m_Panels[index].Panel.Open(m_DisplayPanel, m_Panels[index].Button);
        }

        public void OpenPrevious()
        {
            if(m_SelectedPanelIndex <= 0){ return; }
            OpenSubPanel(m_SelectedPanelIndex-1);
        }

        public void OpenNext()
        {
            if(m_SelectedPanelIndex >= m_Panels.Count-1){ return; }
            OpenSubPanel(m_SelectedPanelIndex+1);
        }
    }
}
