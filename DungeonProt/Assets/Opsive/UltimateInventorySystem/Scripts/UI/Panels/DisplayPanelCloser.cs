/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Panels
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// <summary>
    /// A component that allows to close panels when clicking the background image.
    /// </summary>
    public class DisplayPanelCloser : MonoBehaviour, IPointerClickHandler
    {
        [Tooltip("Close the panel when clicking the background image (Use an image that covers the entire screen).")]
        [SerializeField] protected bool m_CloseOnBackgroundClick = true;
        [Tooltip("Let the user click through the background image?")]
        [SerializeField] protected bool m_PassThroughClick = false;
        [Tooltip("The display panel that needs to act as a pop up.")]
        [SerializeField] protected DisplayPanel m_DisplayPanel;

        protected List<RaycastResult> m_RaycastResults;

        /// <summary>
        /// Handle the on pointer click event and close the display panel.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!m_CloseOnBackgroundClick) { return; }

            m_DisplayPanel.Close();

            if (!m_PassThroughClick) { return; }

            PassThroughClick(eventData);
        }

        /// <summary>
        /// Pass through click allows to click after the background is closed.
        /// </summary>
        /// <param name="eventData">The event Data.</param>
        private void PassThroughClick(PointerEventData eventData)
        {
            if (m_RaycastResults == null) { m_RaycastResults = new List<RaycastResult>(); } else { m_RaycastResults.Clear(); }

            EventSystem.current.RaycastAll(eventData, m_RaycastResults);

            if (m_RaycastResults.Count <= 0) { return; }

            var result = m_RaycastResults[0];

            var selectable = result.gameObject.GetComponentInParent<Selectable>();
            var clickableGameObject = selectable?.gameObject;
            if (clickableGameObject == null) {
                var clickable = result.gameObject.GetComponentInParent<IPointerClickHandler>();
                clickableGameObject = (clickable as MonoBehaviour)?.gameObject;
            }

            if (clickableGameObject == null) { clickableGameObject = result.gameObject; }

            ExecuteEvents.Execute(clickableGameObject, eventData, ExecuteEvents.pointerEnterHandler);
            ExecuteEvents.Execute(clickableGameObject, eventData, ExecuteEvents.selectHandler);
            ExecuteEvents.Execute(clickableGameObject, eventData, ExecuteEvents.updateSelectedHandler);
            ExecuteEvents.Execute(clickableGameObject, eventData, ExecuteEvents.pointerClickHandler);

        }
    }
}