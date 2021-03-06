using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject player;

    private MapBuider mapBuider;
    private Room[,] rooms;
    public Room currentRoom;

    private void Start()
    {
        mapBuider = gameObject.GetComponent<MapBuider>();
        rooms = mapBuider.spawnedRooms;
        currentRoom = mapBuider.StartRoom;
    }

    private void OnEnable()
    {
        Room.ChangeRoomEvent += Room_ChangeCurrentRoom;
    }

    private void Room_ChangeCurrentRoom(int nextX, int nextY, int spawnerIndx)
    {
        if ((currentRoom != null) && (currentRoom.PlayerSpawners != null))
        {
            currentRoom = rooms[nextX, nextY];
            player.transform.position = currentRoom.PlayerSpawners[spawnerIndx].position;

            if (currentRoom.name == "FinallRoom(Clone)")
            {
                currentRoom.TopWay.GetComponent<RoomWay>().ChangeDoorStatus(false);
                currentRoom.LeftWay.GetComponent<RoomWay>().ChangeDoorStatus(false);
                currentRoom.RightWay.GetComponent<RoomWay>().ChangeDoorStatus(false);
                currentRoom.BottomWay.GetComponent<RoomWay>().ChangeDoorStatus(false);
            }
        }
    }

    private void OnDisable()
    {
        Room.ChangeRoomEvent -= Room_ChangeCurrentRoom;
    }
}

