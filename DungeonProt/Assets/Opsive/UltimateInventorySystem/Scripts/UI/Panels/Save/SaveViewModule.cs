/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Panels.Save
{
    using Opsive.UltimateInventorySystem.SaveSystem;
    using Opsive.UltimateInventorySystem.UI.CompoundElements;
    using Opsive.UltimateInventorySystem.UI.Views;
    using System;
    using UnityEngine;

    /// <summary>
    /// A box used to display a save.
    /// </summary>
    public class SaveViewModule : ViewModule<SaveData>
    {
        [Tooltip("The file number format after the save file name.")]
        [SerializeField] protected string m_FileNumberFormat = "{0:00}";
        [Tooltip("The file number text.")]
        [SerializeField] protected Text m_FileNumberText;
        [Tooltip("The save content text.")]
        [SerializeField] protected Text m_SaveContentText;

        protected SaveData m_SaveData;
        protected int m_Index;

        public SaveData SaveData => m_SaveData;
        public int Index => m_Index;

        /// <summary>
        /// The box index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void SetIndex(int index)
        {
            m_Index = index;
        }

        /// <summary>
        /// The the box value.
        /// </summary>
        /// <param name="info">The itemInfo.</param>
        public override void SetValue(SaveData info)
        {
            m_SaveData = info;
            m_FileNumberText.text = string.Format(m_FileNumberFormat, m_Index);

            if (m_SaveData == null) { m_SaveContentText.text = "Empty"; } else {
                m_SaveContentText.text = new DateTime(m_SaveData.DateTimeTicks).ToString();
            }
        }

        /// <summary>
        /// Clear the box.
        /// </summary>
        public override void Clear()
        {
            m_FileNumberText.text = string.Format("{0:000}", m_Index);
            m_SaveContentText.text = "Empty";
        }
    }
}