namespace Opsive.UltimateInventorySystem.UI.Panels
{
    using Opsive.UltimateInventorySystem.Core;
    using UnityEngine;

    public class PanelOpener : MonoBehaviour
    {
        [Tooltip("The panel manager index")]
        [SerializeField] protected int m_PanelManagerIndex = 1;
        [Tooltip("The panel name (optional if the panel is referenced directly)")]
        [SerializeField] protected string m_PanelName;

        protected bool m_IsInitialized;
        protected DisplayPanel m_DisplayPanel;
        protected DisplayPanelManager m_PanelManager;
        
        protected virtual void Start()
        {
            Initialize(false);
        }

        protected virtual void Initialize(bool force)
        {
            if(m_IsInitialized && !force){ return; }

            if (m_PanelManager == null) {
                m_PanelManager = InventorySystemManager.GetDisplayPanelManager(m_PanelManagerIndex);
            }

            m_DisplayPanel = m_PanelManager.GetPanel(m_PanelName);
            
            m_IsInitialized = true;
        }

        /// <summary>
        /// Open the menu using the client inventory field.
        /// </summary>
        [ContextMenu("Open")]
        public virtual void Open()
        {
            if (m_DisplayPanel != null) {
                m_DisplayPanel.SmartOpen();
            }
        }
    }
}