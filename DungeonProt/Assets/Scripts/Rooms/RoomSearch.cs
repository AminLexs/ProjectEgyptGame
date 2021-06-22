using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomSearch
{
    // Список посещенных вершин
    private HashSet<RoomNode> visited;
    // Путь из начальной вершины в целевую.
    private LinkedList<RoomNode> path;
    private RoomNode goal;

    public LinkedList<RoomNode> DFS(RoomNode start, RoomNode goal)
    {
        visited = new HashSet<RoomNode>();
        path = new LinkedList<RoomNode>();
        this.goal = goal;
        DFS(start);
        if (path.Count > 0)
        {
            path.AddFirst(start);
        }
        return path;
    }

    private bool DFS(RoomNode node)
    {
        if (node == goal)
        {
            return true;
        }
        visited.Add(node);
        foreach (var child in node.Children.Where(x => !visited.Contains(x)))
        {
            if (DFS(child))
            {
                path.AddFirst(child);
                return true;
            }
        }
        return false;
    }
}
