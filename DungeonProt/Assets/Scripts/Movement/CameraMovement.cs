using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public RoomManager RoomManager;
    public float CameraSpeed = 3.55f;

    private Vector3 target;

    private void Update()
    {
        target = RoomManager.currentRoom.transform.position + new Vector3(0, 0, -10);
        gameObject.transform.position = Vector3.Lerp(transform.position, target, CameraSpeed*Time.deltaTime);
    }
}
