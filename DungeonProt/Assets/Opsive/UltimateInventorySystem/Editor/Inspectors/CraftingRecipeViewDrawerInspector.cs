namespace Opsive.UltimateInventorySystem.Editor.Inspectors
{
    using Opsive.UltimateInventorySystem.Editor.VisualElements;
    using Opsive.UltimateInventorySystem.UI.DataContainers;
    using Opsive.UltimateInventorySystem.UI.Item;
    using Opsive.UltimateInventorySystem.UI.Panels.Crafting;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine.UIElements;

    /// <summary>
    /// A custom inspector for the Item View drawer component.
    /// </summary>
    [CustomEditor(typeof(CraftingRecipeViewDrawer), true)]
    public class CraftingRecipeViewDrawerInspector : InspectorBase
    {
        protected override List<string> PropertiesToExclude => new List<string>() { "m_CategoryRecipeViewSet" };

        protected CraftingRecipeViewDrawer m_CraftingRecipViewDrawer;
        protected ObjectFieldWithNestedInspector<CategoryRecipeViewSet, CategoryRecipeViewSetInspector>
            m_CategoryItemViewSet;

        /// <summary>
        /// Initialize the inspector when it is first selected.
        /// </summary>
        protected override void InitializeInspector()
        {
            m_CraftingRecipViewDrawer = target as CraftingRecipeViewDrawer;

            base.InitializeInspector();
        }

        /// <summary>
        /// Create the inspector.
        /// </summary>
        /// <param name="container">The parent container.</param>
        protected override void CreateInspector(VisualElement container)
        {
            m_CategoryItemViewSet = new ObjectFieldWithNestedInspector
                <CategoryRecipeViewSet, CategoryRecipeViewSetInspector>(
                    "Category Item View Set",
                    m_CraftingRecipViewDrawer.CategoryRecipeViewSet,
                    "The Item Views for each itemCategory.",
                    (newValue) =>
                    {
                        m_CraftingRecipViewDrawer.CategoryRecipeViewSet = newValue;
                        Shared.Editor.Utility.InspectorUtility.SetDirty(m_CraftingRecipViewDrawer);
                    });

            container.Add(m_CategoryItemViewSet);
        }
    }
}