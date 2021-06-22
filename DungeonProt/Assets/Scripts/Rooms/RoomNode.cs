using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode
{
    // Комната вершины
    public Room Room { get; }

    // Список соседних вершин
    public List<RoomNode> Children { get; }

    public RoomNode(Room room)
    {
        Room = room;
        Children = new List<RoomNode>();
    }

    // Добавляет новую соседнюю вершину.
    public RoomNode AddChildren(RoomNode node)
    {
        Children.Add(node);
        return this;
    }
}
