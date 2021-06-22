using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateInventorySystem.ItemActions;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;

using Opsive.Shared.Game;
using System;

[Serializable]
public class HealItemAction : ItemAction
{
    [SerializeField] protected string m_AttributeName = "HealAmount";
    protected override bool CanInvokeInternal(ItemInfo itemInfo, ItemUser itemUser)
    {
        var exampleHealth = itemUser.gameObject.GetCachedComponent<Player>();
        if (exampleHealth == null)
        {
            return false;
        }

        if (itemInfo.Item.GetAttribute<Attribute<int>>(m_AttributeName) == null)
        {
            return false;
        }

        return true;
    }

    protected override void InvokeActionInternal(ItemInfo itemInfo, ItemUser itemUser)
    {
        var exampleHealth = itemUser.gameObject.GetCachedComponent<Player>();
        exampleHealth.Heal(itemInfo.Item.GetAttribute<Attribute<int>>(m_AttributeName).GetValue());
        itemInfo.Inventory.RemoveItem((1, itemInfo));
    }
}

