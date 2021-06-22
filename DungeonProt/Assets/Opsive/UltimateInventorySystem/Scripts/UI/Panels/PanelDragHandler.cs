namespace Opsive.UltimateInventorySystem.UI.Panels
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// The base class for UI panels.
    /// </summary>
    public class PanelDragHandler : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        [SerializeField] protected RectTransform m_PanelRect;

        protected Canvas m_Canvas;
        
        protected virtual void Awake()
        {
            if (m_PanelRect == null) { m_PanelRect = GetComponent<RectTransform>(); }
            if (m_Canvas == null) { m_Canvas = GetComponentInParent<Canvas>(); }
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_PanelRect.anchoredPosition += eventData.delta / m_Canvas.scaleFactor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_PanelRect.SetAsLastSibling();
        }
    }
}