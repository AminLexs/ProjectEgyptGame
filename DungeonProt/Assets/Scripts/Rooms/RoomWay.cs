using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomWay : MonoBehaviour
{
    public string PlayerTag = "Player";
    public Room ParentRoom; // —юда отправл€ем сообщение о том, что игрок хочет покинуть комнату

    // ƒл€ комнат-ловушек
    private bool isDoorOpen = true;
    private Collider2D doorCollider;

    // True - открыть, false - закрыть
    public void ChangeDoorStatus(bool state)
    {
        isDoorOpen = state;
        doorCollider.enabled = state;
    }

    public bool IsDoorOpen()
    {
        if (isDoorOpen)
            return true;
        else
            return false;
    }

    private void Start()
    {
        doorCollider = gameObject.GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(PlayerTag))
        {
            ParentRoom.DetectWay(gameObject);
        }
    }
}