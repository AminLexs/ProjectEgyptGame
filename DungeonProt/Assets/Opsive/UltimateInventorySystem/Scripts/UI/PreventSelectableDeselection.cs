/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// This component is used to prevent UI from being deselected.
    /// If nothing is selected, it will select the last selected selectable. 
    /// </summary>
    public class PreventSelectableDeselection : MonoBehaviour
    {
        EventSystem m_EventSystem;

        /// <summary>
        /// Initialize the component.
        /// </summary>
        private void Start()
        {
            m_EventSystem = EventSystem.current;
        }

        GameObject m_LastSelection;

        /// <summary>
        /// Check the event system selection, and select the previous one if null.
        /// </summary>
        private void Update()
        {
            if (m_EventSystem.currentSelectedGameObject != null && m_EventSystem.currentSelectedGameObject != m_LastSelection)
                m_LastSelection = m_EventSystem.currentSelectedGameObject;
            else if (m_LastSelection != null && m_EventSystem.currentSelectedGameObject == null)
                m_EventSystem.SetSelectedGameObject(m_LastSelection);
        }
    }
}
