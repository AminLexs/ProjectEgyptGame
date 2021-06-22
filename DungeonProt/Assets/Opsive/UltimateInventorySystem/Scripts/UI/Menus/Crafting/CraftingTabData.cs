namespace Opsive.UltimateInventorySystem.UI.Menus.Crafting
{
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Crafting;
    using Opsive.UltimateInventorySystem.UI.Grid;
    using UnityEngine;

    /// <summary>
    /// Crafting tab data used by the Crafting Menu.
    /// </summary>
    public class CraftingTabData : MonoBehaviour, IFilterSorter<CraftingRecipe>
    {
        [SerializeField] protected DynamicItemCategory m_ItemCategory;
        [SerializeField] protected DynamicCraftingCategory m_CraftingCategory;

        public ItemCategory ItemCategory => m_ItemCategory;
        public CraftingCategory CraftingCategory => m_CraftingCategory;

        protected bool m_IsInitialized;
        protected IFilterSorter<CraftingRecipe> m_CraftingRecipeFilter;

        public IFilterSorter<CraftingRecipe> CraftingFilter => m_CraftingRecipeFilter;

        public virtual void Initialize(bool force)
        {
            if(m_IsInitialized && ! force){ return; }

            m_CraftingRecipeFilter = this;
        }

        public ListSlice<CraftingRecipe> Filter(ListSlice<CraftingRecipe> input, ref CraftingRecipe[] outputPooledArray)
        {
            var itemCategory = m_ItemCategory.Value;
            var craftingCategory = m_CraftingCategory.Value;

            var count = 0;
            for (int i = 0; i < input.Count; i++) {

                var recipe = input[i];
                
                if (FilterItemCategory(recipe, itemCategory) == false) {
                    continue;
                }

                if (FilterCraftingCategory(craftingCategory, recipe) == false) {
                    continue;
                }

                count++;
                outputPooledArray.ResizeIfNecessary(ref outputPooledArray, count);
                outputPooledArray[count - 1] = recipe;
            }

            return (outputPooledArray, 0, count);
        }

        private bool FilterCraftingCategory(CraftingCategory craftingCategory, CraftingRecipe recipe)
        {
            if (m_CraftingCategory.HasValue== false) { return true; }
            
            var inherentlyContains = craftingCategory.InherentlyContains(recipe);
            if (inherentlyContains == false) { return false; }

            return true;
        }

        private bool FilterItemCategory(CraftingRecipe recipe, ItemCategory itemCategory)
        {
            if (m_ItemCategory.HasValue == false) { return true; }

            var itemResult = recipe.DefaultOutput.MainItemAmount;
            if (itemResult.HasValue == false) { return true; }

            var inherentlyContains = itemCategory.InherentlyContains(itemResult.Value.Item);
            if (inherentlyContains == false) { return false; }

            return true;
        }
    }
}