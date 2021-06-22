/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Input
{
    using System;
    using UnityEngine;

    /// <summary>
    /// The input type used to specify when to check for the input.
    /// </summary>
    [Serializable]
    [Flags]
    public enum InputType
    {
        Normal = 1, // Equivalent to Input.GetButton.
        Up = 2,     // Equivalent to Input.GetButtonUp.
        Down = 4,   // Equivalent to Input.GetButtonDown.
    }

    /// <summary>
    /// A struct of a string and a keycode used for input.
    /// </summary>
    [Serializable]
    public struct KeyStringStandardInput
    {
        public string m_ButtonName;
        public KeyCode m_KeyCode;
        public InputType m_InputType;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="buttonName">The button name used by the input manager.</param>
        /// <param name="keyCode">The input key code used by the input manager.</param>
        /// <param name="inputType">The input type.</param>
        public KeyStringStandardInput(string buttonName, KeyCode keyCode, InputType inputType)
        {
            m_ButtonName = buttonName;
            m_KeyCode = keyCode;
            m_InputType = inputType;
        }

        /// <summary>
        /// Check if the input is active
        /// </summary>
        public bool GetInput()
        {
            if ((m_InputType & InputType.Normal) != 0) {
                if (string.IsNullOrWhiteSpace(m_ButtonName) == false) {
                    if (Input.GetButton(m_ButtonName)) { return true; }
                }

                if (Input.GetKey(m_KeyCode)) { return true; }
            }

            if ((m_InputType & InputType.Up) != 0) {
                if (string.IsNullOrWhiteSpace(m_ButtonName) == false) {
                    if (Input.GetButtonUp(m_ButtonName)) { return true; }
                }

                if (Input.GetKeyUp(m_KeyCode)) { return true; }
            }

            if ((m_InputType & InputType.Down) != 0) {
                if (string.IsNullOrWhiteSpace(m_ButtonName) == false) {
                    if (Input.GetButtonDown(m_ButtonName)) { return true; }
                }

                if (Input.GetKeyDown(m_KeyCode)) { return true; }
            }

            return false;
        }
    }
}