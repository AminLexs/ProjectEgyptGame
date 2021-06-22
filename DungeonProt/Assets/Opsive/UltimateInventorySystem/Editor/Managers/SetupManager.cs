/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.Managers
{
    using Opsive.Shared.Editor.Utility;
    using Opsive.Shared.Game;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.InventoryCollections;
    using Opsive.UltimateInventorySystem.DropsAndPickups;
    using Opsive.UltimateInventorySystem.Editor.Managers.UIDesigner;
    using Opsive.UltimateInventorySystem.Editor.Styles;
    using Opsive.UltimateInventorySystem.Editor.Utility;
    using Opsive.UltimateInventorySystem.Editor.VisualElements;
    using Opsive.UltimateInventorySystem.Exchange;
    using Opsive.UltimateInventorySystem.Input;
    using Opsive.UltimateInventorySystem.Interactions;
    using Opsive.UltimateInventorySystem.ItemActions;
    using Opsive.UltimateInventorySystem.SaveSystem;
    using Opsive.UltimateInventorySystem.Storage;
    using Opsive.UltimateInventorySystem.UI;
    using Opsive.UltimateInventorySystem.UI.CompoundElements;
    using Opsive.UltimateInventorySystem.UI.DataContainers;
    using Opsive.UltimateInventorySystem.UI.Item;
    using Opsive.UltimateInventorySystem.UI.Item.ItemViewModules;
    using Opsive.UltimateInventorySystem.UI.Monitors;
    using Opsive.UltimateInventorySystem.UI.Panels;
    using Opsive.UltimateInventorySystem.UI.Panels.Hotbar;
    using Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using UnityEngine.UIElements;
    using Button = UnityEngine.UIElements.Button;
    using Toggle = UnityEngine.UIElements.Toggle;

    /// <summary>
    /// The SetupManager shows any project or scene related setup options.
    /// </summary>
    [OrderedEditorItem("Setup", 1)]
    public class SetupManager : Manager
    {
        
        protected VisualElement m_Container;
        
        private ObjectField m_DatabaseField;

        private ObjectField m_CharacterGameObject;

        private ObjectField m_Inventory;
        private ObjectField m_CurrencyOwner;
        private ObjectField m_GameobjectToSave;
        private ObjectField m_CurrencyOwnerSave;
        private PopupField<string> m_DisplayType;
        private ObjectField m_CategoryItemViews;
        private ObjectField m_CategoryAttributeBoxes;
        private ObjectField m_CategoriesItemActions;

        protected CreateItemObjectTemplates m_CreateItemObjectTemplates;

        /// <summary>
        /// Adds the visual elements to the ManagerContentContainer visual element. 
        /// </summary>
        public override void BuildVisualElements()
        {
            // Database Creation.
            var horizontalLayout = new VisualElement();
            horizontalLayout.style.flexDirection = FlexDirection.Row;
            horizontalLayout.style.minHeight = 22;

            var moreOptions = new IconOptionButton(IconOption.Cog);
            m_DatabaseField = new ObjectField("Database");
            m_DatabaseField.tooltip =
                "The inventory database is used to store all the inventory objects (item category, definition, recipes, ect).";
            m_DatabaseField.style.flexGrow = 1;
            m_DatabaseField.objectType = typeof(InventorySystemDatabase);
            m_DatabaseField.RegisterValueChangedCallback(evt =>
            {
                m_MainManagerWindow.Database = evt.newValue as InventorySystemDatabase;
                moreOptions.SetEnabled(m_MainManagerWindow.Database != null);
            });
            m_DatabaseField.value = m_MainManagerWindow.Database;
            horizontalLayout.Add(m_DatabaseField);

            moreOptions.clicked += () => { ShowDatabaseMoreOptionsMenu(); };
            moreOptions.style.marginTop = 2;
            moreOptions.SetEnabled(m_MainManagerWindow.Database != null);
            horizontalLayout.Add(moreOptions);

            var createDatabaseButton = new Button();
            createDatabaseButton.tooltip = "Creates a new inventory database.";
            createDatabaseButton.text = "New";
            createDatabaseButton.clickable.clicked += CreateDatabase;
            horizontalLayout.Add(createDatabaseButton);

            m_ManagerContentContainer.Add(horizontalLayout);
            
            m_Container = new ScrollView(ScrollViewMode.Vertical);

            CreateMenuBox("Scene Setup", "Adds the Inventory System Manager and other requires Manager components to the scene under a 'Game' game object.", null, "Add Components", () =>
            {
                AddSceneComponents();
                Debug.Log("The components have been added.");
            }, m_Container, true);
            
            CreateMenuBox("Character Setup", "Adds the Inventory, Item User, Inventory Input, Inventory Interactor, Currency Owner and Inventory Identifier components on the game object.", AddCharacterOptions, "Add Components", SetupCharacter, m_Container, true);
            
            CreateMenuBox("Save Setup", "Adds the Inventory Saver and/or the Currency Owner Saver on the selected object.\n" +
                                        "Adds the Save Manager and Inventory System Manager Item Saver if they are missing.", AddSaveOptions, "Add Components", SetupSave, m_Container, true);

            CreateMenuBox("UI Setup", "You may set up and create UI using the UI Designer Manager.", null, "Open UI Designer", () => { MainManagerWindow.ShowUIDesignerWindow(); }, m_Container, true);
            
            m_CreateItemObjectTemplates = new CreateItemObjectTemplates();
            m_Container.Add(m_CreateItemObjectTemplates);
            
            m_ManagerContentContainer.Add(m_Container);
        }

        /// <summary>
        /// Check if the required assets for the menu setups are available.
        /// </summary>
        /// <returns>Return true if at least one of the required assets is missing.</returns>
        private bool AreDemoAssetsMissing()
        {
            return InspectorUtility.LoadAsset<GameObject>("f93adf60a19012a4c9c35fb7595903d0") == null ||
                   InspectorUtility.LoadAsset<GameObject>("25eea7b1384f975459f5e67c2b9b0bde") == null ||
                   InspectorUtility.LoadAsset<CategoryItemViewSet>("b0286cb637ba1a64f81828f2dddf84ad") == null;
        }

        /// <summary>
        /// Populates and shows the More Options Generic Menu.
        /// </summary>
        private void ShowDatabaseMoreOptionsMenu()
        {
            var moreOptions = new GenericMenu();
            moreOptions.AddItem(new GUIContent("Duplicate"), false, () => { DuplicateDatabase(); });
            moreOptions.AddItem(new GUIContent("Generate C# script"), false, () =>
            {
                var databasePath = m_MainManagerWindow.GetDatabaseDirectory();
                var path = EditorUtility.SaveFolderPanel("Generate Database strings", databasePath, "");
                if (path.Length != 0 && Application.dataPath.Length < path.Length) {

                    path = string.Format("Assets/{0}", path.Substring(Application.dataPath.Length + 1));
                    DatabaseNamesScriptGenerator.GenerateDatabaseNamesScript(path, m_MainManagerWindow.Database);
                }
            });

            moreOptions.ShowAsContext();
        }

        /// <summary>
        /// Creates a new database.
        /// </summary>
        private void CreateDatabase()
        {
            var path = EditorUtility.SaveFilePanel("Create Database", "Assets", "InventoryDatabase", "asset");
            if (path.Length != 0 && Application.dataPath.Length < path.Length) {
                var database = ScriptableObject.CreateInstance<InventorySystemDatabase>();

                // Save the database.
                path = string.Format("Assets/{0}", path.Substring(Application.dataPath.Length + 1));
                path = AssetDatabaseUtility.GetPathForNewDatabase(path, out var newFolderPath);

                AssetDatabase.DeleteAsset(path);
                AssetDatabase.CreateAsset(database, path);
                AssetDatabase.ImportAsset(path);

                m_DatabaseField.value = database;
            }
        }

        /// <summary>
        /// Duplicates the selected database.
        /// </summary>
        private void DuplicateDatabase()
        {
            if (m_DatabaseField.value == null) {
                Debug.LogWarning("You must select a database to duplicate in the object field.");
                return;
            }

            var path = EditorUtility.SaveFilePanel("Duplicate Database", "Assets", "InventoryDatabase", "asset");
            if (path.Length != 0 && Application.dataPath.Length < path.Length) {

                path = string.Format("Assets/{0}", path.Substring(Application.dataPath.Length + 1));

                var database = DatabaseDuplicator.DuplicateDatabase(m_DatabaseField.value as InventorySystemDatabase, path);

                m_DatabaseField.value = database;
            }
        }

        /// <summary>
        /// Creates a standard box that can be used to show menu content.
        /// </summary>
        /// <param name="title">The title of the box.</param>
        /// <param name="description">The description of the box.</param>
        /// <param name="options">Any additional options (can be null).</param>
        /// <param name="buttonTitle">The title of the action button.</param>
        /// <param name="buttonAction">The action of the button.</param>
        /// <param name="parent">The VisualElement that the box should be added to.</param>
        public static void CreateMenuBox(string title, string description, Action<VisualElement> options, string buttonTitle, Action buttonAction, VisualElement parent, bool enableButton)
        {
            var container = new VisualElement();
            container.AddToClassList(ManagerStyles.SubMenu);
            container.style.marginTop = 20f;
            var titleLabel = new Label(title);
            titleLabel.AddToClassList(ManagerStyles.SubMenuTitle);
            container.Add(titleLabel);
            var destriptionLabel = new Label();
            destriptionLabel.text = description;
            container.Add(destriptionLabel);
            if (options != null) {
                destriptionLabel.style.marginBottom = 4;
                options(container);
            }

            if (buttonAction != null) {
                var button = new Button();
                button.AddToClassList(ManagerStyles.SubMenuButton);
                button.style.marginTop = 4;
                button.text = buttonTitle;
                button.clicked += buttonAction;
                button.SetEnabled(enableButton);
                container.Add(button);
            }

            parent.Add(container);
        }

        /// <summary>
        /// Adds the singleton components to the scene.
        /// </summary>
        private void AddSceneComponents()
        {
            // Create the "Game" components if it doesn't already exists.
            Scheduler scheduler;
            GameObject gameGameObject;
            if ((scheduler = GameObject.FindObjectOfType<Scheduler>()) == null) {
                gameGameObject = new GameObject("Game");
            } else {
                gameGameObject = scheduler.gameObject;
            }

            // Add the Singletons.
            var inventorySystemManager = InspectorUtility.AddComponent<Core.InventorySystemManager>(gameGameObject);
            if (m_MainManagerWindow.Database != null) {
                inventorySystemManager.Database = m_MainManagerWindow.Database;
            }
            InspectorUtility.AddComponent<Scheduler>(gameGameObject);
            InspectorUtility.AddComponent<ObjectPool>(gameGameObject);
        }

        /// <summary>
        /// Adds the options for the character setup.
        /// </summary>
        /// <param name="parent">The VisualElement that the options should be parented to.</param>
        private void AddCharacterOptions(VisualElement parent)
        {
            // The Player tag will be used to attempt to auto populate the objects.
            var player = GameObject.FindWithTag("Player");

            m_CharacterGameObject = new ObjectField();
            m_CharacterGameObject.tooltip = "The character game object where the inventory components will be added.";
            m_CharacterGameObject.label = "Character Gameobject";
            m_CharacterGameObject.objectType = typeof(GameObject);
            m_CharacterGameObject.value = player;
            parent.Add(m_CharacterGameObject);
        }
        
        /// <summary>
        /// Adds the options for the save menu box.
        /// </summary>
        /// <param name="parent">The VisualElement that the options should be parented to.</param>
        private void AddSaveOptions(VisualElement parent)
        {
            m_GameobjectToSave = new ObjectField();
            m_GameobjectToSave.tooltip = "The inventory or currency owner that should be saved.";
            m_GameobjectToSave.label = "Object to save";
            m_GameobjectToSave.objectType = typeof(GameObject);
            parent.Add(m_GameobjectToSave);
        }

        /*
        /// <summary>
        /// Creates a new SciptableObject with the specified name.
        /// </summary>
        /// <param name="name">The name of the ScriptableObject.</param>
        private T CreateScriptableObject<T>(string name) where T : ScriptableObject
        {
            var path = EditorUtility.SaveFilePanel($"Create {typeof(T)}", "Assets", name, "asset");
            if (path.Length == 0 || Application.dataPath.Length >= path.Length) { return null; }
            path = $"Assets/{path.Substring(Application.dataPath.Length + 1)}";

            // Persists the object.
            var obj = ScriptableObject.CreateInstance<T>();
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(obj, path);
            AssetDatabase.ImportAsset(path);
            return obj;
        }*/

        /// <summary>
        /// Instantiate a prefab.
        /// </summary>
        /// <param name="obj">The prefab to instantiate.</param>
        /// <returns>The instance of the prefab.</returns>
        private GameObject InstantiatePrefab(GameObject obj, Transform parent = null)
        {
            if (parent == null) {
                return GameObject.Instantiate(obj);
            } else {
                return GameObject.Instantiate(obj, parent);
            }
        }

        /// <summary>
        /// Set up the character components.
        /// </summary>
        private void SetupCharacter()
        {
            var character = m_CharacterGameObject.value as GameObject;

            if (character == null) { return; }
            
            if (character.GetComponent<InventoryIdentifier>() == null) {
                character.AddComponent<InventoryIdentifier>();
            }

            if (character.GetComponent<Inventory>() == null) { character.AddComponent<Inventory>(); }
            if (character.GetComponent<ItemUser>() == null) { character.AddComponent<ItemUser>(); }

            if (character.GetComponent<InventoryInput>() == null) {
                var input = character.AddComponent<InventoryStandardInput>();
                character.GetComponent<ItemUser>().InventoryInput = input;
            }

            if (character.GetComponent<CurrencyOwner>() == null) {
                character.AddComponent<CurrencyOwner>();
            }
            
            if (character.GetComponent<InventoryInteractor>() == null) {
                character.AddComponent<InventoryInteractor>();
            }
        }

        /// <summary>
        /// Sets up the saving and loading components/UI.
        /// </summary>
        private void SetupSave()
        {
            // Setup the managers.
            AddSceneComponents();
            var gameGameObject = GameObject.FindObjectOfType<InventorySystemManager>().gameObject;
            InspectorUtility.AddComponent<SaveSystem.SaveSystemManager>(gameGameObject);
            InspectorUtility.AddComponent<SaveSystem.InventorySystemManagerItemSaver>(gameGameObject);

            //Add the saver components on the player inventory and currency owner
            var targetGameObject = m_GameobjectToSave.value as GameObject;
            if(targetGameObject == null){ return; }
            
            var inventory = targetGameObject.GetComponent<Inventory>();
            if (inventory != null) {
#if FIRST_PERSON_CONTROLLER || THIRD_PERSON_CONTROLLER
                Debug.Log("The UCC integration character cannot use the default Inventory Saver component. Please add an Inventory Bridge Saver component on your character instead.");
#else
                InspectorUtility.AddComponent<InventorySaver>(targetGameObject);
#endif

            }

            var currencyOwner = targetGameObject.GetComponent<CurrencyOwner>();
            if (currencyOwner != null) {
                InspectorUtility.AddComponent<CurrencyOwnerSaver>(targetGameObject);
            }
        }

        /// <summary>
        /// The database has been changed.
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            m_DatabaseField.value = m_MainManagerWindow.Database;
        }
        
        public abstract class SetupBoxBase : VisualElement
        {
            protected VisualElement m_IconOptions;

            public virtual string DocumentationURL => "yo";
            public abstract string Title { get; }
            public abstract string Description { get; }

            public SetupBoxBase()
            {
                AddToClassList(ManagerStyles.SubMenu);
                style.marginTop = 20f;
                
                var topHorizontalLayout = new VisualElement();
                topHorizontalLayout.AddToClassList(ManagerStyles.SubMenuTop);
                Add(topHorizontalLayout);
                
                var titleLabel = new Label(Title);
                titleLabel.AddToClassList(ManagerStyles.SubMenuTitle);
                topHorizontalLayout.Add(titleLabel);
                
                m_IconOptions = new VisualElement();
                m_IconOptions.AddToClassList(ManagerStyles.SubMenuIconOptions);
                topHorizontalLayout.Add(m_IconOptions);

                DrawIconOptions();
                
                var descriptionLabel = new Label();
                descriptionLabel.AddToClassList(ManagerStyles.SubMenuIconDescription);
                descriptionLabel.text = Description;
                Add(descriptionLabel);
            }

            public virtual void DrawIconOptions()
            {
                m_IconOptions.Clear();
                
                if (string.IsNullOrWhiteSpace(DocumentationURL) == false) {
                    var documentationIcon = new IconOptionButton(IconOption.QuestionMark);
                    documentationIcon.tooltip = "Open the documentation";
                    documentationIcon.clicked += () =>
                    {
                        Application.OpenURL(DocumentationURL);
                    };
                    m_IconOptions.Add(documentationIcon);
                }
            }
        }
        
        public class CreateItemObjectTemplates : SetupBoxBase
        {
            public override string Title => "Create Item Object Templates";

            public override string Description =>
                "Create templates for Item Object Pickups and more which can easily be reused.\n" +
                "Instead of creating one for each item, Create just a few a replace the model/sprite/components/values dynamically when binding it with an Item.";

            public enum Option
            {
                ItemObjectPickup
            }

            private EnumField m_Option;
            private VisualElement m_SelectionContainer;

            private InventoryHelpBox m_HelpBox;

            private VisualElement m_ItemObjectPickupContainer;
            private ObjectField m_DefaultPickupPrefab;
            private ObjectField m_ItemViewPickupPrefab;
            private Toggle m_3DPickup;
            
            private VisualElement m_UsableItemObjectContainer;

            private Button m_BuildButton;
            
            public CreateItemObjectTemplates()
            {
                m_Option = new EnumField("Option", Option.ItemObjectPickup);
                m_Option.RegisterValueChangedCallback(evt =>
                {
                    Refresh();
                });
                Add(m_Option);

                m_SelectionContainer = new VisualElement();
                Add(m_SelectionContainer);

                m_HelpBox = new InventoryHelpBox("All good");
                
                m_ItemObjectPickupContainer = new VisualElement();
                
                m_DefaultPickupPrefab = new ObjectField("Default Pickup Model Prefab");
                m_DefaultPickupPrefab.objectType = typeof(GameObject);
                m_DefaultPickupPrefab.tooltip = "The Default Pickup Model must be specified to proceed";
                m_DefaultPickupPrefab.RegisterValueChangedCallback(evt =>
                {
                    Refresh();
                });
                m_ItemObjectPickupContainer.Add(m_DefaultPickupPrefab);
                
                m_ItemViewPickupPrefab = new ObjectField("Item View Prefab");
                m_ItemViewPickupPrefab.objectType = typeof(GameObject);
                m_ItemViewPickupPrefab.tooltip = "The Item View Prefab is optional, it will be added in a world canvas on top of the item pickup";
                m_ItemViewPickupPrefab.RegisterValueChangedCallback(evt =>
                {
                    Refresh();
                });
                m_ItemObjectPickupContainer.Add(m_ItemViewPickupPrefab);
                

                m_3DPickup = new Toggle("3D Pickup");
                m_3DPickup.value = true;
                m_ItemObjectPickupContainer.Add(m_3DPickup);
                
                
                m_UsableItemObjectContainer = new VisualElement();
                
                m_BuildButton = new Button();
                m_BuildButton.AddToClassList(ManagerStyles.SubMenuButton);
                m_BuildButton.style.marginTop = 4;
                m_BuildButton.text = "Create";
                m_BuildButton.clicked += CreateTemplate;
                
                Add(m_BuildButton);
                
                Refresh();
            }

            private void CreateTemplate()
            {
                var option = (Option) m_Option.value;
                switch (option) {
                    case Option.ItemObjectPickup:
                        CreateItemPickup();
                        break;
                    /*case Option.UsableItemObject:
                        CreateUsableItemObject();
                        break;*/
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private void CreateItemPickup()
            {
                var itemPickupGameObject = new GameObject("Item Pickup Template");
                itemPickupGameObject.transform.position = new Vector3(0,1,0);
                itemPickupGameObject.AddComponent<ItemObject>();
                var pickup = itemPickupGameObject.AddComponent<ItemPickup>();
                var visualListener = itemPickupGameObject.AddComponent<ItemObjectView>();
                if (itemPickupGameObject.GetComponent<Interactable>() == null) {
                    itemPickupGameObject.AddComponent<Interactable>();
                }
                
                ComponentSelection.Select(pickup);

                var modelParent = new GameObject("Item Model Parent");
                modelParent.transform.SetParent(itemPickupGameObject.transform);
                modelParent.transform.localPosition = Vector3.zero;

                if (m_3DPickup.value) {
                    //3D
                    itemPickupGameObject.AddComponent<Rigidbody>();
                    var boxCollider = itemPickupGameObject.AddComponent<BoxCollider>();
                    boxCollider.size = new Vector3(0.5f,0.5f,0.5f);
                    boxCollider.center = new Vector3(0,-0.25f,0f);
                    
                    var sphereCollider = modelParent.AddComponent<SphereCollider>();
                    sphereCollider.isTrigger = true;
                } else {
                    //2D
                    itemPickupGameObject.AddComponent<Rigidbody2D>();
                    itemPickupGameObject.AddComponent<BoxCollider2D>();

                    var circleCollider =modelParent.AddComponent<CircleCollider2D>();
                    circleCollider.isTrigger = true;
                }

                visualListener.m_ItemObjectParent = modelParent.transform;
                visualListener.m_DefaultVisualPrefab = m_DefaultPickupPrefab.value as GameObject;
                var model = GameObject.Instantiate(visualListener.m_DefaultVisualPrefab, visualListener.m_ItemObjectParent);

                var itemViewPrefab = m_ItemViewPickupPrefab.value as GameObject;
                if(itemViewPrefab == null){return;}
                
                //Add a canvas with the Item View
                
                var pickupCanvas = new GameObject("Pickup Canvas").AddComponent<Canvas>();
                var rectTransform = pickupCanvas.transform as RectTransform;
                rectTransform.SetParent(itemPickupGameObject.transform);
                pickupCanvas.gameObject.AddComponent<CanvasScaler>();
                
                rectTransform.localScale = new Vector3(0.01f,0.01f,0.01f);
                rectTransform.localPosition = new Vector3(0,0.7f,0);
                rectTransform.sizeDelta = Vector2.zero;

                var itemView = GameObject.Instantiate(itemViewPrefab, pickupCanvas.transform).GetComponent<ItemView>();

                visualListener.ItemView = itemView;
                pickup.m_SelectIndicators = new [] { pickupCanvas.gameObject };
            }

            private void CreateUsableItemObject()
            {
                
            }

            public void Refresh()
            {
                m_SelectionContainer.Clear();
                VisualElement optionContainer = null;
                (bool valid, string message ) result = (false,"");

                var option = (Option) m_Option.value;
                switch (option) {
                    case Option.ItemObjectPickup:
                        optionContainer = m_ItemObjectPickupContainer;
                        result = ItemPickupCondition();
                        break;
                    /*case Option.UsableItemObject:
                        optionContainer = m_UsableItemObjectContainer;
                        result = UsableItemObjectCondition();
                        break;*/
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (result.valid == false) {
                    m_HelpBox.SetMessage(result.message);
                    m_SelectionContainer.Add(m_HelpBox);
                }
                
                m_SelectionContainer.Add(optionContainer);
                
                m_BuildButton.SetEnabled(result.valid);
            }

            private (bool valid, string message) UsableItemObjectCondition()
            {
                return (true, null);
            }

            private (bool valid, string message ) ItemPickupCondition()
            {
                if (m_DefaultPickupPrefab.value == null) {
                    return (false, "A Default Model Pickup Prefab must be specified");
                }

                var itemViewPrefab = m_ItemViewPickupPrefab.value as GameObject;
                if (itemViewPrefab != null && itemViewPrefab.GetComponent<ItemView>() == false ) {
                    return (false, "The Item View Prefab must have an Item View component at it's root");
                }

                return (true, null);
            }
        }
    }
}