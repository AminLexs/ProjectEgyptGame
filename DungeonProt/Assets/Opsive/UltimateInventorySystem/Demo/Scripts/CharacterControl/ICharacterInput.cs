/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Demo.CharacterControl
{
    using Opsive.UltimateInventorySystem.ItemObjectActions;

    /// <summary>
    /// Interface for the character input.
    /// </summary>
    public interface ICharacterInput
    {
        /// <summary>
        /// Update every frame.
        /// </summary>
        void Tick();

        /// <summary>
        /// The horizontal input.
        /// </summary>
        float Horizontal { get; }

        /// <summary>
        /// The vertical input.
        /// </summary>
        float Vertical { get; }

    }
}