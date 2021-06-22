namespace Opsive.UltimateInventorySystem.UI.Item.DragAndDrop.DropActions
{
    using System;

    [Serializable]
    public class ItemViewDropMoveIndexAction : ItemViewDropAction
    {
        public override void Drop(ItemViewDropHandler itemViewDropHandler)
        {
            itemViewDropHandler.SourceContainer.MoveItem(itemViewDropHandler.StreamData.SourceIndex, itemViewDropHandler.StreamData.DestinationIndex);
        }
    }
}