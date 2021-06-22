namespace Opsive.UltimateInventorySystem.UI.Panels
{
    using UnityEngine;

    /// <summary>
    /// The base class for UI panels.
    /// </summary>
    public class DisplayPanelDragHandler : PanelDragHandler
    {
        [SerializeField] protected DisplayPanel m_Panel;

        protected override void Awake()
        {
            base.Awake();
            if (m_Panel == null) { m_Panel = GetComponentInParent<DisplayPanel>(); }
            m_Panel.OnOpen += HandleDisplayPanelOpen;
        }

        private void HandleDisplayPanelOpen()
        {
            m_PanelRect.SetAsLastSibling();
        }
    }
}