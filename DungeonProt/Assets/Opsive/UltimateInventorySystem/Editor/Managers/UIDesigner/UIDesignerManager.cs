namespace Opsive.UltimateInventorySystem.Editor.Managers.UIDesigner
{
    using Opsive.Shared.Editor.UIElements;
    using Opsive.Shared.Editor.Utility;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Editor.Styles;
    using Opsive.UltimateInventorySystem.Editor.VisualElements;
    using Opsive.UltimateInventorySystem.UI.Panels;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Object = UnityEngine.Object;

    public enum NavigationOption
    {
        Buttons,
        ScrollStep,
        ScrollView,
        //Page,
        None
    }

    /// <summary>
    /// The SetupManager shows any project or scene related setup options.
    /// </summary>
    [OrderedEditorItem("UI Designer", 6)]
    [RequireDatabase]
    public class UIDesignerManager : Manager
    {
        public const string UIDesignerSchemaClassicGUID = "11cd918b7dca149428d825071dbd2b87";
        public const string UIDesignerSchemaShooterGUID = "b5929dd6bcdf6034289f4ce00e4d2abd";

        // The UIBuilderConfig path is based on the project.
        private static string UIBuilderConfigGUIDKey => "Opsive.UltimateInventorySystem.UIBuilderConfigGUID."+Application.productName;
        private static string UIBuilderConfigGUID { get => EditorPrefs.GetString(UIBuilderConfigGUIDKey, string.Empty); }
        
        private static string UIBuilderTabIndexKey => "Opsive.UltimateInventorySystem.UIBuilderTabIndex."+Application.productName;
        private static int UIBuilderTabIndex { get => EditorPrefs.GetInt(UIBuilderTabIndexKey, 0); }
        
        protected TabToolbar m_Toolbar;
        protected VisualElement m_Container;

        protected List<UIDesignerTabContentBase> m_Builders;
        protected SetupDesigner m_SetupDesigner;
        protected MainMenuDesigner m_MainMenuDesigner;
        protected InventoryGridDesigner m_InventoryGridDesigner;
        protected EquipmentDesigner m_EquipmentDesigner;
        protected HotbarDesigner m_HotbarDesigner;
        protected ShopDesigner m_ShopDesigner;
        protected CraftingDesigner m_CraftingDesigner;
        protected SaveDesigner m_SaveDesigner;
        protected ItemDescriptionDesigner m_ItemDescriptionDesigner;
        protected CurrencyDesigner m_CurrencyDesigner;
        protected InventoryMonitorDesigner m_InventoryMonitorDesigner;
        protected ItemViewDesigner m_ItemViewDesigner;
        protected AttributeViewDesigner m_AttributeViewDesigner;
        
        protected (bool isValid,string message) m_UIDesignerValidationResult = (false, "Not Yet Checked");
        public (bool isValid,string message) UIDesignerValidationResult => m_UIDesignerValidationResult;

        private static UIDesignerManager s_Instance;
        public static UIDesignerManager Instance => s_Instance;
        
        private static UIDesignerSchema s_UIDesignerSchema;

        public static UIDesignerSchema UIDesignerSchema
        {
            get
            {
                if (s_UIDesignerSchema != null) { return s_UIDesignerSchema; }
                
                s_UIDesignerSchema = InspectorUtility.LoadAsset<UIDesignerSchema>(UIBuilderConfigGUID);
                return s_UIDesignerSchema;
            }
            set
            {
                s_UIDesignerSchema = value;
                if (s_UIDesignerSchema != null) {
                    EditorPrefs.SetString(UIBuilderConfigGUIDKey, AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(s_UIDesignerSchema)));
                } else {
                    EditorPrefs.SetString(UIBuilderConfigGUIDKey, "");
                }
            }
        }

        public override void BuildVisualElements()
        {
            if(m_MainManagerWindow.Database == null){ return; }
            
            m_MainManagerWindow.OnFocusEvent += OnFocus;
            
            m_ManagerContentContainer.Clear();
            s_Instance = this;
            
            m_SetupDesigner = new SetupDesigner();
            m_MainMenuDesigner = new MainMenuDesigner();
            m_InventoryGridDesigner = new InventoryGridDesigner();
            m_EquipmentDesigner = new EquipmentDesigner();
            m_HotbarDesigner = new HotbarDesigner();
            m_ShopDesigner = new ShopDesigner();
            m_CraftingDesigner = new CraftingDesigner();
            m_SaveDesigner = new SaveDesigner();
            m_ItemDescriptionDesigner = new ItemDescriptionDesigner();
            m_CurrencyDesigner = new CurrencyDesigner();
            m_InventoryMonitorDesigner = new InventoryMonitorDesigner();
            m_ItemViewDesigner = new ItemViewDesigner();
            m_AttributeViewDesigner = new AttributeViewDesigner();

            m_Builders = new List<UIDesignerTabContentBase>()
            {
                m_SetupDesigner,
                m_MainMenuDesigner,
                m_InventoryGridDesigner,
                m_EquipmentDesigner,
                m_HotbarDesigner,
                m_ShopDesigner,
                m_CraftingDesigner,
                m_SaveDesigner,
                m_ItemDescriptionDesigner,
                m_CurrencyDesigner,
                m_InventoryMonitorDesigner,
                m_ItemViewDesigner,
                m_AttributeViewDesigner,
            };

            var titleList = new string[m_Builders.Count];
            for (int i = 0; i < titleList.Length; i++) { titleList[i] = m_Builders[i].Title; }

            m_Toolbar = new TabToolbar(titleList, UIBuilderTabIndex, ChangeTab);
            m_Toolbar.style.flexWrap = Wrap.Wrap;
            m_ManagerContentContainer.Add(m_Toolbar);
            
            m_Container = new ScrollView(ScrollViewMode.Vertical);
            m_ManagerContentContainer.Add(m_Container);
            
            ChangeTab(m_Toolbar.Selected);
        }

        private void OnFocus()
        {
            if(m_MainManagerWindow.IsOpen(this) == false){ return; }
            
            Refresh();
        }

        public override void Refresh()
        {
            base.Refresh();

            //In case it refreshes while the code is compiling stop before null exception.
            if(m_Builders == null){return;}

            var result = IsSetupValid();
            m_UIDesignerValidationResult = result;
            
            for (int i = 1; i < m_Builders.Count; i++) {
                m_Toolbar.EnableButton(i, result.isValid );
            }
            
            if (result.isValid == false) {
                m_Toolbar.Selected = 0;
                m_Builders[0].Refresh();
                ChangeTab(0);
                return;
            }

            for (int i = 0; i < m_Builders.Count; i++) {
                m_Builders[i].Refresh();
            }
        }

        private (bool isValid, string message) IsSetupValid()
        {
            if (Application.isPlaying) {
                return (false, "You may not use the UI Designer at runtime!");
            }
            
            if (m_MainManagerWindow.Database == null) {
                return (false, "A database must be selected before UIDesigner can be used.");
            }

            if (GameObject.FindObjectOfType<InventorySystemManager>() == null) {
                return (false, "An Inventory System Manager must be present in the scene, use the Editor Setup Manager to create one.");
            }
            
            if (GameObject.FindObjectOfType<DisplayPanelManager>() == null) {
                return (false, "At least one Display Panel Manager must be present in the scene, use the Create UI Mangers.");
            }
            
            if (UIDesignerSchema == null) {
                return (false, "A UI Designer schema must be created and assigned to use the UI Designer Editor Manager.");
            }

            if (IsSchemaFromOpsive()) {
                return (false, "It is required to duplicate one of the available schemas before continuing, this ensures you keep the prefabs in your project folders.");
            }

            return UIDesignerSchema.CheckIfValid();
        }

        public bool IsSchemaFromOpsive()
        {
            #pragma warning disable 0162
            
            #if UIS_DEV
                return false;
            #endif
            
            if (UIDesignerSchema == null) { return false;}
            
            var schemaGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(UIDesignerSchema));
            if (schemaGUID == UIDesignerSchemaClassicGUID 
                || schemaGUID == UIDesignerSchemaShooterGUID) {
                return true;
            }
            
            #pragma warning restore 0162
            return false;
        }

        public T GetTab<T>() where T: UIDesignerTabContentBase
        {
            for (int i = 0; i < m_Builders.Count; i++) {
                if (m_Builders[i] is T builderT) { return builderT; }
            }

            return null;
        }

        public void ChangeTab(UIDesignerTabContentBase tab)
        {
            for (int i = 0; i < m_Builders.Count; i++) {
                if (tab != m_Builders[i]) { continue; }

                ChangeTab(i);
                return;
            }
        }
        
        public void ChangeTab(int index)
        {
            m_Container.Clear();
            
            if(index>=m_Builders.Count){ return; }

            EditorPrefs.SetInt(UIBuilderTabIndexKey, index);
            m_Container.Add(m_Builders[index]);
        }

        public T InstantiateSchemaPrefab<T>(GameObject prefab, RectTransform parent)
        {
            var gameObject = UIDesignerSchema.KeepPrefabLink ?
                PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject :
                GameObject.Instantiate(prefab, parent);
            
            gameObject.name = prefab.name;

            var component = gameObject.GetComponentInChildren<T>();
            return component;
        }
        
        public T InstantiateSchemaPrefab<T>(T prefabComponent, RectTransform parent) where T: Component
        {
            var componentInstance = UIDesignerSchema.KeepPrefabLink ?
                PrefabUtility.InstantiatePrefab(prefabComponent, parent) as T:
                GameObject.Instantiate(prefabComponent, parent);
            
            componentInstance.name = prefabComponent.name;

            var component = componentInstance.GetComponentInChildren<T>();
            return component;
        }
    }

    public abstract class UIDesignerBoxBase : VisualElement
    {
        protected VisualElement m_IconOptions;
        
        public UIDesignerSchema UIDesignerSchema => UIDesignerManager.UIDesignerSchema;
        public UIDesignerManager UIDesignerManager => UIDesignerManager.Instance;

        public virtual string DocumentationURL => null;
        public virtual Func<Component> SelectTargetGetter => null;
        public abstract string Title { get; }
        public abstract string Description { get; }

        public UIDesignerBoxBase()
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

            if (SelectTargetGetter != null) {
                var findIcon = CreateSelectComponentIcon(SelectTargetGetter);
                m_IconOptions.Add(findIcon);
            }
        }

        public static VisualElement CreateSelectComponentIcon(Func<Component> getObject)
        {
            var findIcon = new ComponentSelectionButton(getObject);
            return findIcon;
        }

        public static Button CreateButton(string text, VisualElement container, Action action)
        {
            var button = new Button();
            button.AddToClassList(ManagerStyles.SubMenuButton);
            button.style.marginTop = 4;
            button.text = text;
            button.clicked += action;
            container.Add(button);

            return button;
        }
        
        public void RemoveComponent(Component obj)
        {
            UIDesignerUtility.RemoveComponent(obj);
        }
        
        public void DestroyGameObject(Component obj)
        {
            UIDesignerUtility.DestroyGameObject(obj);
        }
        
        public void DestroyGameObject(GameObject obj)
        {
            UIDesignerUtility.DestroyGameObject(obj);
        }
        
        public RectTransform CreateRectTransform(Transform parent)
        {
            return UIDesignerUtility.CreateRectTransform(parent);
        }
    }

    public class SubTitleLabel : Label
    {
        public SubTitleLabel()
        {
            AddToClassList(ManagerStyles.SubMenuTitle);
        }
        
        public SubTitleLabel(string text) : this()
        {
            this.text = text;
        }
    }
    
    public class SubDescriptionLabel : Label
    {
        public SubDescriptionLabel()
        {
            style.whiteSpace = WhiteSpace.Normal;
        }
        
        public SubDescriptionLabel(string text) : this()
        {
            this.text = text;
        }
    }

    public abstract class UIDesignerTabContentBase : VisualElement
    {
        public UIDesignerSchema UIDesignerSchema => UIDesignerManager.UIDesignerSchema;
        public UIDesignerManager UIDesignerManager => UIDesignerManager.Instance;
        
        public abstract string Title { get; }
        public abstract string Description { get; }

        public UIDesignerTabContentBase()
        {
            style.marginTop = 20f;

            var titleLabel = new Label(Title);
            titleLabel.AddToClassList(ManagerStyles.SubMenuTitle);
            Add(titleLabel);
            
            var descriptionLabel = new Label();
            descriptionLabel.text = Description;
            descriptionLabel.style.whiteSpace = WhiteSpace.Normal;
            Add(descriptionLabel);
        }

        public abstract void Refresh();
    }
    
    public abstract class UIDesignerCreateEditTabContent<To,Tc,Te> : UIDesignerTabContentBase where Tc: UIDesignerCreator<To>, new() where Te : UIDesignerEditor<To>, new() where To : Component
    {

        protected Tc m_DesignerCreator;
        protected Te m_DesignerEditor;

        public Tc DesignerCreator => m_DesignerCreator;
        public Te DesignerEditor => m_DesignerEditor;

        protected UIDesignerCreateEditTabContent()
        {
            m_DesignerCreator = new Tc();
            m_DesignerCreator.OnCreate += (target) =>
            {
                m_DesignerEditor.SetTarget(target);
            };
            Add(m_DesignerCreator);

            m_DesignerEditor = new Te();
            Add(m_DesignerEditor);
        }

        public override void Refresh()
        {
            m_DesignerCreator.Refresh();
            m_DesignerEditor.Refresh();
        }
    }
    
    public abstract class UIDesignerCreator<T> : UIDesignerBoxBase
    {
        public event Action<T> OnCreate;
        
        public override string Title => "Create";
        public override string Description => $"Create a new {typeof(T).Name}";
        
        protected ObjectField m_ParentTransform;
        protected Button m_BuildButton;
        protected VisualElement m_OptionsContentContainer;
        protected VisualElement m_ConditionWarningsContainer;
        protected InventoryHelpBox m_ConditionHelpBox;

        public UIDesignerCreator()
        {
            m_ParentTransform = new ObjectField();
            m_ParentTransform.objectType = typeof(RectTransform);
            m_ParentTransform.label = "Parent Transform";
            m_ParentTransform.RegisterValueChangedCallback(ParentTransformChanged);
            Add(m_ParentTransform);

            m_OptionsContentContainer = new VisualElement();
            Add(m_OptionsContentContainer);
            
            m_ConditionWarningsContainer = new VisualElement();
            m_ConditionHelpBox = new InventoryHelpBox("No Warnings");
            m_ConditionWarningsContainer.Add(m_ConditionHelpBox);
            Add(m_ConditionWarningsContainer);
            
            CreateOptionsContent(m_OptionsContentContainer);

            m_BuildButton = new SubMenuButton("Create",Build);
            Add(m_BuildButton);
            
            Refresh();
        }

        protected virtual void ParentTransformChanged(ChangeEvent<Object> evt)
        {
            Refresh();
        }

        public virtual void Build()
        {
            if (BuildCondition(true) == false) {
                return;
            }

            var result = BuildInternal();
            OnCreate?.Invoke(result);
        }

        public virtual bool BuildCondition(bool logWarnings)
        {
            var parentRect = m_ParentTransform.value as RectTransform;
            if (parentRect == null) {
                m_ConditionHelpBox.SetMessage("Assign a parent transform indicating where spawn the object.");
                return false;
            }

            var panelManager= parentRect.gameObject.GetComponentInParent<DisplayPanelManager>(true);
            if (panelManager == null) {
                m_ConditionHelpBox.SetMessage("The parent transform must have a parent ancestor with a panel manager (usually located next to the canvas).");
                return false;
            }

            return true;
        }

        public virtual void Refresh()
        {
            var buildCondition = BuildCondition(false);
            
            m_ConditionWarningsContainer.Clear();
            if (buildCondition == false) {
                m_ConditionWarningsContainer.Add(m_ConditionHelpBox);
                
            }

            m_BuildButton.SetEnabled(buildCondition);
        }

        protected abstract void CreateOptionsContent(VisualElement container);

        protected abstract T BuildInternal();
    }

    public abstract class UIDesignerEditor<T> : UIDesignerBoxBase where T : Component
    {
        public override string Title => "Edit";
        public override string Description => $"Edit an existing {typeof(T).Name}";
        protected virtual bool RequireDisplayPanel => true;
        public override Func<Component> SelectTargetGetter => () => m_Target;

        protected DisplayPanel m_DisplayPanel;
        protected T m_Target;
        protected GameObject m_TargetObject;
        protected ObjectField m_TargetField;
        protected Button m_FindTargetButton;
        
        protected VisualElement m_TargetOptionsContainer;

        public DisplayPanel DisplayPanel => m_DisplayPanel;
        public T Target => m_Target;

        protected UIDesignerEditor()
        {
            var horizontalLayout = new VisualElement();
            horizontalLayout.style.flexDirection = FlexDirection.Row;
            Add(horizontalLayout);
            
            m_TargetField = new ObjectField("Target UI");
            m_TargetField.style.flexGrow = 1;
            m_TargetField.objectType = typeof(GameObject);
            m_TargetField.RegisterValueChangedCallback(evt =>
            {
                m_TargetObject = evt.newValue as GameObject;
                TargetObjectChanged();
            });
            horizontalLayout.Add(m_TargetField);
            
            m_FindTargetButton = new Button();
            m_FindTargetButton.style.flexShrink = 1;
            m_FindTargetButton.text = "Find Available Targets in Scene";
            m_FindTargetButton.clicked += FindTargetsInScene;
            horizontalLayout.Add(m_FindTargetButton);
            
            if (m_Target == null) {
                SelectFirstAvailableTarget();
            }

            m_TargetOptionsContainer = new VisualElement();
            Add(m_TargetOptionsContainer);
        }

        public void Refresh()
        {
            
            TargetObjectChanged();
        }

        protected virtual void TargetObjectChanged()
        {
            m_TargetOptionsContainer.Clear();

            if (m_TargetObject == null) {
                m_Target = null;
                m_DisplayPanel = null;
                return;
            }
            
            m_Target = m_TargetObject.GetComponent<T>();
            m_DisplayPanel = m_TargetObject.GetComponent<DisplayPanel>();
            
            if (m_Target == null && m_DisplayPanel == null) {
                Debug.LogWarning($"The field only accepts objects with a a display panel with a child target type or an object with the target type {typeof(T)}.");
                m_TargetField.value = null;
                return;
            }
            
            if (m_Target == null) {

                m_Target = m_DisplayPanel.GetComponentInChildren<T>(true);
                if (m_Target == null) {
                    Debug.LogWarning($"The field only accepts objects with a a display panel with a child target type or an object with the target type {typeof(T)}.");
                    m_TargetField.value = null;
                    return;
                }
            }

            if (m_DisplayPanel == null) {
                m_DisplayPanel = m_Target.gameObject.GetComponentInParent<DisplayPanel>(true);

                if (RequireDisplayPanel && m_DisplayPanel == null) {
                    Debug.LogWarning($"The target must have a display panel?.");
                    m_TargetField.value = null;
                    return;
                }
            }

            m_TargetField.SetValueWithoutNotify(m_Target);
            NewValidTargetAssigned();
        }

        public void SetTarget(T newTarget)
        {
            m_Target = newTarget;
            m_TargetField.value = newTarget?.gameObject;
        }

        protected virtual void NewValidTargetAssigned()
        {
        }

        protected void SelectFirstAvailableTarget()
        {
            m_Target = GameObject.FindObjectOfType<T>();
            m_TargetObject = m_Target?.gameObject;
        }

        private void FindTargetsInScene()
        {
            var targetOptions = new GenericMenu();

            var rootGameObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();

            var optionList = new List<string>();

            for (int i = 0; i < rootGameObjects.Length; i++) {
                var availableTargets = rootGameObjects[i].GetComponentsInChildren<T>(true);
                
                for (int j = 0; j < availableTargets.Length; j++) {
                    var localIndex = j;

                    var optionName = GetAvailableName(availableTargets[localIndex].name, optionList);

                    targetOptions.AddItem(new GUIContent(optionName), false, () =>
                    {
                        SetTarget(availableTargets[localIndex]);
                    });
                    
                    optionList.Add(optionName);
                }
            }

            targetOptions.ShowAsContext();
        }

        private string GetAvailableName(string option, List<string> optionList)
        {
            var count = 1;
            var newOption = option;
            
            while (optionList.Contains(newOption)) {
                newOption = $"{option} ({count})";
                count++;
            }

            return newOption;
        }
    }

    public class CreateSelectDeleteContainer : VisualElement
    {
        protected string m_Title;
        protected Action m_Create;
        protected Action m_Delete;
        protected Func<Component> m_GetTarget;

        public bool HasTarget => m_GetTarget?.Invoke() != null;
        
        public CreateSelectDeleteContainer(string title, Action create, Action delete, Func<Component> getTarget)
        {
            AddToClassList(ManagerStyles.CreateDeleteSelectContainer);
            
            m_Title = title;

            m_Create = () =>
            {
                create?.Invoke();
                Refresh();
            };
            
            m_Delete = () =>
            {
                delete?.Invoke();
                Refresh();
            };
            
            m_GetTarget = getTarget;
        }

        public void Refresh()
        {
            Clear();
            
            var target = m_GetTarget();
            if (target == null) {
                var createButton = new SubMenuButton("Create "+m_Title, m_Create);
                Add(createButton);
                return;
            }

            var deleteButton = new SubMenuButton("Delete "+m_Title, m_Delete);
            Add(deleteButton);
            
            var componentSelect = new ComponentSelectionButton("Select "+m_Title, m_GetTarget);
            Add(componentSelect);
        }
    }

    public class SubMenuButton : Button
    {
        public SubMenuButton(string buttonText, Action action)
        {
            AddToClassList(ManagerStyles.SubMenuButton);
            text = buttonText;
            clicked += action;
            
        }
        
        public SubMenuButton(string buttonText, float width, Action action) : this(buttonText,action)
        {
            style.width = width;
        }
    }

    public class ComponentSelectionButton : VisualElement
    {
        private Func<Component> m_GetObject;

        public ComponentSelectionButton(Func<Component> GetObject)
        {
            m_GetObject = GetObject;
            
            var findIcon = new IconOptionButton(IconOption.MagnifyingGlass);
            findIcon.tooltip= "Select the component in the hierarchy";
            findIcon.clicked += SelectInHierarchy;
            Add(findIcon);
        }
        
        public ComponentSelectionButton(string buttonText, Func<Component> GetObject)
        {
            m_GetObject = GetObject;
            
            var button = new SubMenuButton(null, SelectInHierarchy);
            button.tooltip = "Select the component in the hierarchy";
            button.style.flexDirection = FlexDirection.Row;

            var icon = new IconOptionImage(IconOption.MagnifyingGlass);
            button.Add(icon);
            var label = new Label(buttonText);
            button.Add(label);
            
            Add(button);
        }

        public void SelectInHierarchy()
        {
            ComponentSelection.Select(m_GetObject());
        }
    }

    public static class ComponentSelection
    {
        public static void Select(Component obj)
        {
            
            Selection.SetActiveObjectWithContext(obj, obj);
            if(obj == null){ return; }
            
            //var highlightTask = HighlightDelayed("CustomIdentifier"+obj.GetInstanceID());
            //OnlyShowComponent(obj);
            
            var allComponentsOnGameobject = obj.gameObject.GetComponents<Component>();
            for (int i = 0; i < allComponentsOnGameobject.Length; i++) {
                var component = allComponentsOnGameobject[i];
                var componentMatch = component == obj;
                
                UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(component, componentMatch);
            }
            
            Selection.SetActiveObjectWithContext(obj, obj);
        }

        //DOES NOT WORK, STOPS WHEN USING REFLECTION
        private static async Task OnlyShowComponent(Object obj)
        {
            Debug.Log("Yo");
            await Task.Yield();
            Debug.Log("Ya 1");
            
            // now I need to focus on the Inspector view
            // to make sure EditorWindow.focusedWindow will return instance of the InspectorWindow
            EditorApplication.ExecuteMenuItem("Window/General/Inspector");
 
            Debug.Log("Ya 2");
            EditorWindow inspectorWindow = EditorWindow.focusedWindow;
            
            Debug.Log("Ya 3");
            ActiveEditorTracker tracker = (ActiveEditorTracker)inspectorWindow.GetType().GetMethod("GetTracker").Invoke(inspectorWindow, null);
 
            Debug.Log("Ya 4");
            Editor[] editors = tracker.activeEditors;

            Debug.Log(editors.Length);
            for (int i = 0; i < editors.Length; i++) {
                var visible = editors[i].target != obj;
                Debug.Log(visible);
                
                tracker.SetVisible(i, visible ? 0 : 1);
            }
        }
        
        //OBSELETE!
        private static async Task HighlightDelayed(string indentifier)
        {
            await Task.Delay(500);
            //The identifier text is in the InspectorBase script.
            var result = Highlighter.Highlight("Inspector", indentifier, HighlightSearchMode.Identifier);
            Debug.Log(result);
            await Task.Delay(500);
            Highlighter.Stop();
        }
        
    }
    
    public static class UIDesignerUtility{

        public static void RemoveComponent(Component obj)
        {
            if (obj == null) { return; }
            GameObject.DestroyImmediate(obj);
        }
        
        public static void DestroyGameObject(Component obj)
        {
            if (obj == null) { return; }
            GameObject.DestroyImmediate(obj.gameObject);
        }
        
        public static void DestroyGameObject(GameObject obj)
        {
            if (obj == null) { return; }
            GameObject.DestroyImmediate(obj);
        }
        
        public static RectTransform CreateRectTransform(Transform parent)
        {
            var rect = new GameObject().AddComponent<RectTransform>();
            rect.SetParent(parent);
            rect.anchoredPosition = Vector2.zero;
            return rect;
        }
        
        public static T GetComponentInParent<T>(this GameObject obj, bool includeInactive, bool includeThis = true) where T : Component
        {
            if (includeThis) {
                var component = obj.GetComponent<T>();
                if (component != null) { return component; }
            }

            if (includeInactive == false) {
                return obj.GetComponentInParent<T>();
            }
            
            var targetParent = obj.transform.parent;
            while (targetParent != null) {
                var component = targetParent.GetComponent<T>();
                    
                if (component != null) {
                    return component;
                }

                targetParent = targetParent.parent;
            }

            return null;
        }

        public static List<T> FindAllObjectsOfType<T>() where T: Component
        {
            var rootGameObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();

            var allObjects = new List<T>();

            for (int i = 0; i < rootGameObjects.Length; i++) {
                var availableTargets = rootGameObjects[i].GetComponentsInChildren<T>(true);
                
                allObjects.AddRange(availableTargets);
            }

            return allObjects;
        }

    }
}