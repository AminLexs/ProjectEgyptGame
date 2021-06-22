/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.SaveSystem
{
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core;
    using UnityEngine;

    /// <summary>
    /// The Inventory System Manager Saver 
    /// </summary>
    public class InventorySystemManagerItemSaver : SaverBase
    {
        public override int Priority => -1000;

        /// <summary>
        /// The item save data.
        /// </summary>
        [System.Serializable]
        public struct ItemsSaveData
        {
            public Item[] Items;
        }

        [Tooltip("If true the items saved that have the same ID but different data will be loaded with a new ID instead of being ignored.")]
        [SerializeField] protected bool m_ForceItemLoad;

        /// <summary>
        /// Serialize the save data.
        /// </summary>
        /// <returns>The serialize data.</returns>
        public override Serialization SerializeSaveData()
        {
            if (InventorySystemManager.IsNull) { return null; }
            var allItems = InventorySystemManager.ItemRegister.GetAll();
            var newItemArray = new Item[allItems.Count];

            int count = 0;
            foreach (var item in allItems) {
                item.Serialize();

                newItemArray[count] = item;
                count++;
            }

            var saveData = new ItemsSaveData {
                Items = newItemArray
            };

            return Serialization.Serialize(saveData);
        }

        /// <summary>
        /// Deserialize and load the save data.
        /// </summary>
        /// <param name="serializedSaveData">The serialized save data.</param>
        public override void DeserializeAndLoadSaveData(Serialization serializedSaveData)
        {
            if (InventorySystemManager.IsNull) { return; }

            var savedData = serializedSaveData.DeserializeFields(MemberVisibility.All) as ItemsSaveData?;

            if (savedData.HasValue == false) {
                return;
            }

            var itemsSaveData = savedData.Value;

            //Reset InventorySystemManager First.
            if (InventorySystemManager.Instance.IsInitialized == false) {
                InventorySystemManager.Instance.Initialize();
            }

            for (int i = 0; i < itemsSaveData.Items.Length; i++) {
                var item = itemsSaveData.Items[i];

                if (InventorySystemManager.ItemRegister.TryGetValue(item.ID, out var registeredItem)) {
                    //An Item with the same ID is already loaded
                    if (m_ForceItemLoad && Item.AreValueEquivalent(item, registeredItem) == false) {
                        item.ID = RandomID.Empty;
                        item.Initialize(false);
                        InventorySystemManager.ItemRegister.Register(ref item);
                    }
                } else {
                    item.Initialize(false);
                    InventorySystemManager.ItemRegister.Register(ref item);
                }
            }
        }
    }
}