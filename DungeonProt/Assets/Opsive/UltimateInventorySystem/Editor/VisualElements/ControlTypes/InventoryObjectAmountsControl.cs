/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.VisualElements.ControlTypes
{
    using Opsive.Shared.Editor.UIElements.Controls.Types;
    using Opsive.UltimateInventorySystem.Storage;
    using System;
    using System.Reflection;
    using UnityEngine.UIElements;
    using Object = UnityEngine.Object;

    /// <summary>
    /// The base control for inventory object amounts.
    /// </summary>
    public abstract class InventoryObjectAmountsControl : TypeControlBase
    {
        protected InventorySystemDatabase m_Database;

        public override bool UseLabel => true;

        /// <summary>
        /// Returns the control that should be used for the specified ControlType.
        /// </summary>
        /// <param name="unityObject">A reference to the owning Unity Object.</param>
        /// <param name="target">The object that should have its fields displayed.</param>
        /// <param name="field">The field responsible for the control (can be null).</param>
        /// <param name="arrayIndex">The index of the object within the array (-1 indicates no array).</param>
        /// <param name="type">The type of control being retrieved.</param>
        /// <param name="value">The value of the control.</param>
        /// <param name="onChangeEvent">An event that is sent when the value changes. Returns false if the control cannot be changed.</param>
        /// <param name="userData">Optional data which can be used by the controls.</param>
        /// <returns>The created control.</returns>
        public override VisualElement GetControl(Object unityObject, object target, FieldInfo field, int arrayIndex, Type type, object value,
            Func<object, bool> onChangeEvent, object userData)
        {
            if (userData is object[] objArray) {
                for (int i = 0; i < objArray.Length; i++) {
                    if (objArray[i] is bool b) {
                        if (b == false) { return null; }
                    }
                    if (objArray[i] is InventorySystemDatabase database) { m_Database = database; }
                }
            } else if (userData is InventorySystemDatabase database) {
                m_Database = database;
            }

            var container = new VisualElement();
            if (field == null) {
                var text = value == null ? "NULL" : value.ToString();
                container.Add(new Label(text));
                return container;
            }

            var objectAmountsView = CreateObjectAmountsView(value, onChangeEvent);
            container.Add(objectAmountsView);

            return container;
        }

        /// <summary>
        /// Creates an ObjectAmountView for the correct object type.
        /// </summary>
        /// <param name="value">The start value.</param>
        /// <param name="onChangeEvent">The onChangeEvent.</param>
        /// <returns>The new ObjectAmountsView.</returns>
        public abstract VisualElement CreateObjectAmountsView(object value, Func<object, bool> onChangeEvent);
    }
}