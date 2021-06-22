/// ---------------------------------------------
/// Ultimate Inventory System.
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Crafting.RecipeTypes
{
    using Opsive.UltimateInventorySystem.Crafting.IngredientsTypes;
    using Opsive.UltimateInventorySystem.Utility;
    using UnityEngine;

    /// <summary>
    /// Crafting Recipe with currency included.
    /// </summary>
    [OverrideCraftingIngredients(typeof(CraftingIngredientsWithCurrency))]
    public class CraftingRecipeWithCurrency : CraftingRecipe
    {
        /// <summary>
        /// Makes sure that the ingredients are of the correct type.
        /// </summary>
        protected override void DeserializeIngredientsInternal()
        {
            if (m_Ingredients.GetType() == typeof(CraftingIngredientsWithCurrency)) { return; }

            var previousIngredients = m_Ingredients;
            m_Ingredients = new CraftingIngredientsWithCurrency();
            ReflectionUtility.ObjectCopy(previousIngredients, m_Ingredients);
        }
    }
    
    public class CraftingRecipeWithTime : CraftingRecipe
    {
        [SerializeField] protected float m_CraftTime;

        public float CraftTime => m_CraftTime;
    }
}
