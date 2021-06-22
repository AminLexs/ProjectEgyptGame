using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public Player player;
    private Vector2 direction;

    private void Update()
    {
        direction.x = Input.GetAxisRaw("Horizontal"); // Нажатия для x
        direction.y = Input.GetAxisRaw("Vertical"); // Нажатия для y
    }

    private void FixedUpdate()
    {
        if (direction.x != 0 && direction.y != 0)
        {
            direction.x = direction.x * (float)Math.Sqrt(2) / 2;
            direction.y = direction.y * (float)Math.Sqrt(2) / 2;
        }
        player.Move(direction);
    }
}