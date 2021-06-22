namespace Opsive.UltimateInventorySystem.UI.Grid
{
    using Opsive.UltimateInventorySystem.UI.Panels;
    using UnityEngine;

    /// <summary>
    /// A grid display is similar to a gridUI but it is a panel.
    /// </summary>
    public class GridPanel : DisplayPanelBinding
    {
        [Tooltip("The grid.")]
        [SerializeField] protected GridBase m_GridBase;

        public override void Initialize(DisplayPanel display)
        {
            base.Initialize(display);
            m_GridBase.SetParentPanel(m_DisplayPanel);
            m_GridBase.Initialize(false);
        }
    }
}