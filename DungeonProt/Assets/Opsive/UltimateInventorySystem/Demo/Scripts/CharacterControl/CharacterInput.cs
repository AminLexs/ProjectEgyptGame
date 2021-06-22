/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Demo.CharacterControl
{
    using UnityEngine;

    /// <summary>
    /// The character Input.
    /// </summary>
    public class CharacterInput : MonoBehaviour, ICharacterInput
    {
        [SerializeField] protected string m_HorizontalInput = "Horizontal";
        [SerializeField] protected string m_VerticalInput = "Vertical";

        /// <summary>
        /// Do nothing.
        /// </summary>
        public void Tick()
        {
            //Do nothing.
        }

        public float Horizontal => Input.GetAxis(m_HorizontalInput);
        public float Vertical => Input.GetAxis(m_VerticalInput);
    }
}