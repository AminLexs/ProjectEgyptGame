/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.Managers
{
    using Opsive.Shared.Editor.Utility;
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Editor.Managers.UIDesigner;
    using Opsive.UltimateInventorySystem.Editor.Styles;
    using Opsive.UltimateInventorySystem.Editor.Utility;
    using Opsive.UltimateInventorySystem.Storage;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// The MainManagerWindow is an editor window which contains all of the sub managers. This window draws the high level menu options and draws
    /// the selected sub manager.
    /// </summary>
    [InitializeOnLoad]
    public class MainManagerWindow : EditorWindow
    {
        public event Action OnFocusEvent;
        public static event Action OnLostFocusEvent;

        private static MainManagerWindow s_Instance;
        public static MainManagerWindow Instance => s_Instance;
        public static InventorySystemDatabase InventorySystemDatabase {
            get {
                if (s_Instance != null) { return s_Instance.Database; }

                if (!string.IsNullOrEmpty(DatabaseGUID)) {
                    return InspectorUtility.LoadAsset<InventorySystemDatabase>(DatabaseGUID);
                }

                return null;
            }
        }
        private const float c_MenuWidth = 120;
        private const string c_LatestVersionKey = "Opsive.UltimateInventorySystem.Editor.LatestVersion";
        private const string c_LastUpdateCheckKey = "Opsive.UltimateInventorySystem.Editor.LastUpdateCheck";

        private static string DatabaseGUIDKey => "Opsive.UltimateInventorySystem.DatabaseGUID." + Application.productName;

        // The database path is based on the project.
        private static string DatabaseGUID { get => EditorPrefs.GetString(DatabaseGUIDKey, string.Empty); }


        public float MenuWidth { get { return c_MenuWidth; } }

        private Manager[] m_Managers;
        private string[] m_ManagerNames;
        private int m_MenuSelection;
        private List<int> m_RequiredDatabaseManagers = new List<int>();

        private VisualElement m_MenuElement;
        private MenuButton[] m_MenuButtons;
        private Label m_SelectedManagerLabel;
        private VisualElement m_ContentElement;

        // Unity's serialization doesn't support abstract classes so serialize the data separately.
        private Serialization[] m_ManagerData;

        private UnityEngine.Networking.UnityWebRequest m_UpdateCheckRequest;
        private DateTime m_LastUpdateCheck = DateTime.MinValue;

        private InventorySystemDatabase m_Database;

        public IReadOnlyList<Manager> Managers => m_Managers;
        public string LatestVersion {
            get => EditorPrefs.GetString(c_LatestVersionKey, AssetInfo.Version);
            set => EditorPrefs.SetString(c_LatestVersionKey, value);
        }
        private DateTime LastUpdateCheck {
            get {
                try {
                    // Don't read from editor prefs if it isn't necessary.
                    if (m_LastUpdateCheck != DateTime.MinValue) {
                        return m_LastUpdateCheck;
                    }

                    m_LastUpdateCheck = DateTime.Parse(EditorPrefs.GetString(c_LastUpdateCheckKey, "1/1/1971 00:00:01"), System.Globalization.CultureInfo.InvariantCulture);
                } catch (Exception /*e*/) {
                    m_LastUpdateCheck = DateTime.UtcNow;
                }
                return m_LastUpdateCheck;
            }
            set {
                m_LastUpdateCheck = value;
                EditorPrefs.SetString(c_LastUpdateCheckKey, m_LastUpdateCheck.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
        }
        public InventorySystemDatabase Database {
            get => m_Database;
            set { UpdateDatabase(value); }
        }

        /// <summary>
        /// Unity button which has a different style for the menu buttons.
        /// </summary>
        private class MenuButton : Button
        {
            /// <summary>
            /// Default constructor.
            /// </summary>
            public MenuButton() : base()
            {
                name = ManagerStyles.MenuButton;
                RemoveFromClassList(ussClassName);
            }
        }

        /// <summary>
        /// Initializes the Main Manager.
        /// </summary> 
        [MenuItem("Tools/Opsive/Ultimate Inventory System/Main Manager", false, 0)]
        public static MainManagerWindow ShowWindow()
        {
            var window = EditorWindow.GetWindow<MainManagerWindow>(false, "Inventory Manager");
            window.minSize = new Vector2(800, 550);
            return window;
        }

        /// <summary>
        /// Initializes the Main Manager and shows the Item Categories Manager.
        /// </summary> 
        [MenuItem("Tools/Opsive/Ultimate Inventory System/Item Categories Manager", false, 11)]
        public static void ShowItemCategoriesManagerWindow()
        {
            var window = ShowWindow();
            window.Open(typeof(ItemCategoryManager));
        }

        /// <summary>
        /// Initializes the Main Manager and shows the Item Definitions Manager.
        /// </summary> 
        [MenuItem("Tools/Opsive/Ultimate Inventory System/Item Definitions Manager", false, 12)]
        public static void ShowItemDefinitionsManagerWindow()
        {
            var window = ShowWindow();
            window.Open(typeof(ItemDefinitionManager));
        }

        /// <summary>
        /// Initializes the Main Manager and shows the Crafting Categories Manager.
        /// </summary> 
        [MenuItem("Tools/Opsive/Ultimate Inventory System/Crafting Categories Manager", false, 13)]
        public static void ShowCraftingCategoriesManagerWindow()
        {
            var window = ShowWindow();
            window.Open(typeof(CraftingCategoryManager));
        }

        /// <summary>
        /// Initializes the Main Manager and shows the Crafting Recipes Manager.
        /// </summary> 
        [MenuItem("Tools/Opsive/Ultimate Inventory System/Crafting Recipes Manager", false, 14)]
        public static void ShowCraftingRecipliesManagerWindow()
        {
            var window = ShowWindow();
            window.Open(typeof(CraftingRecipeManager));
        }

        /// <summary>
        /// Initializes the Main Manager and shows the Currencies Manager.
        /// </summary> 
        [MenuItem("Tools/Opsive/Ultimate Inventory System/Currencies Manager", false, 15)]
        public static void ShowCurrenciesManagerWindow()
        {
            var window = ShowWindow();
            window.Open(typeof(CurrencyManager));
        }
        
        /// <summary>
        /// Initializes the Main Manager and shows the Currencies Manager.
        /// </summary> 
        [MenuItem("Tools/Opsive/Ultimate Inventory System/UI Designer", false, 16)]
        public static void ShowUIDesignerWindow()
        {
            var window = ShowWindow();
            window.Open(typeof(UIDesignerManager));
        }

        /// <summary>
        /// Initializes the Main Manager and shows the Integrations Manager.
        /// </summary> 
        [MenuItem("Tools/Opsive/Ultimate Inventory System/Integrations Manager", false, 26)]
        public static void ShowIntegrationsManagerWindow()
        {
            var window = ShowWindow();
            window.Open(typeof(IntegrationsManager));
        }

        /// <summary>
        /// Updates the inspector.
        /// </summary>
        private void OnInspectorUpdate()
        {
            UpdateCheck();
        }

        /// <summary>
        /// Is an update available?
        /// </summary>
        /// <returns>True if an update is available.</returns>
        private bool UpdateCheck()
        {
            if (m_UpdateCheckRequest != null && m_UpdateCheckRequest.isDone) {
                if (string.IsNullOrEmpty(m_UpdateCheckRequest.error)) {
                    LatestVersion = m_UpdateCheckRequest.downloadHandler.text;
                }
                m_UpdateCheckRequest = null;
                return false;
            }

            if (m_UpdateCheckRequest == null && DateTime.Compare(LastUpdateCheck.AddDays(1), DateTime.UtcNow) < 0) {
                var url = string.Format("https://opsive.com/asset/UpdateCheck.php?asset=UltimateInventorySystem&version={0}&unityversion={1}&devplatform={2}&targetplatform={3}",
                                            AssetInfo.Version, Application.unityVersion, Application.platform, EditorUserBuildSettings.activeBuildTarget);
                m_UpdateCheckRequest = UnityEngine.Networking.UnityWebRequest.Get(url);
                m_UpdateCheckRequest.SendWebRequest();
                LastUpdateCheck = DateTime.UtcNow;
            }

            return m_UpdateCheckRequest != null;
        }

        /// <summary>
        /// The window has been enabled.
        /// </summary>
        private void OnEnable()
        {
            s_Instance = this;
            rootVisualElement.styleSheets.Add(InspectorUtility.LoadAsset<StyleSheet>("e70f56fae2d84394b861a2013cb384d0"));//shared uss
            rootVisualElement.styleSheets.Add(CommonStyles.StyleSheet);
            rootVisualElement.styleSheets.Add(ManagerStyles.StyleSheet);
            rootVisualElement.styleSheets.Add(ControlTypeStyles.StyleSheet);

            // Initialize the database.
            if (Application.isPlaying) {
                m_Database = FindObjectOfType<InventorySystemManager>().Database;
            } else {
                if (!string.IsNullOrEmpty(DatabaseGUID)) {
                    m_Database = InspectorUtility.LoadAsset<InventorySystemDatabase>(DatabaseGUID);
                }
            }


            // Ensure the database is valid.
            ValidateDatabase();

            StructureVisualLayout();
            DeserializeManagers();
            BuildManagers();

            m_MenuButtons = new MenuButton[m_Managers.Length];
            for (int i = 0; i < m_Managers.Length; ++i) {
                // Add the button for each manager.
                var button = new MenuButton();
                button.userData = i;
                button.clickable.clicked += () =>
                {
                    // Use the userdata to determine which button is clicked.
                    SelectMenu((int)button.userData);
                };
                button.text = m_ManagerNames[i];
                m_MenuElement.Add(button);
                m_MenuButtons[i] = button;
                // If there isn't a database then the button should start disabled.
                if (m_Database == null && m_RequiredDatabaseManagers.Contains(i)) {
                    m_MenuButtons[i].SetEnabled(false);
                }

                // Add the content.
                m_Managers[i].BuildVisualElements();
                m_ContentElement.Add(m_Managers[i].ManagerContentContainer);
                m_Managers[i].ManagerContentContainer.style.display = m_MenuSelection == i ? DisplayStyle.Flex : DisplayStyle.None;
            }

            // The first menu should have spacing at the top.
            m_MenuButtons[0].style.marginTop = 30;

            // Initialize the starting menu.
            m_SelectedManagerLabel.text = m_ManagerNames[m_MenuSelection];
            m_MenuButtons[m_MenuSelection].AddToClassList(ManagerStyles.SelectedMenuButtonBackground);

            // Ensure all of the managers are updated with the current data.
            if (m_Database != null) {
                for (int i = 0; i < m_Managers.Length; ++i) {
                    m_Managers[i].Refresh();
                }
            }

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        /// <summary>
        /// Select the menu at the index provided.
        /// </summary>
        /// <param name="index">The menu index.</param>
        public void SelectMenu(int index)
        {
            // Use the userdata to determine which button is clicked.
            m_Managers[m_MenuSelection].ManagerContentContainer.style.display = DisplayStyle.None;
            m_MenuButtons[m_MenuSelection].RemoveFromClassList(ManagerStyles.SelectedMenuButtonBackground);
            m_MenuSelection = index;
            m_Managers[m_MenuSelection].ManagerContentContainer.style.display = DisplayStyle.Flex;
            m_MenuButtons[m_MenuSelection].AddToClassList(ManagerStyles.SelectedMenuButtonBackground);
            m_Managers[m_MenuSelection].Refresh();
            m_SelectedManagerLabel.text = m_ManagerNames[m_MenuSelection];
        }

        /// <summary>
        /// Creates the base structure for the editor window.
        /// </summary>
        private void StructureVisualLayout()
        {
            var layoutElement = new VisualElement();
            layoutElement.AddToClassList("horizontal-layout");
            rootVisualElement.Add(layoutElement);

            // Menu on the left.
            m_MenuElement = new VisualElement();
            m_MenuElement.name = "menu";
            m_MenuElement.AddToClassList(CommonStyles.s_VerticalLayout);
            m_MenuElement.AddToClassList(ManagerStyles.MenuBackground);
            layoutElement.Add(m_MenuElement);

            // Content on the right.
            m_ContentElement = new VisualElement();
            m_ContentElement.AddToClassList(CommonStyles.s_VerticalLayout);
            layoutElement.Add(m_ContentElement);

            // Place a title at the top.
            m_SelectedManagerLabel = new Label();
            m_SelectedManagerLabel.name = "contentTitle";
            m_ContentElement.Add(m_SelectedManagerLabel);
        }

        /// <summary>
        /// Builds the array which contains all of the IManager objects.
        /// </summary>
        private void BuildManagers()
        {
            var managers = new List<Type>();
            var managerIndexes = new List<int>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; ++i) {
                var assemblyTypes = assemblies[i].GetTypes();
                for (int j = 0; j < assemblyTypes.Length; ++j) {
                    // Must implement Manager.
                    if (!typeof(Manager).IsAssignableFrom(assemblyTypes[j])) {
                        continue;
                    }

                    // Ignore abstract classes.
                    if (assemblyTypes[j].IsAbstract) {
                        continue;
                    }

                    // A valid manager class.
                    managers.Add(assemblyTypes[j]);
                    var index = managerIndexes.Count;
                    if (assemblyTypes[j].GetCustomAttributes(typeof(OrderedEditorItem), true).Length > 0) {
                        var item = assemblyTypes[j].GetCustomAttributes(typeof(OrderedEditorItem), true)[0] as OrderedEditorItem;
                        index = item.Index;
                    }
                    managerIndexes.Add(index);
                }
            }

            // Do not reinitialize the managers if they are already initialized and there aren't any changes.
            if (m_Managers != null && m_Managers.Length == managers.Count) {
                return;
            }

            // All of the manager types have been found. Sort by the index.
            var managerTypes = managers.ToArray();
            Array.Sort(managerIndexes.ToArray(), managerTypes);

            m_Managers = new Manager[managers.Count];
            m_ManagerNames = new string[managers.Count];

            // The manager types have been found and sorted. Add them to the list.
            for (int i = 0; i < managerTypes.Length; ++i) {
                m_Managers[i] = Activator.CreateInstance(managerTypes[i]) as Manager;
                m_Managers[i].Initialize(this);

                var name = InspectorUtility.SplitCamelCase(managerTypes[i].Name);
                if (managers[i].GetCustomAttributes(typeof(OrderedEditorItem), true).Length > 0) {
                    var item = managerTypes[i].GetCustomAttributes(typeof(OrderedEditorItem), true)[0] as OrderedEditorItem;
                    name = item.Name;
                }
                m_ManagerNames[i] = name;

                // The manager may require a database in order to activate.
                if (managerTypes[i].GetCustomAttributes(typeof(RequireDatabase), true).Length > 0) {
                    m_RequiredDatabaseManagers.Add(i);
                }
            }

            SerializeManagers();
        }

        /// <summary>
        /// Opens the specified manager.
        /// </summary>
        /// <param name="managerType">The type of manager to open.</param>
        /// <returns>The index of the opened manager.</returns>
        public int Open(Type managerType)
        {
            for (int i = 0; i < m_Managers.Length; ++i) {
                if (m_Managers[i].GetType() == managerType) {
                    SelectMenu(i);
                    return i;
                }
            }
            return -1;
        }
        
        /// <summary>
        /// Checks if the specified manager is opened.
        /// </summary>
        /// <param name="managerType">The manager to check.</param>
        /// <returns>True if the manager is opened.</returns>
        public bool IsOpen(Manager manager)
        {
            if (m_MenuSelection < 0 || m_MenuSelection >= m_Managers.Length) { return false;}
            return m_Managers[m_MenuSelection] == manager;
        }

        /// <summary>
        /// Serializes the data for each manager.
        /// </summary>
        public void SerializeManagers()
        {
            m_ManagerData = new Serialization[m_Managers.Length];
            for (int i = 0; i < m_Managers.Length; ++i) {
                var serializedValue = new Serialization();
                serializedValue.Serialize(m_Managers[i], true, MemberVisibility.Public);
                m_ManagerData[i] = serializedValue;
            }
        }

        /// <summary>
        /// Updates the data base to the specified value.
        /// </summary>
        /// <param name="database">The new database value.</param>
        private void UpdateDatabase(InventorySystemDatabase database)
        {
            if (Application.isPlaying) {
                var loadedDatabase = FindObjectOfType<InventorySystemManager>().Database;

                if (loadedDatabase != database) {
                    Debug.LogWarning("The database in the main manager must matched the database loaded in the scene.");
                    return;
                }
            }

            if (m_Database == database) { return; }

            m_Database = database;
            if (m_Database != null) {
                EditorPrefs.SetString(DatabaseGUIDKey, AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(m_Database)));
            }

            ValidateDatabase();

            // The menus should reflect the database change.
            for (int i = 0; i < m_RequiredDatabaseManagers.Count; ++i) {
                m_MenuButtons[m_RequiredDatabaseManagers[i]].SetEnabled(m_Database != null);
                m_Managers[m_RequiredDatabaseManagers[i]].BuildVisualElements();
            }

            // The selected manager should update to reflect the change.
            m_Managers[m_MenuSelection].Refresh();
        }

        /// <summary>
        /// Returns the location of the database directory.
        /// </summary>
        /// <returns>The location of the database directory.</returns>
        public string GetDatabaseDirectory()
        {
            return DatabaseValidator.GetDatabaseDirectory(m_Database);
        }

        /// <summary>
        /// Validate the database.
        /// </summary>
        private void ValidateDatabase()
        {
            if (m_Database == null) { return; }

            var valid = DatabaseValidator.CheckIfValid(m_Database, false);
            if (valid == false) {
                Debug.LogWarning("The database is not valid. An autofix process will now run.");
                valid = DatabaseValidator.CheckIfValid(m_Database, true);
                if (valid == false) {
                    Debug.LogError("The database has errors and some could not be automatically fixed.");
                } else { Debug.Log("The database errors have been fixed automatically."); }
            }
        }

        /// <summary>
        /// Navigates to the specified object.
        /// </summary>
        /// <param name="obj">The object that should be selected.</param>
        /// <param name="showWindow">Should the window be shown if it is not already?</param>
        public static void NavigateTo(object obj, bool showWindow = false)
        {
            if (showWindow) {
                ShowWindow();
            }

            if (s_Instance == null) {
                return;
            }
            s_Instance.NavigateToInternal(obj);
        }

        /// <summary>
        /// Navigates to the specified object.
        /// </summary>
        /// <param name="obj">The object that should be selected.</param>
        private void NavigateToInternal(object obj)
        {
            if (obj is Core.ItemCategory itemCategory) {
                var index = Open(typeof(ItemCategoryManager));
                (m_Managers[index] as ItemCategoryManager).Select(itemCategory);
            }
            if (obj is Core.ItemDefinition itemDefinition) {
                var index = Open(typeof(ItemDefinitionManager));
                (m_Managers[index] as ItemDefinitionManager).Select(itemDefinition);
            }
            if (obj is Crafting.CraftingCategory craftingCategory) {
                var index = Open(typeof(CraftingCategoryManager));
                (m_Managers[index] as CraftingCategoryManager).Select(craftingCategory);
            }
            if (obj is Crafting.CraftingRecipe craftingRecipe) {
                var index = Open(typeof(CraftingRecipeManager));
                (m_Managers[index] as CraftingRecipeManager).Select(craftingRecipe);
            }
            if (obj is Exchange.Currency currency) {
                var index = Open(typeof(CurrencyManager));
                (m_Managers[index] as CurrencyManager).Select(currency);
            }
        }

        /// <summary>
        /// An undo or redo has been performed. Refresh the display.
        /// </summary>
        private void OnUndoRedo()
        {
            if (Database == null) { return; }

            //After an undo objects need to be forced to deserialize
            //TODO is there a more efficient way instead of deserializing the entire database?
            Database.Initialize(true);

            //The editor needs to be redrawn
            for (int i = 0; i < m_Managers.Length; ++i) {
                m_Managers[i].Refresh();
            }
        }

        /// <summary>
        /// Deserializes the data for each manager.
        /// </summary>
        private void DeserializeManagers()
        {
            if (m_ManagerData != null) {
                m_Managers = new Manager[m_ManagerData.Length];
                for (int i = 0; i < m_ManagerData.Length; ++i) {
                    m_Managers[i] = m_ManagerData[i].DeserializeFields(MemberVisibility.Public) as Manager;
                    // The object will be null if the class doesn't exist anymore.
                    if (m_Managers[i] == null) {
                        continue;
                    }
                    m_Managers[i].Initialize(this);
                }
            }
        }

        /// <summary>
        /// Reload database on focus.
        /// </summary>
        private void OnFocus()
        {
            if (m_Database == null) { return; }
            
            OnFocusEvent?.Invoke();
            
            if (m_Database == Database) { return; }

            UpdateDatabase(m_Database);
            for (int i = 0; i < m_Managers.Length; i++) {
                m_Managers[i].Refresh();
            }
        }

        /// <summary>
        /// Send event when lost focus.
        /// </summary>
        void OnLostFocus()
        {
            OnLostFocusEvent?.Invoke();
        }

        /// <summary>
        /// MainManager destructor.
        /// </summary>
        ~MainManagerWindow()
        {
            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.ClearAll();
        }
    }

    /// <summary>
    /// Attribute which specifies the name and ordering of the editor items.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OrderedEditorItem : Attribute
    {
        private string m_Name;
        private int m_Index;
        public string Name => m_Name;
        public int Index => m_Index;
        public OrderedEditorItem(string name, int index) { m_Name = name; m_Index = index; }
    }

    /// <summary>
    /// Attribute which specifies that the manager requires a database in order to activate.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RequireDatabase : Attribute
    {
        public RequireDatabase() { }
    }
}