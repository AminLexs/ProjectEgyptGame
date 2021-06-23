using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour,
                      IUnit,
                      IMovable,
                      IHealable,
                      IWizzard

{
    public int Health { get; set; } = 50;
    public int MaxHealth { get; set; } = 50;
    public float MoveSpeed { get; set; } = 4;
    public int ManaPoints { get; set; } = 50;
    public int MaxMana { get; set; } = 50;
    public ISpell spell { get; set; } = null;

    private Rigidbody2D rb;

    private Animator animator;
    //private float lastIdleState = 1;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponentInChildren<Animator>();
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }

    public void Heal(int hp)
    {
        Health += hp;

        if (Health > MaxHealth)
            Health = MaxHealth;
    }

    public void Move(Vector2 direction)
    {
        rb.MovePosition(rb.position + direction * MoveSpeed * Time.fixedDeltaTime);

        if (rb.constraints != RigidbodyConstraints2D.FreezeAll)
        {
            animator.SetFloat("Speed", direction.sqrMagnitude); // выставление скорости в состоянии (не является скоростью)

            if (direction.sqrMagnitude != 0)
            {
                float idleState = 1;
                if (direction.x != 0)
                    idleState = (direction.x > 0) ? 2 : 3;
                else
                    idleState = (direction.y > 0) ? 0 : 1;
                animator.SetFloat("IdleState", idleState);
            }
        }

        //if (direction.sqrMagnitude == 0) //если перемещение закончилось, зафиксировать в состояниях текущее направление героя
        //{
        //    animator.SetFloat("IdleState", lastIdleState);
        //}
        //else
        //{ //получить текущее направление перемещения
        //    if (direction.x != 0)
        //        lastIdleState = (direction.x > 0) ? 2 : 3;
        //    else lastIdleState = (direction.y > 0) ? 0 : 1;
        //}

        animator.SetFloat("Horizontal", direction.x); //установить в состояниях нажатия на клавиши по оси x и y
        animator.SetFloat("Vertical", direction.y);
    }

    public void RegenMana(int mp)
    {
        ManaPoints += mp;

        if (ManaPoints > MaxMana)
            ManaPoints = MaxMana;
    }

    public void TakeDamage(int damage)
    {
        spriteRenderer.color = Color.red;

        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            Die();
        }
    }

    private int inHurt = 0;
    public void FixedUpdate()
    {
        if (spriteRenderer.color == Color.red)
        {
            inHurt++;
            if (inHurt >= 10)
            {
                inHurt = 0;
                spriteRenderer.color = Color.white;
            }
        }
    }
}
