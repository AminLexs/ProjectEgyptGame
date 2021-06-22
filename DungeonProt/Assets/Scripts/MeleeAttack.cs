using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public Animator animator;
    public Animator idleAnimator;

    public Transform attackPoint;
    public LayerMask enemyLayers;
    public new Rigidbody2D rigidbody;

    public float attackRange = 0.3f;
    public int damage = 25;

    public float attackRate = 1f;
    private float nextAttackTime = 0f;

    private bool mousePressed = false;

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButton(0) && !mousePressed)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
        mousePressed = Input.GetMouseButton(0) ? true : false;
    }

    void FixedUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).IsName("Sword_Temp"))
            rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Attack()
    {
        // detecting direction
        CorrectAttackPoint();

        // freeze for animation lol
        rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;

        // animation lol
        animator.SetTrigger("Attack");

        // detect enemy
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // damage
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(damage);
        }
    }

    // correcting attacPoint.position considering cursor
    private void CorrectAttackPoint()
    {
        Vector3 worldSpaceMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float dx = worldSpaceMousePosition.x - gameObject.transform.position.x;
        float dy = worldSpaceMousePosition.y - gameObject.transform.position.y;

        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        Vector3 scale = attackPoint.localScale;
        scale.x = Mathf.Abs(scale.x);

        float idle = 1;

        if (Mathf.Abs(dx) > Mathf.Abs(dy))
        {
            scale.x = Mathf.Sign(dx) * scale.x;
            position.x = Mathf.Sign(dx) * 0.5f;

            idle = (Mathf.Sign(dx) > 0) ? 2 : 3;
        }
        else
        {
            rotation.z = Mathf.Sign(dy) * 1f;
            position.y = Mathf.Sign(dy) * 0.5f;

            idle = (Mathf.Sign(dy) > 0) ? 0 : 1;
        }

        idleAnimator.SetFloat("IdleState", idle);

        attackPoint.localPosition = position;
        attackPoint.rotation = rotation;
        attackPoint.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
