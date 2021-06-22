using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed; //скорость бега

    private Rigidbody2D rb;
    private Animator animator;
    float lastIdleState = 0; // в какую сторону персонаж смотрит 0 - вверх, 1 - вниз, 2 - вправо, 3 - влево
    private Vector2 movement;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal"); // Нажатия для x
        movement.y = Input.GetAxisRaw("Vertical"); // Нажатия для y

        animator.SetFloat("Speed", movement.sqrMagnitude); // выставление скорости в состоянии (не является скоростью)
        if (movement.sqrMagnitude == 0) //если перемещение закончилось, зафиксировать в состояниях текущее направление героя
        {
            animator.SetFloat("IdleState", lastIdleState);
        }
        else
        { //получить текущее направление перемещения
            if (movement.x != 0)
                lastIdleState = (movement.x > 0) ? 2 : 3;
            else lastIdleState = (movement.y > 0) ? 0 : 1;
        }
        animator.SetFloat("Horizontal", movement.x); //установить в состояниях нажатия на клавиши по оси x и y
        animator.SetFloat("Vertical", movement.y);
    }

    private void FixedUpdate()
    {
        if (movement.x != 0 && movement.y != 0)
        {
            movement.x = movement.x * (float)Math.Sqrt(2) / 2;
            movement.y = movement.y * (float)Math.Sqrt(2) / 2;
        }
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
