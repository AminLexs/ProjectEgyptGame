using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject TopWay;
    public GameObject RightWay;
    public GameObject BottomWay;
    public GameObject LeftWay;

    public int x;
    public int y;

    public delegate void ChangeRoomEventHandler(int x, int y, int nexSpawnerInd);
    public static event ChangeRoomEventHandler ChangeRoomEvent;

    public Transform[] PlayerSpawners;

    public void DetectWay(GameObject way)
    {
        if (way != null)
        {
            int nextX = x;
            int nextY = y;
            int nextSpawner = 0;

            if (way == TopWay)
            {
                nextX = x;
                nextY = y + 1;
                nextSpawner = 3;
            }
            else if (way == RightWay)
            {
                nextX = x + 1;
                nextY = y;
                nextSpawner = 0;
            }
            else if (way == BottomWay)
            {
                nextX = x;
                nextY = y - 1;
                nextSpawner = 1;
            }
            else if (way == LeftWay)
            {
                nextX = x - 1;
                nextY = y;
                nextSpawner = 2;
            }

            ChangeRoomEvent(nextX, nextY, nextSpawner);
        }
    }
}
