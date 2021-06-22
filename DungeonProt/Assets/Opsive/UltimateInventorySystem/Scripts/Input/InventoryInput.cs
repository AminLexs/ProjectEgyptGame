namespace Opsive.UltimateInventorySystem.Input
{
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.ItemObjectActions;
    using System;
    using UnityEngine;
    using EventHandler = Opsive.Shared.Events.EventHandler;

    /// <summary>
    /// The component used to get input from the player to control UI and use items.
    /// </summary>
    public abstract class InventoryInput : MonoBehaviour
    {

        protected bool m_IsInputActive = true;

        public virtual bool IsInputActive => isActiveAndEnabled && m_IsInputActive;

        public void SetInputActive(bool active)
        {
            m_IsInputActive = active;
        }
        
        public void Interact()
        {
            EventHandler.ExecuteEvent(gameObject, EventNames.c_GameObject_OnInput_Interact);
        }
        
        public void TriggerPrevious()
        {
            EventHandler.ExecuteEvent(gameObject, EventNames.c_GameObject_OnInput_TriggerPrevious);
        }
        
        public void TriggerNext()
        {
            EventHandler.ExecuteEvent(gameObject, EventNames.c_GameObject_OnInput_TriggerNext);
        }
        
        public void ClosePanel()
        {
            EventHandler.ExecuteEvent(gameObject, EventNames.c_GameObject_OnInput_ClosePanel);
        }

        public void OpenTogglePanel(string panelName, bool toggle)
        {
            if (toggle) {
                EventHandler.ExecuteEvent<string>(gameObject, EventNames.c_GameObject_OnInput_TogglePanel_String, panelName);
            } else {
                EventHandler.ExecuteEvent<string>(gameObject, EventNames.c_GameObject_OnInput_OpenPanel_String, panelName);
            }
        }

        public void UseItemAction(int index)
        {
            EventHandler.ExecuteEvent<int>(gameObject, EventNames.c_GameObject_OnInput_ItemAction_Int, index);
        }

        /// <summary>
        /// The input for the quick item at the index provided.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <returns>True if the input is valid.</returns>
        public void UseHotbarItem(int index)
        {
            EventHandler.ExecuteEvent<int>(gameObject, EventNames.c_GameObject_OnInput_HotbarUseItem_Int,index);
        }
        
        public void UseItemObject(int itemObjectSlotIndex, int actionIndex)
        {
            EventHandler.ExecuteEvent<int,int>(gameObject, EventNames.c_GameObject_OnInput_UseItemObject_Int_Int,itemObjectSlotIndex,actionIndex);
        }
    }
    
    /// <summary>
    /// Hot bar input.
    /// </summary>
    [Serializable]
    public struct IndexedInput
    {
        public int Index;
        public KeyStringStandardInput Input;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="index">The hot bar slot index affected by this input.</param>
        /// <param name="input">The input that use the item in the hotbar.</param>
        public IndexedInput(int index, KeyStringStandardInput input)
        {
            Index = index;
            Input = input;
        }

        public bool GetInput()
        {
            return Input.GetInput();
        }
    }
    
    /// <summary>
    /// Hot bar input.
    /// </summary>
    [Serializable]
    public struct UsableItemObjectInput
    {
        public int ItemObjectIndex;
        public int ActionIndex;
        public KeyStringStandardInput Input;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="index">The hot bar slot index affected by this input.</param>
        /// <param name="keyStringInput">The input that use the item in the hotbar.</param>
        public UsableItemObjectInput(int itemObjectIndex, int actionIndex, KeyStringStandardInput keyStringInput)
        {
            ItemObjectIndex = itemObjectIndex;
            ActionIndex = actionIndex;
            Input = keyStringInput;
        }

        public bool GetInput()
        {
            return Input.GetInput();
        }
    }
    
    /// <summary>
    /// Hot bar input.
    /// </summary>
    [Serializable]
    public struct OpenToggleInput
    {
        public bool Toggle;
        public string PanelName;
        public KeyStringStandardInput Input;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="panelName">The name of the input.</param>
        /// <param name="input">The input that use the item in the hotbar.</param>
        public OpenToggleInput(bool toggle, string panelName, KeyStringStandardInput input)
        {
            Toggle = toggle;
            PanelName = panelName;
            Input = input;
        }

        public bool GetInput()
        {
            return Input.GetInput();
        }
    }
}