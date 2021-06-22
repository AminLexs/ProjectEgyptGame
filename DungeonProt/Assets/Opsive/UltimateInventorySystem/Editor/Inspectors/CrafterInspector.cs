namespace Opsive.UltimateInventorySystem.Editor.Inspectors
{
    using Opsive.Shared.Editor.UIElements;
    using Opsive.Shared.Editor.Utility;
    using Opsive.UltimateInventorySystem.Crafting;
    using Opsive.UltimateInventorySystem.Editor.VisualElements;
    using Opsive.UltimateInventorySystem.Exchange;
    using Opsive.UltimateInventorySystem.Storage;
    using Opsive.UltimateInventorySystem.UI.Currency;
    using Opsive.UltimateInventorySystem.UI.Menus;
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;

    /// <summary>
    /// Custom editor to display the category item actions.
    /// </summary>
    [CustomEditor(typeof(Crafter), true)]
    public class CrafterInspector : DatabaseInspectorBase
    {
        protected override List<string> PropertiesToExclude => new List<string>() { "m_MiscellaneousRecipes", "m_CraftingCategories"  };

        protected Crafter m_Crafter;
        protected CraftingCategoryReorderableList m_CraftingCategoryReorderableList;
        protected CraftingRecipeReorderableList m_CraftingRecipeReorderableList;

        /// <summary>
        /// Initialize the inspector when it is first selected.
        /// </summary>
        protected override void InitializeInspector()
        {
            m_Crafter = target as Crafter;

            if (m_Crafter.MiscellaneousRecipes == null) {
                m_Crafter.MiscellaneousRecipes = new CraftingRecipe[0];
                return;
            }
            
            if (m_Crafter.CraftingCategories == null) {
                m_Crafter.CraftingCategories = new CraftingCategory[0];
                return;
            }
            
            base.InitializeInspector();
        }

        /// <summary>
        /// Create the inspector.
        /// </summary>
        /// <param name="container">The parent container.</param>
        protected override void CreateInspector(VisualElement container)
        {

            m_CraftingCategoryReorderableList = new CraftingCategoryReorderableList(
                "Crafting Categories",
                m_Database,
                ()=> m_Crafter.CraftingCategories,
                (newValue)=>
                {
                    m_Crafter.CraftingCategories = newValue;
                    InspectorUtility.SetDirty(m_Crafter);
                });
            container.Add(m_CraftingCategoryReorderableList);
            
            m_CraftingRecipeReorderableList = new CraftingRecipeReorderableList(
                "Miscellaneous Recipes",
                m_Database,
                ()=> m_Crafter.MiscellaneousRecipes,
                (newValue)=>
                {
                    m_Crafter.MiscellaneousRecipes = newValue;
                    InspectorUtility.SetDirty(m_Crafter);
                });
            container.Add(m_CraftingRecipeReorderableList);
        }
    }
}