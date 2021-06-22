using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapBuider : MonoBehaviour
{
    public int MapWidth;
    public int MapHeight;
    public int RoomCount;

    public Room[] RoomPrefabs;
    public Room StartRoom;
    public Room FinallRoom;

    public Room[,] spawnedRooms;
    public Dictionary<Room, int> roomPath;
    public List<RoomNode> roomNodes;

    private void Awake()
    {
        spawnedRooms = new Room[MapHeight, MapWidth];

        StartRoom.x = spawnedRooms.GetLength(0) / 2;
        StartRoom.y = spawnedRooms.GetLength(1) / 2;
        spawnedRooms[StartRoom.x, StartRoom.y] = StartRoom;

        var roomSearch = new RoomSearch();

        roomPath = new Dictionary<Room, int>();

        roomNodes = new List<RoomNode>();
        roomNodes.Add(new RoomNode(StartRoom));

        for (int i = 0; i < RoomCount; i++)
        {
            PlaceOneRoom();
        }

        for (int x = 0; x < spawnedRooms.GetLength(0); x++)
        {
            for (int y = 0; y < spawnedRooms.GetLength(1); y++)
            {
                if (spawnedRooms[x, y] != null)
                {
                    var temp = roomSearch.DFS(roomNodes[0], roomNodes.Find(j => j.Room == spawnedRooms[x,y]));
                    roomPath.Add(spawnedRooms[x, y], temp.Count);
                }
            }
        }

        var max = roomPath.Max(s => s.Value);
        var result = roomPath.Where(s => s.Value.Equals(max)).Select(s => s.Key).ToList();

        var bossRoom = Instantiate(FinallRoom, new Vector3((result[0].x - (spawnedRooms.GetLength(0) / 2)) * 17.8f,
                                                           (result[0].y - (spawnedRooms.GetLength(1) / 2)) * 10, 0), 
                                                           Quaternion.identity);

        bossRoom.TopWay.SetActive(result[0].TopWay.activeSelf);
        bossRoom.LeftWay.SetActive(result[0].LeftWay.activeSelf);
        bossRoom.BottomWay.SetActive(result[0].BottomWay.activeSelf);
        bossRoom.RightWay.SetActive(result[0].RightWay.activeSelf);
        bossRoom.x = result[0].x;
        bossRoom.y = result[0].y;
        spawnedRooms[result[0].x, result[0].y] = bossRoom;

        Destroy(result[0].gameObject);
    }

    private void PlaceOneRoom()
    {
        HashSet<Vector2Int> vacantPlaces = new HashSet<Vector2Int>();

        for (int x = 0; x < spawnedRooms.GetLength(0); x++)
        {
            for (int y = 0; y < spawnedRooms.GetLength(1); y++)
            {
                if (spawnedRooms[x, y] == null) continue;

                int maxX = spawnedRooms.GetLength(0) - 1;
                int maxY = spawnedRooms.GetLength(1) - 1;

                if (x > 0 && spawnedRooms[x - 1, y] == null) vacantPlaces.Add(new Vector2Int(x - 1, y));
                if (y > 0 && spawnedRooms[x, y - 1] == null) vacantPlaces.Add(new Vector2Int(x, y - 1));
                if (x < maxX && spawnedRooms[x + 1, y] == null) vacantPlaces.Add(new Vector2Int(x + 1, y));
                if (y < maxY && spawnedRooms[x, y + 1] == null) vacantPlaces.Add(new Vector2Int(x, y + 1));
            }
        }

        Room newRoom = Instantiate(RoomPrefabs[Random.Range(0, RoomPrefabs.Length)]);

        int limit = 500;
        while (limit-- > 0)
        {
            Vector2Int position = vacantPlaces.ElementAt(Random.Range(0, vacantPlaces.Count));

            var room = ConnectToSomething(newRoom, position);

            if (room != null)
            {
                newRoom.transform.position = new Vector3((position.x - (spawnedRooms.GetLength(0) / 2)) * 17.8f, (position.y - (spawnedRooms.GetLength(1) / 2)) * 10, 0);
                spawnedRooms[position.x, position.y] = newRoom;

                newRoom.x = position.x;
                newRoom.y = position.y;

                // Тут добавляем узел графа
                var newNode = new RoomNode(newRoom);
                roomNodes.Add(newNode);
                roomNodes.Find(x => x.Room == room).AddChildren(newNode);

                break;
            }
        }
    }

    private Room ConnectToSomething(Room room, Vector2Int p)
    {
        int maxX = spawnedRooms.GetLength(0) - 1;
        int maxY = spawnedRooms.GetLength(1) - 1;

        List<Vector2Int> neighbours = new List<Vector2Int>();

        if (room.TopWay != null && p.y < maxY && spawnedRooms[p.x, p.y + 1]?.BottomWay != null)
            neighbours.Add(Vector2Int.up);
        if (room.BottomWay != null && p.y > 0 && spawnedRooms[p.x, p.y - 1]?.TopWay != null)
            neighbours.Add(Vector2Int.down);
        if (room.RightWay != null && p.x < maxX && spawnedRooms[p.x + 1, p.y]?.LeftWay != null)
            neighbours.Add(Vector2Int.right);
        if (room.LeftWay != null && p.x > 0 && spawnedRooms[p.x - 1, p.y]?.RightWay != null)
            neighbours.Add(Vector2Int.left);

        if (neighbours.Count == 0) return null;

        Vector2Int selectedDirection = neighbours[Random.Range(0, neighbours.Count)];
        Room selectedRoom = spawnedRooms[p.x + selectedDirection.x, p.y + selectedDirection.y];

        if (selectedDirection == Vector2Int.up)
        {
            room.TopWay.SetActive(true);
            selectedRoom.BottomWay.SetActive(true);
        }
        else if (selectedDirection == Vector2Int.down)
        {
            room.BottomWay.SetActive(true);
            selectedRoom.TopWay.SetActive(true);
        }
        else if (selectedDirection == Vector2Int.right)
        {
            room.RightWay.SetActive(true);
            selectedRoom.LeftWay.SetActive(true);
        }
        else if (selectedDirection == Vector2Int.left)
        {
            room.LeftWay.SetActive(true);
            selectedRoom.RightWay.SetActive(true);
        }

        return selectedRoom;
    }
}
