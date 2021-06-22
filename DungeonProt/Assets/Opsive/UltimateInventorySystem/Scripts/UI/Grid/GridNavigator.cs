/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Grid
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public delegate void IndexChangeEvent(int previousIndex, int nextIndex);

    [Serializable]
    public enum SlicerStepOption
    {
        Page,       //The step moves by the grid size amount.
        Vertical,   //The step moves row by row.
        Horizontal, //The step moves column by column.
        Amount      //The step moves by a constant amount.
    }

    public class GridNavigator : GridNavigatorBase
    {
        [SerializeField] protected SlicerStepOption m_StepOption;
        [SerializeField] protected int m_CustomAmount;

        protected int m_Step;

        protected override int Step => m_Step != 0 ? m_Step : GetStep();

        public override void Initialize(bool force)
        {
            if(m_IsInitialized && force == false){ return; }

            m_Step = GetStep();
            
            base.Initialize(force);
        }
        
        protected int GetStep()
        {
            switch (m_StepOption) {
                case SlicerStepOption.Page:
                    return m_Grid.GridSizeCount;
                case SlicerStepOption.Vertical:
                    return m_Grid.GridSize.x;
                case SlicerStepOption.Horizontal:
                    return m_Grid.GridSize.y;
                case SlicerStepOption.Amount:
                    return m_CustomAmount;
            }

            return 0;
        }
        
        public override bool NextSlice()
        {
            var result = base.NextSlice();

            if (result == false) { return false;}

            //Set the selected view slot opposite to the current one.
            if (m_StepOption == SlicerStepOption.Page) {
                if (m_Grid.GridLayoutGroup.startAxis == GridLayoutGroup.Axis.Horizontal) {
                    m_Grid.GridEventSystem.SelectColumnButton(false);
                } else {
                    m_Grid.GridEventSystem.SelectRowButton(false);
                }
            }
            
            return result;
        }

        public override bool PreviousSlice()
        {
            var result = base.PreviousSlice();
            
            if (result == false) { return false;}
            
            //Set the selected view slot opposite to the current one.
            if (m_StepOption == SlicerStepOption.Page) {
                if (m_Grid.GridLayoutGroup.startAxis == GridLayoutGroup.Axis.Horizontal) {
                    m_Grid.GridEventSystem.SelectColumnButton(true);
                } else {
                    m_Grid.GridEventSystem.SelectRowButton(true);
                }
            }
            
            return result;
        }
    }
}