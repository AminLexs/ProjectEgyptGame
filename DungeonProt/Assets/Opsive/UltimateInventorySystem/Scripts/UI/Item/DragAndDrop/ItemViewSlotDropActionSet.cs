/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------


namespace Opsive.UltimateInventorySystem.UI.Item.DragAndDrop
{
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.UI.Item.DragAndDrop.DropActions;
    using Opsive.UltimateInventorySystem.UI.Item.DragAndDrop.DropConditions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A scriptable object used to define the possible item view drop actions.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemViewSlotDropActionSet",
        menuName = "Ultimate Inventory System/UI/Item View Slot Drop Action Set", order = 1)]
    public class ItemViewSlotDropActionSet : ScriptableObject
    {
        [SerializeField] protected Serialization[] m_ActionsWithConditionsData;
        
        protected ItemViewDropActionsWithConditions[] m_ActionsWithConditions;
        
        protected bool m_Initialized;
        
        public ItemViewDropActionsWithConditions[] ActionsWithConditions
        {
            get => m_ActionsWithConditions;
            set => m_ActionsWithConditions = value;
        }

        /// <summary>
        /// Default constructor adds the basics.
        /// </summary>
        public ItemViewSlotDropActionSet()
        {
            ResetActionsConditionsToDefault();
        }

        public void ResetActionsConditionsToDefault()
        {
            m_ActionsWithConditions = new ItemViewDropActionsWithConditions[4];

            // Same container move.
            m_ActionsWithConditions[0] = new ItemViewDropActionsWithConditions();
            m_ActionsWithConditions[0].Conditions = new ItemViewDropCondition[2];
            m_ActionsWithConditions[0].Actions = new ItemViewDropAction[1];

            m_ActionsWithConditions[0].Conditions[0] = new ItemViewDropContainerCanAddCondition(true,true);
            m_ActionsWithConditions[0].Conditions[1] = new ItemViewDropSameContainerCondition();
            m_ActionsWithConditions[0].Actions[0] = new ItemViewDropMoveIndexAction();
            
            // Container Exchange
            m_ActionsWithConditions[1] = new ItemViewDropActionsWithConditions();
            m_ActionsWithConditions[1].Conditions = new ItemViewDropCondition[1];
            m_ActionsWithConditions[1].Actions = new ItemViewDropAction[1];

            m_ActionsWithConditions[1].Conditions[0] = new ItemViewDropContainerCanAddCondition(true,true);
            m_ActionsWithConditions[1].Actions[0] = new ItemViewDropContainerExchangeAction(true,true,true,true);
            
            // Container exchange with Null Item Source
            m_ActionsWithConditions[2] = new ItemViewDropActionsWithConditions();
            m_ActionsWithConditions[2].Conditions = new ItemViewDropCondition[2];
            m_ActionsWithConditions[2].Actions = new ItemViewDropAction[1];

            m_ActionsWithConditions[2].Conditions[0] = new ItemViewDropContainerCanAddCondition(true,false);
            m_ActionsWithConditions[2].Conditions[1] = new ItemViewDropNullItemCondition(true,false);
            m_ActionsWithConditions[2].Actions[0] = new ItemViewDropContainerExchangeAction(false,true,false,true);
            
            // Container exchange with Null Item Destination
            m_ActionsWithConditions[3] = new ItemViewDropActionsWithConditions();
            m_ActionsWithConditions[3].Conditions = new ItemViewDropCondition[2];
            m_ActionsWithConditions[3].Actions = new ItemViewDropAction[1];

            m_ActionsWithConditions[3].Conditions[0] = new ItemViewDropContainerCanAddCondition(false,true);
            m_ActionsWithConditions[3].Conditions[1] = new ItemViewDropNullItemCondition(false,true);
            m_ActionsWithConditions[3].Actions[0] = new ItemViewDropContainerExchangeAction(true,false,true,false);
            

            Serialize();
        }

        /// <summary>
        /// Initializes the scriptable object to deserialize the abstract arrays.
        /// </summary>
        /// <param name="force">Force initialize the object.</param>
        public virtual void Initialize(bool force)
        {
            if (m_Initialized && !force) { return; }

            Deserialize();
        }


        /// <summary>
        /// Deserialize all the properties of the ItemDefinition.
        /// </summary>
        internal void Deserialize()
        {
            if (m_ActionsWithConditionsData == null) {
                if (m_ActionsWithConditions == null) { m_ActionsWithConditions = new ItemViewDropActionsWithConditions[0]; }
                return;
            }

            m_ActionsWithConditions = new ItemViewDropActionsWithConditions[m_ActionsWithConditionsData.Length];
            for (int i = 0; i < m_ActionsWithConditionsData.Length; i++) {
                var actionsWithConditions = m_ActionsWithConditionsData[i].DeserializeFields(MemberVisibility.Public) as ItemViewDropActionsWithConditions;
                m_ActionsWithConditions[i] = actionsWithConditions;
            }
        }
        
        /// <summary>
        /// Serialize all the properties of the itemDefinition.
        /// </summary>
        public void Serialize()
        {
            m_ActionsWithConditionsData = Serialization.Serialize(m_ActionsWithConditions as IList<ItemViewDropActionsWithConditions>);
        }

        public void HandleItemViewSlotDrop(ItemViewDropHandler itemViewDropHandler)
        {
            var index = GetFirstPassingConditionIndex(itemViewDropHandler);
            if (index == -1) { return; }

            m_ActionsWithConditions[index].Drop(itemViewDropHandler);
        }

        public int GetFirstPassingConditionIndex(ItemViewDropHandler itemViewDropHandler)
        {
            for (int i = 0; i < m_ActionsWithConditions.Length; i++) {
                if (m_ActionsWithConditions[i].CanDrop(itemViewDropHandler)) { return i; }
            }

            return -1;
        }
        
        public ItemViewDropActionsWithConditions GetFirstPassingCondition(ItemViewDropHandler itemViewDropHandler)
        {
            for (int i = 0; i < m_ActionsWithConditions.Length; i++) {
                if (m_ActionsWithConditions[i].CanDrop(itemViewDropHandler)) { return m_ActionsWithConditions[i]; }
            }

            return null;
        }
    }

    [Serializable]
    public abstract class ItemViewDropAction
    {
        public abstract void Drop(ItemViewDropHandler itemViewDropHandler);

        public override string ToString()
        {
            return GetType().Name.Replace("ItemViewDrop", "").Replace("Action", "");
        }
    }

    [Serializable]
    public abstract class ItemViewDropCondition
    {
        public abstract bool CanDrop(ItemViewDropHandler itemViewDropHandler);
        
        public override string ToString()
        {
            return GetType().Name.Replace("ItemViewDrop", "").Replace("Condition", "");
        }
    }

    [Serializable]
    public class ItemViewDropActionsWithConditions
    {
        [SerializeField] protected ItemViewDropCondition[] m_Conditions;
        [SerializeField] protected ItemViewDropAction[] m_Actions;
        

        public ItemViewDropCondition[] Conditions
        {
            get => m_Conditions;
            set => m_Conditions = value;
        }

        public ItemViewDropAction[] Actions
        {
            get => m_Actions;
            set => m_Actions = value;
        }

        public bool CanDrop(ItemViewDropHandler itemViewDropHandler)
        {
            for (int i = 0; i < m_Conditions.Length; i++) {
                if (m_Conditions[i].CanDrop(itemViewDropHandler) == false) { return false; }
            }

            return true;
        }

        public void Drop(ItemViewDropHandler itemViewDropHandler)
        {
            for (int i = 0; i < m_Actions.Length; i++) { m_Actions[i].Drop(itemViewDropHandler); }
        }
    }
}