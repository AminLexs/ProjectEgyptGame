namespace Opsive.UltimateInventorySystem.Editor.Managers.UIDesigner
{
    using Opsive.UltimateInventorySystem.Editor.Inspectors;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine.UIElements;

    /// <summary>
    /// The inspector for the categories Item view uis.
    /// </summary>
    [CustomEditor(typeof(UIDesignerSchema), true)]
    public class UIDesignerSchemaInspector : InspectorBase
    {
        protected override List<string> PropertiesToExclude => new List<string>() {};

        protected UIDesignerSchema m_UIDesignerSchema;

        /// <summary>
        /// Initialize the inspector when it is first selected.
        /// </summary>
        protected override void InitializeInspector()
        {
            m_UIDesignerSchema = target as UIDesignerSchema;

            base.InitializeInspector();
        }

        protected override void CreateInspector(VisualElement container)
        { }
    }
}