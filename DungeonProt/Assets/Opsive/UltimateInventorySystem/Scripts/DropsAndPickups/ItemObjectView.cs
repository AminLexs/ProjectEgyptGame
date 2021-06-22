/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.DropsAndPickups
{
    using Opsive.Shared.Game;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Interactions;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
    using System;
    using UnityEngine;
    using EventHandler = Opsive.Shared.Events.EventHandler;

    /// <summary>
    /// The Item pickup visual listener will swap out the ItemPickup mesh gameObject by one specified on the item.
    /// </summary>
    public class ItemObjectView : MonoBehaviour
    {
        [Tooltip("The attribute name for the items visual prefab.")]
        [SerializeField] private string m_PrefabAttributeName;
        [Tooltip("The parent that will hold the item object once spawned.")]
        [SerializeField] internal Transform m_ItemObjectParent;
        [Tooltip("Default visual prefab.")]
        [SerializeField] internal GameObject m_DefaultVisualPrefab;
        [Tooltip("The item UI indicator used to show the item name when an interactor has selected the interactable.")]
        [SerializeField] protected ItemView m_ItemView;

        protected bool m_Initialized;
        protected ItemObject m_ItemObject;
        
        public ItemView ItemView
        {
            get { return m_ItemView; }
            set => m_ItemView = value;
        }

        [ContextMenu("Update Item Visuals")]
        internal void UpdateItemVisuals()
        {
            Initialize(false);
            SetVisualInternal(m_ItemObject.Item);
        }

        /// <summary>
        /// Initialize in awake.
        /// </summary>
        protected void Awake()
        {
            Initialize(false);
        }

        private void Initialize(bool force)
        {
            if(m_Initialized && !force){ return; }
            
            if (string.IsNullOrEmpty(m_PrefabAttributeName)) { m_PrefabAttributeName = "PickupPrefab"; }

            m_ItemObject = GetComponent<ItemObject>();
            if (m_ItemObject != null) { m_ItemObject.ValidateItem(); }

            m_Initialized = true;
        }

        protected void OnEnable()
        {
            if(m_ItemObject == null){ return; }
            EventHandler.RegisterEvent(m_ItemObject, EventNames.c_ItemObject_OnItemChanged, UpdateViews);
        }

        protected void Start()
        {
            UpdateViews();
        }

        protected void OnDisable()
        {
            if(m_ItemObject == null){ return; }
            EventHandler.UnregisterEvent(m_ItemObject, EventNames.c_ItemObject_OnItemChanged, UpdateViews);
        }

        /// <summary>
        /// The item pickup item object has changed.
        /// </summary>
        public void UpdateViews()
        {
            if (m_ItemObject == null) {
                RemoveVisualInternal();
                return;
            }
            
            SetVisualInternal(m_ItemObject.Item);
        }

        private void UpdateItemView(ItemInfo itemInfo)
        {
            if (m_ItemView == null) { return; }
            m_ItemView.SetValue(itemInfo);
        }

        /// <summary>
        /// Remove the game object that holds the item object visuals
        /// </summary>
        protected virtual void RemoveVisualInternal()
        {
            UpdateItemView(ItemInfo.None);
            
            if (m_ItemObjectParent.childCount == 0) { return; }

            var child = m_ItemObjectParent.GetChild(m_ItemObjectParent.childCount - 1);
            if (child != null) {
                if (ObjectPool.IsPooledObject(child.gameObject)) {
                    ObjectPool.Destroy(child.gameObject);
                } else {
                    if (Application.isPlaying) {
                        Destroy(child.gameObject);
                    } else {
                        DestroyImmediate(child.gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// Set the item pickup visual from the item attribute.
        /// </summary>
        /// <param name="item">The item.</param>
        protected virtual void SetVisualInternal(Item item)
        {
            RemoveVisualInternal();
            
            UpdateItemView(m_ItemObject.ItemInfo);

            if (item == null) { return; }

            if (item.ItemDefinition == null) { return; }

            if (!item.TryGetAttributeValue<GameObject>(m_PrefabAttributeName, out var prefab)) {
                prefab = m_DefaultVisualPrefab;
            }

            if (prefab == null) {
                Debug.LogError($"The Prefab attribute value is null, please assign a default value or " +
                               $"in the attribute, make sure the item {item} has a non-null value " +
                               $"for attribute: {m_PrefabAttributeName}.");
                return;
            }

            if (Application.isPlaying) {
                var instance = ObjectPool.Instantiate(prefab, m_ItemObjectParent);
            } else {
                var instance = GameObject.Instantiate(prefab, m_ItemObjectParent);
            }
            
        }
    }
}