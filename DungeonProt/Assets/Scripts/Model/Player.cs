using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour,
                      IUnit,
                      IMovable,
                      IHealable,
                      IWizzard

{
    public int Health { get; set; } = 30;
    public int MaxHealth { get; set; } = 100;
    public float MoveSpeed { get; set; } = 4;
    public int ManaPoints { get; set; } = 50;
    public int MaxMana { get; set; } = 100;
    public ISpell spell { get; set; } = null;

    private Rigidbody2D rb;

    private Animator animator;
    //private float lastIdleState = 1;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    public void Die()
    {
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
        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            Die();
        }
    }
}
