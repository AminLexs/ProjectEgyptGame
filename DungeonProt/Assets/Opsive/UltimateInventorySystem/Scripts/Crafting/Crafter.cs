namespace Opsive.UltimateInventorySystem.UI.Menus
{
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Crafting;
    using Opsive.UltimateInventorySystem.Crafting.Processors;
    using Opsive.UltimateInventorySystem.Storage;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class Crafter : MonoBehaviour, IDatabaseSwitcher
    {
        [Tooltip("The recipes to display in the menu.")]
        [SerializeField] protected DynamicCraftingRecipeArray m_MiscellaneousRecipes;
        [Tooltip("The recipes with the categories specified will be visible in the menu.")]
        [SerializeField] protected DynamicCraftingCategoryArray m_CraftingCategories;
        [Tooltip("Use crafting processor callback to remove ingredient items from the inventory.")]
        [SerializeField] protected bool m_RemoveItemsWithCallback = false;

        protected bool m_IsInitialized;
        protected List<CraftingRecipe> m_CraftingRecipes;
        protected CraftingProcessor m_Processor;
        
        public List<CraftingRecipe> CraftingRecipes => m_CraftingRecipes;

        public CraftingRecipe[] MiscellaneousRecipes
        {
            get => m_MiscellaneousRecipes;
            set => m_MiscellaneousRecipes = value;
        }
        
        public CraftingCategory[] CraftingCategories
        {
            get => m_CraftingCategories;
            set => m_CraftingCategories = value;
        }
        
        public CraftingProcessor Processor
        {
            get => m_Processor;
            set => m_Processor = value;
        }

        private void Awake()
        {
            Initialize(false);
        }

        /// <summary>
        /// Initialize.
        /// </summary>
        public void Initialize(bool force)
        {
            if(m_IsInitialized && force){return; }

            m_IsInitialized = true;
            
            if ((this as IDatabaseSwitcher).IsComponentValidForDatabase(InventorySystemManager.Instance.Database) == false) {
                Debug.LogError("The Crafting interactable behavior has recipes and crafting categories from the wrong database, please fix it.");
                return;
            }

            m_CraftingRecipes = new List<CraftingRecipe>(m_MiscellaneousRecipes.Value);
            if (m_Processor == null) {
                m_Processor = new SimpleCraftingProcessorWithCurrency(m_RemoveItemsWithCallback);
            }

            
            for (int i = 0; i < CraftingCategories.Length; i++) {
                var pooledArray = GenericObjectPool.Get<CraftingRecipe[]>();
                var recipesCount = CraftingCategories[i].GetAllChildrenElements(ref pooledArray);
                for (int j = 0; j < recipesCount; j++) {
                    m_CraftingRecipes.Add(pooledArray[j]);
                }
                GenericObjectPool.Return(pooledArray);
            }
        }

        /// <summary>
        /// Check if the object contained by this component are part of the database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>True if all the objects in the component are part of that database.</returns>
        bool IDatabaseSwitcher.IsComponentValidForDatabase(InventorySystemDatabase database)
        {
            if (database == null) { return false; }

            var recipes = MiscellaneousRecipes;
            if (recipes != null) {
                for (int i = 0; i < recipes.Length; i++) {
                    if (database.Contains(recipes[i])) { continue; }

                    return false;
                }
            }

            var categories = CraftingCategories;
            if (categories != null) {
                for (int i = 0; i < categories.Length; i++) {
                    if (database.Contains(categories[i])) { continue; }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Replace any object that is not in the database by an equivalent object in the specified database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns>The objects that have been changed.</returns>
        ModifiedObjectWithDatabaseObjects IDatabaseSwitcher.ReplaceInventoryObjectsBySelectedDatabaseEquivalents(InventorySystemDatabase database)
        {
            if (database == null) { return null; }

            var recipes = MiscellaneousRecipes;
            if (recipes != null) {
                for (int i = 0; i < recipes.Length; i++) {
                    if (database.Contains(recipes[i])) { continue; }

                    recipes[i] = database.FindSimilar(recipes[i]);
                }
            
                m_MiscellaneousRecipes = new DynamicCraftingRecipeArray(recipes);
            }
           

            var categories = CraftingCategories;
            if (categories != null) {
                for (int i = 0; i < categories.Length; i++) {
                    if (database.Contains(categories[i])) { continue; }

                    categories[i] = database.FindSimilar(categories[i]);
                }

                m_CraftingCategories = new DynamicCraftingCategoryArray(categories);
            }

            return null;
        }
    }
}