namespace Opsive.UltimateInventorySystem.UI.Item.DragAndDrop.DropConditions
{
    using System;

    [Serializable]
    public class ItemViewDropContainerCanMoveCondition : ItemViewDropCondition
    {

        public override bool CanDrop(ItemViewDropHandler itemViewDropHandler)
        {
            if (itemViewDropHandler.SourceContainer != itemViewDropHandler.DestinationContainer) { return false;}
            
            return itemViewDropHandler.SourceContainer.CanMoveItem(
                itemViewDropHandler.SourceIndex, itemViewDropHandler.DestinationIndex);
        }
    }
}