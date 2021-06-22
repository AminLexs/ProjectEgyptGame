namespace Opsive.UltimateInventorySystem.Editor.Managers.UIDesigner
{
    using Opsive.UltimateInventorySystem.UI.Panels;
    using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
    using System;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class DisplayPanelOptions : UIDesignerBoxBase
    {
        public override string Title => "Display Panel options";
        public override string Description => "Edit common settings on the panel object.";
        public override Func<Component> SelectTargetGetter => () => m_DisplayPanel;

        protected DisplayPanel m_DisplayPanel;
        protected VisualElement m_Container;
        
        //protected TextField m_PanelName;
        protected ComponentSelectionButton m_SelectInventoryButton;
        //protected ComponentSelectionButton m_SelectPanelButton;

        public DisplayPanel DisplayPanel => m_DisplayPanel;

        public DisplayPanelOptions()
        {
            //m_PanelName = new TextField("Panel Name");
            
            //m_SelectPanelButton = new ComponentSelectionButton("Select Panel",()=>m_DisplayPanel);
            m_SelectInventoryButton = new ComponentSelectionButton("Select Bound Inventory", () => m_DisplayPanel?.GetComponent<InventoryPanelBinding>()?.Inventory);

            m_Container = new VisualElement();
            Add(m_Container);
        }

        public void Refresh(DisplayPanel displayPanel)
        {
            m_Container.Clear();

            m_DisplayPanel = displayPanel;

            if (m_DisplayPanel == null) {
                m_Container.Add(new Label("DisplayPanel does not found for target."));
                return;
            }

            //m_Container.Add(m_SelectPanelButton);
            m_Container.Add(m_SelectInventoryButton);
            
            /*m_PanelName.value = displayPanel.UniqueName;
            m_Container.Add(m_PanelName);*/

        }
    }
}