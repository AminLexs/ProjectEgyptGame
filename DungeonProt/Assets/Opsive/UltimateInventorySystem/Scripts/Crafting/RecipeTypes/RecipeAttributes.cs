/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Crafting.RecipeTypes
{
    using System;

    /// <summary>
    /// The Attribute used to override the type of crafting ingredients for a crafting recipe.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class OverrideCraftingIngredients : Attribute
    {
        public Type m_OverrideType;

        public OverrideCraftingIngredients(Type overrideType)
        {
            m_OverrideType = overrideType;
        }
    }

    /// <summary>
    /// The Attribute used to override the type of crafting output for a crafting recipe.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class OverrideCraftingOutput : Attribute
    {
        public Type m_OverrideType;

        public OverrideCraftingOutput(Type overrideType)
        {
            m_OverrideType = overrideType;
        }
    }
}
