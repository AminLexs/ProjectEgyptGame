namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers
{
    using Opsive.UltimateInventorySystem.UI.Item;
    using UnityEngine;

    public class ItemViewSlotPanelToTooltip : MonoBehaviour
    {
        [Tooltip("The transform to place next to the Item View selected/clicked.")]
        [SerializeField] protected RectTransform m_PanelToPlace;
        [Tooltip("The inventory grid to monitor.")]
        [SerializeField] internal ItemViewSlotsContainerBase m_ItemViewSlotContainer;
        [Tooltip("Set the anchor position of the panel each time it is placed.")]
        [SerializeField] protected bool m_SetAnchorPosition = false;
        [Tooltip("The anchor position of the panel to place (Only used if the SetAnchorPosition is true).")]
        [SerializeField] protected Vector2 m_AnchorPosition = new Vector2(0, 0.5f);
        [Tooltip("The offset compared to the Item View anchor, 0|0 is the box center. 0.5|0.5 is top right.")]
        [SerializeField] protected Vector2 m_AnchorOffset = new Vector2(0.5f, 0.5f);
        [Tooltip("move the panel so that it fits inside the panel bounds (keep null if the panel is unbound).")]
        [SerializeField] protected RectTransform m_PanelBounds;
        [Tooltip("Place the panel next to the box when clicked.")]
        [SerializeField] internal bool m_PlaceOnClick;
        [Tooltip("Activate/Deactivate the panel when clicked.")]
        [SerializeField] internal bool m_ShowOnClick;
        [Tooltip("Place the panel next to the box when selected.")]
        [SerializeField] internal bool m_PlaceOnSelect = true;
        [Tooltip("Activate/Deactivate the panel when selected.")]
        [SerializeField] internal bool m_ShowOnSelect = true;
        [Tooltip("Hide on deselect.")]
        [SerializeField] internal bool m_HideShowOnDeselect = true;

        protected Vector3[] m_BoundCorner;

        /// <summary>
        /// Listen to the grid events.
        /// </summary>
        private void Awake()
        {
            if (m_PanelToPlace == null) {
                m_PanelToPlace = transform as RectTransform;
            }
            
            if (m_PanelBounds != null) {
                m_BoundCorner = new Vector3[4];
                //Get the world corners bottom-left -> top-left -> top-right -> bottom-right.
                m_PanelBounds.GetWorldCorners(m_BoundCorner);
            }

            if (m_ItemViewSlotContainer == null) {
                Debug.LogError("An Item View Slot Container is missing on the panel placer.", gameObject);
            }

            m_ItemViewSlotContainer.OnItemViewSlotClicked += OnItemClicked;
            m_ItemViewSlotContainer.OnItemViewSlotDeselected += (x) =>
            {
                if (m_HideShowOnDeselect) {
                    m_PanelToPlace.gameObject.SetActive(false);
                }
            };

            m_ItemViewSlotContainer.OnItemViewSlotSelected += OnItemSelected;
        }

        /// <summary>
        /// An item was clicked.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        /// <param name="boxIndex">The box index.</param>
        private void OnItemClicked(ItemViewSlotEventData slotEventData)
        {
            if(m_ShowOnClick == false){ return; }
            
            var itemInfo = slotEventData.ItemViewSlot.ItemInfo;
            var show = m_ShowOnClick && itemInfo.Item != null;

            m_PanelToPlace.gameObject.SetActive(show);
            
            if(show == false){return;}

            if (m_PlaceOnClick) {
                var rectTransform = slotEventData.ItemViewSlot.transform as RectTransform;
                PlacePanel(rectTransform);
            }
        }

        /// <summary>
        /// The item was selected.
        /// </summary>
        /// <param name="itemInfo">The item info.</param>
        /// <param name="boxIndex">The box index.</param>
        private void OnItemSelected(ItemViewSlotEventData slotEventData)
        {
            if(m_ShowOnSelect == false){ return; }
            
            var itemInfo = slotEventData.ItemViewSlot.ItemInfo;
            var show = m_ShowOnSelect && itemInfo.Item != null;
            
            m_PanelToPlace.gameObject.SetActive(show);
            
            if(show == false){return;}

            if (m_PlaceOnSelect) {
                var rectTransform = slotEventData.ItemViewSlot.transform as RectTransform;
                PlacePanel(rectTransform);
            }
        }

        /// <summary>
        /// Place the panel next to an Item View.
        /// </summary>
        /// <param name="rectTransform">The rect transform.</param>
        private void PlacePanel(RectTransform rectTransform)
        {
            var newAnchor = m_SetAnchorPosition ? m_AnchorPosition : m_PanelToPlace.pivot;
            var newPosition = rectTransform.position;

            var positionOffset = new Vector2(
                rectTransform.sizeDelta.x * m_AnchorOffset.x,
                rectTransform.sizeDelta.y * m_AnchorOffset.y);

            if (m_BoundCorner != null) {

                var posRight = newPosition.x + positionOffset.x + m_PanelToPlace.sizeDelta.x * (1 - newAnchor.x);
                var posLeft = newPosition.x + positionOffset.x - m_PanelToPlace.sizeDelta.x * newAnchor.x;
                var posTop = newPosition.y + positionOffset.y + m_PanelToPlace.sizeDelta.y * (1 - newAnchor.y);
                var postBot = newPosition.y + positionOffset.y - m_PanelToPlace.sizeDelta.y * newAnchor.y;

                if (posRight > m_BoundCorner[2].x) {
                    newAnchor = new Vector2(1 - newAnchor.x, newAnchor.y);
                    positionOffset = new Vector2(-positionOffset.x, positionOffset.y);
                } else if (posLeft < m_BoundCorner[0].x) {
                    newAnchor = new Vector2(1 - newAnchor.x, newAnchor.y);
                    positionOffset = new Vector2(-positionOffset.x, positionOffset.y);
                }

                if (posTop > m_BoundCorner[2].y) {
                    newAnchor = new Vector2(newAnchor.x, 1 - newAnchor.y);
                    positionOffset = new Vector2(positionOffset.x, -positionOffset.y);
                } else if (postBot < m_BoundCorner[0].y) {
                    newAnchor = new Vector2(newAnchor.x, 1 - newAnchor.y);
                    positionOffset = new Vector2(positionOffset.x, -positionOffset.y);
                }
            }

            if (m_SetAnchorPosition) {
                m_PanelToPlace.anchorMax = newAnchor;
                m_PanelToPlace.anchorMin = newAnchor;
                m_PanelToPlace.pivot = newAnchor;
            }

            newPosition = new Vector3(
                newPosition.x + positionOffset.x,
                newPosition.y + positionOffset.y,
                newPosition.z);

            m_PanelToPlace.position = newPosition;
        }
    }
}