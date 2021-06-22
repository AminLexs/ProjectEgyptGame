/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Equipping
{
    using Opsive.Shared.Game;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.Storage;
    using System;
    using UnityEngine;
    using EventHandler = Opsive.Shared.Events.EventHandler;

    /// <summary>
    /// The Equipper component is used to equip items by converting them to ItemObjects.
    /// </summary>
    public class Equipper : MonoBehaviour, IEquipper, IDatabaseSwitcher
    {
        [Tooltip("The equippers inventory.")]
        [SerializeField] protected Inventory m_Inventory;
        [Tooltip("The equippers itemCollection within the inventory.")]
        [SerializeField]
        protected ItemCollectionID m_EquipmentItemCollectionID =
            new ItemCollectionID("Equipped", ItemCollectionPurpose.Equipped);
        [Tooltip("The attribute name fo the equipment prefab (visual).")]
        [SerializeField] protected string m_EquipablePrefabAttributeName = "EquipmentPrefab";
        [Tooltip("The attribute name fo the usable item prefab (functional).")]
        [SerializeField] protected string m_UsableItemPrefabAttributeName = "UsableItemPrefab";
        [Tooltip("The item slot set used to restruct the items that can be equipped.")]
        [SerializeField] protected ItemSlotSet m_ItemSlotSet;
        [Tooltip("The item object slots which holds the equipped item.")]
        [SerializeField] protected ItemObjectSlot[] m_Slots;

        protected SkinnedMeshRenderer m_MainSkinMeshRenderer;

        protected ItemCollection m_EquipmentItemCollection;

        public ItemSlotSet ItemSlotSet {
            get => m_ItemSlotSet;
            internal set => m_ItemSlotSet = value;
        }
        public ItemObjectSlot[] Slots {
            get => m_Slots;
            internal set => m_Slots = value;
        }

        /// <summary>
        /// Initialize in awake.
        /// </summary>
        protected virtual void Awake()
        {
            m_MainSkinMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            ValidateSlots();
        }

        /// <summary>
        /// Validate the slots by checking the Slot Set.
        /// </summary>
        public virtual void ValidateSlots()
        {
            if (m_Slots == null) { m_Slots = new ItemObjectSlot[0]; }

            var needsRefresh = false;

            if (m_ItemSlotSet == null || m_ItemSlotSet.ItemSlots == null) { return; }

            if (m_Slots.Length != m_ItemSlotSet.ItemSlots.Count) {
                needsRefresh = true;
            } else {
                for (int i = 0; i < m_Slots.Length; i++) {
                    if (m_Slots[i].Name == m_ItemSlotSet.ItemSlots[i].Name &&
                       m_Slots[i].Category == m_ItemSlotSet.ItemSlots[i].Category) { continue; }

                    needsRefresh = true;
                    break;
                }
            }

            if (needsRefresh != true) { return; }

            Array.Resize(ref m_Slots, m_ItemSlotSet.ItemSlots.Count);

            for (int i = 0; i < m_ItemSlotSet.ItemSlots.Count; i++) {
                var itemSlot = m_ItemSlotSet.ItemSlots[i];

                bool foundMatch = false;
                for (int j = 0; j < m_Slots.Length; j++) {
                    if (m_Slots[j] == null || itemSlot.Name != m_Slots[j].Name) { continue; }

                    m_Slots[j] = new ItemObjectSlot(itemSlot.Name, itemSlot.Category, m_Slots[j]);
                    foundMatch = true;

                    if (i != j) {
                        var temp = m_Slots[i];
                        m_Slots[i] = m_Slots[j];
                        m_Slots[j] = temp;
                    }

                    break;
                }

                if (!foundMatch) {
                    m_Slots[i] = new ItemObjectSlot(itemSlot.Name, itemSlot.Category, false, null, null);
                }
            }
        }

        /// <summary>
        /// Initialize the Equiper.
        /// </summary>
        protected virtual void Start()
        {
            if (m_Inventory == null) { m_Inventory = GetComponent<Inventory>(); }
            m_EquipmentItemCollection = m_Inventory.GetItemCollection(m_EquipmentItemCollectionID);

            if (m_EquipmentItemCollection == null) {
                Debug.LogWarning("Your inventory does not have an equipment Item Collection.");
                return;
            }

            EventHandler.RegisterEvent<ItemInfo, ItemStack>(m_Inventory, EventNames.c_Inventory_OnAdd_ItemInfo_ItemStack, OnAddedItemToInventory);
            EventHandler.RegisterEvent<ItemInfo>(m_Inventory, EventNames.c_Inventory_OnRemove_ItemInfo, OnRemovedItemFromInventory);

            var equipmentItemAmounts = m_EquipmentItemCollection.GetAllItemStacks();
            if (equipmentItemAmounts == null) {
                Debug.LogWarning("The Equipment Item Collection is null.");
                return;
            }
            for (int i = 0; i < equipmentItemAmounts.Count; i++) {
                Equip(equipmentItemAmounts[i].Item);
            }
        }

        /// <summary>
        /// Make sure to unregister the listener on Destroy.
        /// </summary>
        private void OnDestroy()
        {
            EventHandler.UnregisterEvent<ItemInfo, ItemStack>(m_Inventory, EventNames.c_Inventory_OnAdd_ItemInfo_ItemStack, OnAddedItemToInventory);
            EventHandler.UnregisterEvent<ItemInfo>(m_Inventory, EventNames.c_Inventory_OnRemove_ItemInfo, OnRemovedItemFromInventory);
        }

        /// <summary>
        /// Equip item that was added to the equipment collection.
        /// </summary>
        /// <param name="originItemInfo">The origin Item info.</param>
        /// /// <param name="addedItemStack">The added item stack.</param>
        private void OnAddedItemToInventory(ItemInfo originItemInfo, ItemStack addedItemStack)
        {
            if (addedItemStack == null) { return; }
            if (addedItemStack.ItemCollection == m_EquipmentItemCollection) {
                Equip(addedItemStack.Item);
            }

        }

        /// <summary>
        /// Unequip an item that was removed from the equipment collection.
        /// </summary>
        /// <param name="removedItemInfo">The removed Item info.</param>
        private void OnRemovedItemFromInventory(ItemInfo removedItemInfo)
        {
            if (removedItemInfo.ItemCollection == m_EquipmentItemCollection) {
                UnEquip(removedItemInfo.Item);
            }
        }

        /// <summary>
        /// Equip an item.
        /// </summary>
        /// <param name="item">The item to equip.</param>
        /// <returns>Return true only if the item equipped successfully.</returns>
        public virtual bool Equip(Item item)
        {
            //Check for available empty slots.
            for (int i = 0; i < m_Slots.Length; i++) {
                if (m_Slots[i].ItemObject != null) { continue; }
                if (m_Slots[i].Category != null && m_Slots[i].Category.InherentlyContains(item) == false) { continue; }

                return Equip(item, i);
            }

            //Check for any slot (even used ones).
            for (int i = 0; i < m_Slots.Length; i++) {
                if (m_Slots[i].Category != null && m_Slots[i].Category.InherentlyContains(item) == false) { continue; }

                return Equip(item, i);
            }

            return false;
        }

        /// <summary>
        /// Equip an item to a specific slot.
        /// </summary>
        /// <param name="item">The item to equip.</param>
        /// <param name="index">The slot to equip to.</param>
        /// <returns>True if equipped successfully.</returns>
        public virtual bool Equip(Item item, int index)
        {
            var slot = m_Slots[index];

            if (slot.Category != null && slot.Category.InherentlyContains(item) == false) { return false; }
            var itemObject = CreateItemObject(item);

            if (slot.IsSkinnedEquipment) {
                slot.SetItemObject(itemObject, m_MainSkinMeshRenderer);
            } else {
                slot.SetItemObject(itemObject);
            }

            EventHandler.ExecuteEvent(this, EventNames.c_Equipper_OnChange);

            return true;
        }

        /// <summary>
        /// Check if the item is equipped.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True if equipped.</returns>
        public bool IsEquipped(Item item)
        {
            if (item == null) { return false; }
            for (int i = 0; i < m_Slots.Length; i++) {
                if (m_Slots[i].ItemObject == null) { continue; }
                if (m_Slots[i].ItemObject.Item != item) { continue; }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the slot has an item equipped already.
        /// </summary>
        /// <param name="index">The slot.</param>
        /// <returns>True if an item is equipped in that slot.</returns>
        public bool IsEquipped(int index)
        {
            if (m_Slots[index].ItemObject == null) { return false; }
            return m_Slots[index].ItemObject.Item != null;
        }

        /// <summary>
        /// Get the item equipped in the slot provided.
        /// </summary>
        /// <param name="index">The slot.</param>
        /// <returns>The item equipped in that slot.</returns>
        public virtual Item GetEquippedItem(int index)
        {
            if (m_Slots[index].ItemObject == null) { return null; }
            return m_Slots[index].ItemObject.Item;
        }

        /// <summary>
        /// Get the Equipment stats by retrieving the total value of the attribute.
        /// </summary>
        /// <param name="attributeName">The attribute name.</param>
        /// <returns>The total amount for the attribute.</returns>
        public virtual int GetEquipmentStat(string attributeName)
        {
            var stat = 0f;
            for (int i = 0; i < m_Slots.Length; i++) {
                if (m_Slots[i].ItemObject == null) { continue; }
                var item = m_Slots[i].ItemObject.Item;

                if (item.TryGetAttributeValue<int>(attributeName, out var intAttributeValue)) {
                    stat += intAttributeValue;
                }
                if (item.TryGetAttributeValue<float>(attributeName, out var floatAttributeValue)) {
                    stat += floatAttributeValue;
                }
            }

            return (int)stat;
        }

        /// <summary>
        /// Get a preview stat total by simulating adding a new item.
        /// </summary>
        /// <param name="attributeName">The attribute.</param>
        /// <param name="itemPreview">The item to preview.</param>
        /// <returns>The total attribute value.</returns>
        public int GetEquipmentStatPreviewAdd(string attributeName, Item itemPreview)
        {
            var stat = 0f;
            for (int i = 0; i < m_Slots.Length; i++) {

                Item item = null;

                if (itemPreview == null) {
                    if (m_Slots[i].ItemObject == null) { continue; }
                    item = m_Slots[i].ItemObject.Item;
                } else if (m_Slots[i].Category.InherentlyContains(itemPreview.Category)) {
                    item = itemPreview;
                } else {
                    if (m_Slots[i].ItemObject == null) { continue; }
                    item = m_Slots[i].ItemObject.Item;
                }

                if (item.TryGetAttributeValue<int>(attributeName, out var intAttributeValue)) {
                    stat += intAttributeValue;
                }
                if (item.TryGetAttributeValue<float>(attributeName, out var floatAttributeValue)) {
                    stat += floatAttributeValue;
                }
            }

            return (int)stat;
        }

        /// <summary>
        /// Preview the attribute stat by simulating removing an item.
        /// </summary>
        /// <param name="attributeName">The attribute.</param>
        /// <param name="itemPreview">The item to preview remove.</param>
        /// <returns>The total attribute value.</returns>
        public int GetEquipmentStatPreviewRemove(string attributeName, Item itemPreview)
        {
            var stat = 0f;
            for (int i = 0; i < m_Slots.Length; i++) {

                if (m_Slots[i].ItemObject == null) { continue; }
                var item = m_Slots[i].ItemObject.Item;
                if (item == itemPreview) { continue; }

                if (item.TryGetAttributeValue<int>(attributeName, out var intAttributeValue)) {
                    stat += intAttributeValue;
                }
                if (item.TryGetAttributeValue<float>(attributeName, out var floatAttributeValue)) {
                    stat += floatAttributeValue;
                }
            }

            return (int)stat;
        }

        /// <summary>
        /// UnEquip an item.
        /// </summary>
        /// <param name="item">The item to unequip.</param>
        public virtual void UnEquip(Item item)
        {
            for (int i = 0; i < m_Slots.Length; i++) {
                if (m_Slots[i].ItemObject == null || m_Slots[i].ItemObject.Item != item) { continue; }

                UnEquip(i);
                return;
            }
        }

        /// <summary>
        /// UnEquip the item at the slot.
        /// </summary>
        /// <param name="index">The slot.</param>
        public virtual void UnEquip(int index)
        {
            var itemObject = m_Slots[index].ItemObject;

            m_Slots[index].SetItemObject(null);

            if (ObjectPool.IsPooledObject(itemObject.gameObject)) {
                ReturnItemObjectToPool(itemObject);
            } else {
                Destroy(itemObject.gameObject);
            }

            EventHandler.ExecuteEvent(this, EventNames.c_Equipper_OnChange);
        }

        /// <summary>
        /// Return the Item Object to the pool.
        /// </summary>
        /// <param name="itemObject">The itemObject to return.</param>
        protected virtual void ReturnItemObjectToPool(ItemObject itemObject)
        {
            //The itemObject could have children that are pooled object such as the equipment model.
            for (int i = itemObject.transform.childCount - 1; i >= 0; i--) {
                if ((ObjectPool.IsPooledObject(itemObject.transform.GetChild(i).gameObject) == false)) { continue; }

                ObjectPool.Destroy(itemObject.transform.GetChild(i).gameObject);
            }

            ObjectPool.Destroy(itemObject.gameObject);
        }

        /// <summary>
        /// Create an item Object from a pool.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The ItemObject.</returns>
        public virtual ItemObject CreateItemObject(Item item)
        {
            if (item.TryGetAttributeValue(m_EquipablePrefabAttributeName, out GameObject itemPrefab) == false) {
                Debug.LogError("Prefab Attribute is undefined for Attribute {m_EquipablePrefabAttributeName}.");
                return null;
            }

            if (itemPrefab == null) {
                Debug.LogError("Prefab Attribute value is null for Attribute {m_EquipablePrefabAttributeName}.");
                return null;
            }

            if (item.TryGetAttributeValue(m_UsableItemPrefabAttributeName, out GameObject usablePrefab) == false) {
                return CreateItemObjectInternal(item, itemPrefab);
            }

            var usableItemGameObject = CreateItemObjectInternal(item, usablePrefab);

            if (usableItemGameObject == null) {
                Debug.LogError($"The Usable Item GameObject is null for Attribute {m_UsableItemPrefabAttributeName}.");
                return null;
            }

            var equipmentGameObject = ObjectPool.Instantiate(itemPrefab, usableItemGameObject.transform);
            equipmentGameObject.transform.localPosition = Vector3.zero;
            equipmentGameObject.transform.localRotation = Quaternion.identity;
            equipmentGameObject.transform.localScale = itemPrefab.transform.localScale;

            return usableItemGameObject;
        }

        /// <summary>
        /// Create an ItemObject.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="itemPrefab">The ItemObjectPrefab.</param>
        /// <returns>The item Object.</returns>
        protected virtual ItemObject CreateItemObjectInternal(Item item, GameObject itemPrefab)
        {
            if (itemPrefab == null) {
                Debug.LogWarning("The item prefab is null.");
                return null;
            }

            var itemGameObject = ObjectPool.Instantiate(itemPrefab);

            var itemObject = itemGameObject.GetComponent<ItemObject>();
            if (itemObject == null) {
                itemObject = itemGameObject.AddComponent<ItemObject>();
            }

            itemObject.SetItem(item);
            return itemObject;
        }

        /// <summary>
        /// Check if the object contained by this component are part of the database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>True if all the objects in the component are part of that database.</returns>
        bool IDatabaseSwitcher.IsComponentValidForDatabase(InventorySystemDatabase database)
        {
            if (database == null) { return false; }

            return (m_ItemSlotSet as IDatabaseSwitcher)?.IsComponentValidForDatabase(database) ?? true;
        }

        /// <summary>
        /// Replace any object that is not in the database by an equivalent object in the specified database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>The objects that have been changed.</returns>
        ModifiedObjectWithDatabaseObjects IDatabaseSwitcher.ReplaceInventoryObjectsBySelectedDatabaseEquivalents(InventorySystemDatabase database)
        {
            if (database == null) { return null; }

            (m_ItemSlotSet as IDatabaseSwitcher)?.ReplaceInventoryObjectsBySelectedDatabaseEquivalents(database);

            return new UnityEngine.Object[] { m_ItemSlotSet };
        }
    }
}