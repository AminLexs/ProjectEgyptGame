/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Monitors
{
    using Opsive.Shared.Game;
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Serialization;
    using EventHandler = Shared.Events.EventHandler;

    /// <summary>
    /// The inventory monitor.
    /// </summary>
    public class InventoryMonitor : MonoBehaviour
    {
        [Tooltip("The inventory ID to monitor, does not monitor if zero.")]
        [SerializeField] protected uint m_InventoryID = 1;
        [Tooltip("The monitored inventory.")]
        [SerializeField] protected Inventory m_MonitoredInventory;
        [Tooltip("The parent rect transform for the pop ups.")]
        [SerializeField] protected RectTransform m_MonitorContent;

        [FormerlySerializedAs("m_ItemBoxPrefab")]
        [Tooltip("The Item View prefab.")]
        [SerializeField] internal GameObject m_ItemViewPrefab;
        [Tooltip("The maximum number of Item Viewes displayed at once.")]
        [SerializeField] protected int m_MaxDisplays;
        [Tooltip("The maximum time a Item View should be displayed for before fading.")]
        [SerializeField] protected float m_RectMaxDisplayTime;
        [Tooltip("The minimum time an Item View should be displayed for before fading.")]
        [SerializeField] protected float m_RectMinDisplayTime;
        [Tooltip("The transition time between Item Viewes popping up.")]
        [SerializeField] protected float m_RectTransitionTime;

        protected ResizableArray<ItemView> m_ItemDisplays;

        protected WaitForSeconds m_WaitMinDisplayTime;
        protected WaitForSeconds m_WaitMaxDisplayTime;

        protected ResizableArray<ItemInfo> m_ItemAmountBuffer;

        protected bool m_Poping;
        protected Action m_PopAction;
        protected Func<IEnumerator> m_Transition;
        protected Func<ItemView, IEnumerator> m_FadeInOut;
        protected Func<ItemView, IEnumerator> m_FadeIn;
        protected Func<ItemView, IEnumerator> m_FadeOut;

        protected float m_ItemDisplayHeight;
        protected bool m_IsListening;

        public Inventory MonitoredInventory {
            get => m_MonitoredInventory;
            internal set => m_MonitoredInventory = value;
        }

        /// <summary>
        /// Initialize the component on awake.
        /// </summary>
        private void Awake()
        {
            if (m_MonitorContent == null) { m_MonitorContent = transform as RectTransform; }

            if (m_ItemViewPrefab == null) {
                Debug.LogWarning("Display Prefab is null.");
                return;
            }

            var prefabItemDisplay = m_ItemViewPrefab.GetComponent<ItemView>();
            if (prefabItemDisplay == null) {
                Debug.LogWarning("Display Prefab does not have an Item Display component.");
                return;
            }

            if (m_MonitoredInventory == null) {
                if (m_InventoryID != 0) {
                    if (InventorySystemManager.InventoryIdentifierRegister.TryGetValue(m_InventoryID,
                        out var inventoryIdentifier)) {
                        m_MonitoredInventory = inventoryIdentifier.Inventory;
                    }
                }

                if (m_MonitoredInventory == null) {
                    Debug.LogWarning("The Inventory Monitor cannot find an Inventory reference.", this);
                    return;
                }
            }

            EventHandler.RegisterEvent<bool>(m_MonitoredInventory.gameObject, EventNames.c_InventoryGameObject_InventoryMonitorListen_Bool, Listen);

            m_ItemDisplayHeight = prefabItemDisplay.RectTransform.sizeDelta.y;

            m_ItemDisplays = new ResizableArray<ItemView>();
            m_ItemDisplays.Initialize(m_MaxDisplays);
            for (int i = 0; i < m_MaxDisplays; i++) {
                var itemView = Instantiate(m_ItemViewPrefab, m_MonitorContent).GetComponent<ItemView>();
                itemView.CanvasGroup.alpha = 0;
                m_ItemDisplays.Add(itemView);
            }

            m_ItemAmountBuffer = new ResizableArray<ItemInfo>();

            m_WaitMinDisplayTime = new WaitForSeconds(m_RectMinDisplayTime);
            m_WaitMaxDisplayTime = new WaitForSeconds(m_RectMaxDisplayTime);

            m_PopAction = () =>
            {
                m_Poping = false;
                Pop();
            };

            m_Transition = Transition;
            m_FadeInOut = FadeInOut;
            m_FadeIn = FadeIn;
            m_FadeOut = FadeOut;
        }

        private void Listen(bool listen)
        {
            if (listen) {
                StartListening();
            } else {
                StopListening();
            }
        }

        /// <summary>
        /// Start listening to events.
        /// </summary>
        void Start()
        {
            StartListening();
        }

        /// <summary>
        /// Start listening to events.
        /// </summary>
        private void OnEnable()
        {
            StartListening();
        }

        /// <summary>
        /// Start listening to events.
        /// </summary>
        public void StartListening()
        {
            if (m_MonitoredInventory == null) {
                Debug.LogWarning("The Inventory Monitor cannot find an Inventory reference.", this);
                return;
            }

            if (m_IsListening) { return; }

            EventHandler.RegisterEvent<ItemInfo, ItemStack>(m_MonitoredInventory,
                EventNames.c_Inventory_OnAdd_ItemInfo_ItemStack,
                OnItemAmountAdded);

            m_IsListening = true;
        }

        /// <summary>
        /// Stop listening to events.
        /// </summary>
        public void StopListening()
        {
            if (m_IsListening == false) { return; }

            EventHandler.UnregisterEvent<ItemInfo, ItemStack>(m_MonitoredInventory,
                EventNames.c_Inventory_OnAdd_ItemInfo_ItemStack,
                OnItemAmountAdded);

            m_IsListening = false;
        }

        /// <summary>
        /// An item amount was added to the inventory.
        /// </summary>
        /// <param name="addedItemInfo">The item info.</param>
        /// <param name="newStack">The item origin.</param>
        private void OnItemAmountAdded(ItemInfo addedItemInfo, ItemStack newStack)
        {
            if (newStack == null || newStack.ItemCollection != m_MonitoredInventory.MainItemCollection) { return; }
            var originCollection = addedItemInfo.ItemCollection;
            if (originCollection != null) {
                if (originCollection.Purpose == ItemCollectionPurpose.Hide
                    || originCollection.Purpose == ItemCollectionPurpose.Loadout
                    || originCollection.Inventory == newStack.Inventory) {
                    return;
                }
            }
            m_ItemAmountBuffer.Add(addedItemInfo);
            Pop();
        }

        /// <summary>
        /// Pop a Item View to show the next item amount that was added.
        /// </summary>
        protected void Pop()
        {
            if (m_Poping == true || m_ItemAmountBuffer.Count == 0) {
                return;
            }

            m_Poping = true;
            var ItemAmount = m_ItemAmountBuffer[0];
            m_ItemAmountBuffer.RemoveAt(0);

            m_ItemDisplays.MoveElementIndex(m_MaxDisplays - 1, 0);

            var itemDisplay = m_ItemDisplays[0];
            //itemDisplay.gameObject.SetActive(true);
            itemDisplay.RectTransform.anchoredPosition = Vector2.zero;
            itemDisplay.SetValue(ItemAmount);
            itemDisplay.CanvasGroup.alpha = 0;

            //Transition
            //Fade
            if (gameObject.activeInHierarchy) {
                StartCoroutine(m_FadeInOut(itemDisplay));
                StartCoroutine(m_Transition());
            } else {
                for (int i = 1; i < m_ItemDisplays.Count; i++) {
                    m_ItemDisplays[i].RectTransform.anchoredPosition = new Vector2(0, i * m_ItemDisplayHeight);
                }
            }
            Scheduler.Schedule(m_RectMinDisplayTime + m_RectTransitionTime, m_PopAction);
        }

        /// <summary>
        /// Fade in and then out the Item View.
        /// </summary>
        /// <param name="itemView">The Item View.</param>
        /// <returns>The IEnumerator.</returns>
        protected IEnumerator FadeInOut(ItemView itemView)
        {
            yield return m_FadeIn(itemView);
            yield return m_WaitMaxDisplayTime;
            yield return m_FadeOut(itemView);
            //itemDisplay.gameObject.SetActive(false);
        }

        /// <summary>
        /// Fade in the Item View.
        /// </summary>
        /// <param name="itemView">The Item View.</param>
        /// <returns>The IEnumerator.</returns>
        protected IEnumerator FadeIn(ItemView itemView)
        {
            var transitionStep = Time.unscaledDeltaTime / m_RectTransitionTime;
            while (itemView.CanvasGroup.alpha < 1) {
                itemView.CanvasGroup.alpha += transitionStep;
                yield return null;
            }
        }

        /// <summary>
        /// Fade out the Item View.
        /// </summary>
        /// <param name="itemView">The Item View.</param>
        /// <returns>The IEnumerator.</returns>
        protected IEnumerator FadeOut(ItemView itemView)
        {
            var transitionStep = Time.unscaledDeltaTime / m_RectTransitionTime;
            while (itemView.CanvasGroup.alpha > 0) {
                itemView.CanvasGroup.alpha -= transitionStep;
                yield return null;
            }
        }

        /// <summary>
        /// Translate the Item View with a nice Lerp.
        /// </summary>
        /// <returns>The IEnumarator.</returns>
        protected IEnumerator Transition()
        {
            //Move existing boxes up
            var transitionStep = m_ItemDisplayHeight * Time.unscaledDeltaTime / m_RectTransitionTime;
            var positionYDelta = 0f;
            while (positionYDelta < m_ItemDisplayHeight) {
                for (int i = 1; i < m_ItemDisplays.Count; i++) {
                    m_ItemDisplays[i].RectTransform.anchoredPosition = new Vector2(0, (i - 1) * m_ItemDisplayHeight + positionYDelta);
                }
                positionYDelta += transitionStep;
                yield return null;
            }

            for (int i = 1; i < m_ItemDisplays.Count; i++) {
                m_ItemDisplays[i].RectTransform.anchoredPosition = new Vector2(0, i * m_ItemDisplayHeight);
            }
        }

        /// <summary>
        /// Stop listening to events.
        /// </summary>
        private void OnDestroy()
        {
            StopListening();
        }

        /// <summary>
        /// Stop listening to events.
        /// </summary>
        private void OnDisable()
        {
            if(m_ItemDisplays == null){ return; }
            
            for (int i = 0; i < m_ItemDisplays.Count; i++) {
                m_ItemDisplays[i].CanvasGroup.alpha = 0;
            }

            StopListening();
        }
    }
}
