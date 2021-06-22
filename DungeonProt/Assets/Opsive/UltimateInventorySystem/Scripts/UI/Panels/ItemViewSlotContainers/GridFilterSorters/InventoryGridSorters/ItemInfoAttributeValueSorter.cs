/// ---------------------------------------------
/// Ultimate Inventory System.
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Panels.ItemViewSlotContainers.GridFilterSorters.InventoryGridSorters
{
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.UI.Grid;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ItemInfoAttributeValueSorter : ItemInfoSorterBase
     {
         [SerializeField] protected string m_AttributeName;
         [SerializeField] protected bool m_Ascending = true;
 
         protected Comparer<ItemInfo> m_Comparer;
         public override Comparer<ItemInfo> Comparer => m_Comparer;
 
         protected override void Awake()
         {
             base.Awake();
             m_Comparer = Comparer<ItemInfo>.Create((i1, i2) =>
             {
                 var i1Attribute = i1.Item?.GetAttribute(m_AttributeName);
                 var i2Attribute = i2.Item?.GetAttribute(m_AttributeName);
 
                 if (i1Attribute == null && i2Attribute == null) { return 0; }
 
                 if (i1Attribute == null) { return 1; }
                 if (i2Attribute == null) { return -1; }
 
                 var i1Value = i1Attribute.GetValueAsObject();
                 var i2Value = i2Attribute.GetValueAsObject();
 
                 if (m_Ascending) {
                     if (i1Value is IComparable i1Comparable) {
                         return i1Comparable.CompareTo(i2Value);
                     }
                 } else {
                     if (i2Value is IComparable i2Comparable) {
                         return i2Comparable.CompareTo(i1Value);
                     }
                 }
                 
 
                 return 0;
             });
         }
     }
}