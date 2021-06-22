/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Panels.Save
{
    using Opsive.Shared.Utility;
    using Opsive.UltimateInventorySystem.ItemActions;
    using Opsive.UltimateInventorySystem.SaveSystem;
    using Opsive.UltimateInventorySystem.UI.Grid;
    using Opsive.UltimateInventorySystem.UI.Panels.ActionPanels;
    using UnityEngine;

    /// <summary>
    /// The Grid UI used for the save menu.
    /// </summary>
    public class SaveGrid : GridGeneric<SaveData>
    {
        [Tooltip("The action panel.")]
        [SerializeField] protected SettableActionPanel m_ActionPanel;
        [Tooltip("The confirmation pop up.")]
        [SerializeField] protected ConfirmationPopUp m_ConfirmationPopUp;

        protected ResizableArray<SaveData> m_SaveDatas;
        protected SettableActionElement[] m_SettableActions;

        private int m_SelectedIndex;
        private SaveData m_SelectedSaveData;

        /// <summary>
        /// Initialize the components.
        /// </summary>
        public override void Initialize(bool force)
        {
            if(m_IsInitialized && force == false){ return; }
            
            base.Initialize(force);

            m_SaveDatas = new ResizableArray<SaveData>();

            m_SettableActions = new SettableActionElement[3];
            m_SettableActions[0] = new SettableActionElement("Save",
                () =>
                {
                    if (m_SelectedIndex > SaveSystemManager.MaxSaves) {
                        Debug.LogWarning($"The max saves data '{SaveSystemManager.MaxSaves}' is smaller than the selected index {m_SelectedIndex}. Please set a higher max saves amount.");
                        return;
                    }

                    if (m_SaveDatas[m_SelectedIndex] == null) {
                        SaveSystemManager.Save(m_SelectedIndex);
                        Refresh();
                        return;
                    }

                    m_ConfirmationPopUp.SetTitle("Are you sure you want to overwrite this save file?");
                    m_ConfirmationPopUp.SetConfirmAction(() =>
                    {
                        SaveSystemManager.Save(m_SelectedIndex);
                        Refresh();
                    });
                    m_ConfirmationPopUp.Open(m_ParentPanel, m_GridEventSystem.GetSelectedButton());

                }, () => true);
            m_SettableActions[1] = new SettableActionElement("Load",
                () =>
                {
                    m_ConfirmationPopUp.SetTitle("Are you sure you want to load this file?");
                    m_ConfirmationPopUp.SetConfirmAction(
                        () =>
                        {
                            SaveSystemManager.Load(m_SelectedIndex);
                            Refresh();
                        });
                    m_ConfirmationPopUp.Open(m_ParentPanel, m_GridEventSystem.GetSelectedButton());
                }, () => m_SelectedSaveData != null);
            m_SettableActions[2] = new SettableActionElement("Delete",
                () =>
                {
                    m_ConfirmationPopUp.SetTitle("Are you sure you want to delete this save file?");
                    m_ConfirmationPopUp.SetConfirmAction(
                        () =>
                        {
                            SaveSystemManager.DeleteSave(m_SelectedIndex);
                            Refresh();
                        });
                    m_ConfirmationPopUp.Open(m_ParentPanel, m_GridEventSystem.GetSelectedButton());
                }, () => m_SelectedSaveData != null);

            m_ActionPanel.AssignActions(m_SettableActions);

            if (m_ActionPanel != null) {
                m_ActionPanel.Close();
            }

            OnElementClicked += OnSaveElementButtonClick;
            OnEmptyClicked += OnEmptyButtonClicked;
        }

        /// <summary>
        /// Refresh the view.
        /// </summary>
        public override void Refresh()
        {
            m_SaveDatas.Clear();
            m_SaveDatas.AddRange(SaveSystemManager.GetSaves());
            if (m_SaveDatas.Count < SaveSystemManager.MaxSaves) {
                for (int i = m_SaveDatas.Count; i < SaveSystemManager.MaxSaves; i++) {
                    m_SaveDatas.Add(null);
                }
            }
            SetElements(m_SaveDatas);

            base.Refresh();
        }

        /// <summary>
        /// Click a button in the grid.
        /// </summary>
        /// <param name="saveData">The save data.</param>
        /// <param name="index">The index.</param>
        private void OnSaveElementButtonClick(SaveData saveData, int index)
        {
            m_SelectedIndex = index + m_StartIndex;
            m_SelectedSaveData = saveData;
            m_ActionPanel.Open(m_ParentPanel, GetButton(index));
        }

        /// <summary>
        /// Click an empty button in the grid.
        /// </summary>
        /// <param name="index">The index.</param>
        private void OnEmptyButtonClicked(int index)
        {
            m_SelectedIndex = index;
            m_SelectedSaveData = null;
            m_ActionPanel.Open(m_ParentPanel, GetButton(index));
        }
    }
}
