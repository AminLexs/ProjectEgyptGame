/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Core
{
    using Opsive.UltimateInventorySystem.Core.AttributeSystem;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.Core.Registers;
    using Opsive.UltimateInventorySystem.Crafting;
    using Opsive.UltimateInventorySystem.Exchange;
    using Opsive.UltimateInventorySystem.Storage;
    using Opsive.UltimateInventorySystem.UI.Panels;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The Inventory System Manager keeps track of all items in the game.
    /// Use it to find items, itemDefinitions and categories.
    /// </summary>
    public class InventorySystemManager : MonoBehaviour, IInventorySystemManager
    {
        private static IInventorySystemManager s_Manager;
        private static InventorySystemManager s_Instance;

        [Tooltip("Specify the database asset where the manager will extract data from.")]
        [SerializeField] protected InventorySystemDatabase m_Database;
        [Tooltip("Pre-evaluate the attributes when loading a database will ensure the attribute values are correct and performant.")]
        [SerializeField] protected bool m_PreEvaluateAttributes = true;
        [Tooltip("Don not destroy when loading a new scene.")]
        [SerializeField] protected bool m_DontDestroyOnLoad = false;

        protected InventorySystemRegister m_Register;
        protected InventorySystemFactory m_Factory;

        public InventorySystemDatabase Database {
            get => m_Database;
            internal set => m_Database = value;
        }

        protected bool m_IsInitialized = false;
        protected DisplayPanelManager[] m_DisplayPanelManagers;
        public bool IsInitialized => gameObject != null && m_IsInitialized;

        InventorySystemFactory IInventorySystemManager.Factory => m_Factory;
        InventorySystemRegister IInventorySystemManager.Register => m_Register;

        public static InventorySystemRegister Register => Manager.Register;
        public static InventorySystemFactory Factory => Manager.Factory;

        public static ItemCategoryRegister ItemCategoryRegister => Manager.Register.ItemCategoryRegister;
        public static ItemDefinitionRegister ItemDefinitionRegister => Manager.Register.ItemDefinitionRegister;
        public static ItemRegister ItemRegister => Manager.Register.ItemRegister;

        public static CurrencyRegister CurrencyRegister => Manager.Register.CurrencyRegister;

        public static CraftingCategoryRegister CraftingCategoryRegister => Manager.Register.CraftingCategoryRegister;
        public static CraftingRecipeRegister CraftingRecipeRegister => Manager.Register.CraftingRecipeRegister;
        
        public static InventoryIdentifierRegister InventoryIdentifierRegister => Manager.Register.InventoryIdentifierRegister;

        /// <summary>
        /// Create an item.
        /// </summary>
        /// <param name="itemDefinitionName">The item Definition name.</param>
        /// <param name="attributes">The attributes to override.</param>
        /// <returns>The item.</returns>
        public static Item CreateItem(string itemDefinitionName, IReadOnlyList<AttributeBase> attributes = null)
        {
            var itemDefinition = GetItemDefinition(itemDefinitionName);
            if (itemDefinition == null) { return null; }

            return Manager.Factory.CreateItem(itemDefinition, attributes);
        }

        /// <summary>
        /// Create an item.
        /// </summary>
        /// <param name="itemDefinitionName">The item Definition name.</param>
        /// <param name="id">The item id.</param>
        /// <param name="attributes">The attributes to override.</param>
        /// <returns>The item.</returns>
        public static Item CreateItem(string itemDefinitionName, uint id, IReadOnlyList<AttributeBase> attributes = null)
        {
            var itemDefinition = GetItemDefinition(itemDefinitionName);
            if (itemDefinition == null) { return null; }

            return Manager.Factory.CreateItem(itemDefinition, id, attributes);
        }

        /// <summary>
        /// Create an item.
        /// </summary>
        /// <param name="itemDefinition">The item Definition.</param>
        /// <param name="attributes">The attributes to override.</param>
        /// <returns>The item.</returns>
        public static Item CreateItem(ItemDefinition itemDefinition, IReadOnlyList<AttributeBase> attributes = null)
        {
            return Manager.Factory.CreateItem(itemDefinition, attributes);
        }

        /// <summary>
        /// Create an item.
        /// </summary>
        /// <param name="itemDefinition">The item Definition.</param>
        /// <param name="id">The item id.</param>
        /// <param name="attributes">The attributes to override.</param>
        /// <returns>The item.</returns>
        public static Item CreateItem(ItemDefinition itemDefinition, uint id, IReadOnlyList<AttributeBase> attributes = null)
        {
            return Manager.Factory.CreateItem(itemDefinition, id, attributes);
        }

        /// <summary>
        /// Create an item.
        /// </summary>
        /// <param name="item">The item to copy.</param>
        /// <param name="attributes">The attributes to override.</param>
        /// <returns>The item.</returns>
        public static Item CreateItem(Item item, IReadOnlyList<AttributeBase> attributes = null)
        {
            return Manager.Factory.CreateItem(item, attributes);
        }

        /// <summary>
        /// Create an item.
        /// </summary>
        /// <param name="item">The item to copy.</param>
        /// <param name="id">The item id.</param>
        /// <param name="attributes">The attributes to override.</param>
        /// <returns>The item.</returns>
        public static Item CreateItem(Item item, uint id, IReadOnlyList<AttributeBase> attributes = null)
        {
            return Manager.Factory.CreateItem(item, id, attributes);
        }

        /// <summary>
        /// Get an item by its ID.
        /// </summary>
        /// <param name="id">The item id.</param>
        /// <returns>The item.</returns>
        public static Item GetItem(uint id)
        {
            Manager.Register.ItemRegister.TryGetValue(id, out var item);
            return item;
        }

        /// <summary>
        /// Get the Item Definition by ID.
        /// </summary>
        /// <param name="id">The Item Definition ID.</param>
        /// <returns>The Item Definition.</returns>
        public static ItemDefinition GetItemDefinition(uint id)
        {
            Manager.Register.ItemDefinitionRegister.TryGetValue(id, out var item);
            return item;
        }

        /// <summary>
        /// Get the Item Definition by name.
        /// </summary>
        /// <param name="name">The name of the Item Definition.</param>
        /// <returns>The Item Definition.</returns>
        public static ItemDefinition GetItemDefinition(string name)
        {
            Manager.Register.ItemDefinitionRegister.TryGetValue(name, out var item);
            return item;
        }

        /// <summary>
        /// Get the Item Category by ID.
        /// </summary>
        /// <param name="id">The Item Category ID.</param>
        /// <returns>The Item Category.</returns>
        public static ItemCategory GetItemCategory(uint id)
        {
            Manager.Register.ItemCategoryRegister.TryGetValue(id, out var item);
            return item;
        }

        /// <summary>
        /// Get the Item Category by name.
        /// </summary>
        /// <param name="name">The name of the Item Category.</param>
        /// <returns>The Item Category.</returns>
        public static ItemCategory GetItemCategory(string name)
        {
            Manager.Register.ItemCategoryRegister.TryGetValue(name, out var item);
            return item;
        }

        /// <summary>
        /// Get the Currency by ID.
        /// </summary>
        /// <param name="id">The ID of the Currency.</param>
        /// <returns>The Currency.</returns>
        public static Currency GetCurrency(uint id)
        {
            Manager.Register.CurrencyRegister.TryGetValue(id, out var item);
            return item;
        }

        /// <summary>
        /// Get the Currency by name.
        /// </summary>
        /// <param name="name">The name of the Currency.</param>
        /// <returns>The Currency.</returns>
        public static Currency GetCurrency(string name)
        {
            Manager.Register.CurrencyRegister.TryGetValue(name, out var item);
            return item;
        }

        /// <summary>
        /// Get the Crafting Category by ID.
        /// </summary>
        /// <param name="id">The Crafting Category ID.</param>
        /// <returns>The Crafting Category.</returns>
        public static CraftingCategory GetCraftingCategory(uint id)
        {
            Manager.Register.CraftingCategoryRegister.TryGetValue(id, out var item);
            return item;
        }

        /// <summary>
        /// Get the CraftingCategory by name.
        /// </summary>
        /// <param name="name">The name of the CraftingCategory.</param>
        /// <returns>The CraftingCategory.</returns>
        public static CraftingCategory GetCraftingCategory(string name)
        {
            Manager.Register.CraftingCategoryRegister.TryGetValue(name, out var item);
            return item;
        }

        /// <summary>
        /// Get the CraftingRecipe by ID.
        /// </summary>
        /// <param name="id">The CraftingRecipe ID.</param>
        /// <returns>The CraftingRecipe.</returns>
        public static CraftingRecipe GetCraftingRecipe(uint id)
        {
            Manager.Register.CraftingRecipeRegister.TryGetValue(id, out var item);
            return item;
        }

        /// <summary>
        /// Get the CraftingRecipe by name.
        /// </summary>
        /// <param name="name">The name of the CraftingRecipe.</param>
        /// <returns>The CraftingRecipe.</returns>
        public static CraftingRecipe GetCraftingRecipe(string name)
        {
            Manager.Register.CraftingRecipeRegister.TryGetValue(name, out var item);
            return item;
        }
        
        /// <summary>
        /// Get the Inventory Identifier by ID.
        /// </summary>
        /// <param name="id">The Inventory Identifier ID.</param>
        /// <returns>The Inventory Identifier.</returns>
        public static InventoryIdentifier GetInventoryIdentifier(uint id)
        {
            Manager.Register.InventoryIdentifierRegister.TryGetValue(id, out var item);
            return item;
        }

        internal static bool CheckIfComponentIsValidWithLoadedDatabase(IDatabaseSwitcher @object)
        {
            if (@object.IsComponentValidForDatabase(Instance.Database)) { return true; }

            Debug.LogError($"The component \"{@object}\" on the \"{(@object as Component)?.gameObject}\" GameObject has objects that are not part of the active database. " +
                           $"Please run the 'Replace Database Objects' script by right-clicking on the folder with the affected prefabs, scriptable objects, or scenes.",(@object as Component)?.gameObject);
            return false;
        }

        public static DisplayPanelManager GetDisplayPanelManager()
        {
            return Instance.m_DisplayPanelManagers[0];
        }
        
        public static DisplayPanelManager GetDisplayPanelManager(int id)
        {
            var panelManagers = Instance.m_DisplayPanelManagers;
            for (int i = 0; i < panelManagers.Length; i++) {
                if (panelManagers[i].ID == id) {
                    return panelManagers[i];
                }
            }

            return null;
        }

#if UNITY_2019_3_OR_NEWER
        /// <summary>
        /// Reset the static variables for domain reloading.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void DomainReset()
        {
            s_Instance = null;
            s_Manager = null;
        }
#endif

        /// <summary>
        /// Returns true if the InventorySystemManger was not Initialized.
        /// </summary>
        public static bool IsNull => s_Instance == null;

        public static InventorySystemManager Instance {
            get {
                if (!IsNull) { return s_Instance; }

                s_Instance = FindObjectOfType<InventorySystemManager>();
                if (s_Instance == null) {
                    s_Instance = new GameObject("InventorySystemManager").AddComponent<InventorySystemManager>();
                }

                return s_Instance;
            }
        }

        public static IInventorySystemManager Manager {
            get => s_Manager ?? Instance;
            set => s_Manager = value;
        }

        /// <summary>
        /// The object has been enabled.
        /// </summary>
        protected virtual void OnEnable()
        {
            // The object may have been enabled outside of the scene unloading.
            if (IsNull) {
                s_Instance = this;
            }
        }

        /// <summary>
        /// Called on Awake Initializes the Manager.
        /// </summary>
        protected virtual void Awake()
        {
            if (m_DontDestroyOnLoad) {
                DontDestroyOnLoad(gameObject);
            }

            OnEnable();

            if (s_Instance != this) { Destroy(this); }

            Initialize();
        }

        /// <summary>
        /// Initilaizes the Manager using the database if one is specified.
        /// </summary>
        public virtual void Initialize()
        {
            CreateInventorySystemRegister();
            CreateInventorySystemFactory();

            if (m_Database != null) {
                AddDatabase(m_Database);
            } else {
                Debug.LogWarning("The database is null, please specify one in the Inventory System Manager.");
            }

            CacheDisplayPanelManagers();

            m_IsInitialized = true;
        }

        protected virtual void CacheDisplayPanelManagers()
        {
            m_DisplayPanelManagers = FindObjectsOfType<DisplayPanelManager>();
            if(m_DisplayPanelManagers.Length <= 1 ){ return; }
            
            Array.Sort(m_DisplayPanelManagers, (x, y) => x.ID - y.ID);
            var previousID = m_DisplayPanelManagers[0].ID;
            for (int i = 1; i < m_DisplayPanelManagers.Length; i++) {
                if (m_DisplayPanelManagers[i].ID == previousID) {
                    Debug.LogWarning("At least Two Display panel Managers have the same ID", m_DisplayPanelManagers[i].gameObject);
                }
            }
        }

        /// <summary>
        /// Create the Inventory System Register.
        /// </summary>
        protected virtual void CreateInventorySystemRegister()
        {
            m_Register = new InventorySystemRegister(this);
        }

        /// <summary>
        /// Create the Inventory System Factory.
        /// </summary>
        protected virtual void CreateInventorySystemFactory()
        {
            m_Factory = new InventorySystemFactory(this);
        }

        /// <summary>
        /// Add a database to the manager.
        /// </summary>
        /// <param name="database">The database to add.</param>
        public virtual void AddDatabase(IInventorySystemDatabase database)
        {
            if (database == null) {
                Debug.LogWarning("The database is null.");
                return;
            }

            var databaseCategories = database.ItemCategories;
            var databaseItemDefinitions = database.ItemDefinitions;
            var databaseCurrencies = database.Currencies;
            var databaseCraftingCategories = database.CraftingCategories;
            var databaseRecipe = database.CraftingRecipes;

            //Register All
            for (int i = 0; i < databaseCategories.Length; i++) {
                if (databaseCategories[i] == null) {
                    Debug.LogWarning("An item category in the database is null, please open the editor and fix it.");
                    continue;
                }
                databaseCategories[i].Initialize(true);
                m_Register.ItemCategoryRegister.Register(databaseCategories[i]);
            }
            for (int i = 0; i < databaseItemDefinitions.Length; i++) {
                if (databaseItemDefinitions[i] == null) {
                    Debug.LogWarning("An item definition in the database is null, please open the editor and fix it.");
                    continue;
                }
                databaseItemDefinitions[i].Initialize(true);
                m_Register.ItemDefinitionRegister.Register(databaseItemDefinitions[i]);

                var defaultItem = databaseItemDefinitions[i].DefaultItem;
                defaultItem.Initialize(true);
                m_Register.ItemRegister.Register(ref defaultItem);
            }

            for (int i = 0; i < databaseCurrencies.Length; i++) {
                if (databaseCurrencies[i] == null) {
                    Debug.LogWarning("A currency in the database is null, please open the editor and fix it.");
                    continue;
                }
                databaseCurrencies[i].Initialize(true);
                m_Register.CurrencyRegister.Register(databaseCurrencies[i]);
            }

            for (int i = 0; i < databaseCraftingCategories.Length; i++) {
                if (databaseCraftingCategories[i] == null) {
                    Debug.LogWarning("A crating category in the database is null, please open the editor and fix it.");
                    continue;
                }
                databaseCraftingCategories[i].Initialize(true);
                m_Register.CraftingCategoryRegister.Register(databaseCraftingCategories[i]);
            }

            for (int i = 0; i < databaseRecipe.Length; i++) {
                if (databaseRecipe[i] == null) {
                    Debug.LogWarning("A crafting recipe in the database is null, please open the editor and fix it.");
                    continue;
                }
                databaseRecipe[i].Initialize(true);
                m_Register.CraftingRecipeRegister.Register(databaseRecipe[i]);
            }

            if (!m_PreEvaluateAttributes) { return; }
            //Re-evaluate the attributes

            var allCategories = m_Register.ItemCategoryRegister.GetAll();
            foreach (var category in allCategories) {
                category.ReevaluateAllAttributes();
            }

            var allDefinitions = m_Register.ItemDefinitionRegister.GetAll();
            foreach (var definition in allDefinitions) {
                definition.ReevaluateAttributes();
            }
        }

        /// <summary>
        /// Destroys the object instance on the network.
        /// </summary>
        /// <param name="obj">The object to destroy.</param>
        public static void Destroy(GameObject obj)
        {
            if (s_Instance == null) {
                Debug.LogError("Error: Unable to destroy object - the Inventory System Manager doesn't exist.");
                return;
            }
            s_Instance.DestroyInternal(obj);
        }

        /// <summary>
        /// Destroy the object.
        /// </summary>
        protected void OnDestroy()
        {
            DestroyInternal(gameObject);
        }

        /// <summary>
        /// Do something when object is destroyed.
        /// </summary>
        /// <param name="obj">The object being destroyed.</param>
        protected virtual void DestroyInternal(GameObject obj)
        {
            if (s_Instance == this) { s_Instance = null; }

            if (ReferenceEquals(s_Manager, this)) { s_Manager = null; }

        }
    }
}
